using System.Collections.Immutable;
using RawSalt.Graphics.Rendering;
using SlangShaderSharp;

namespace RawSalt.Graphics.Pipelines;

/// <summary>
/// Compiles <see cref="PipelineAnatomy"/> sources with the native Slang compiler.
/// </summary>
/// <remarks>
/// Shaders use row-vector math to match <see cref="System.Numerics.Matrix4x4"/>:
/// <c>mul(mul(mul(position, model), view), projection)</c>.
/// </remarks>
public sealed class SlangCompiler
{
	private static readonly Lazy<IGlobalSession> GlobalSession = new(CreateGlobalSession);

	public CompiledShader Compile(PipelineAnatomy anatomy)
	{
		ArgumentNullException.ThrowIfNull(anatomy);
		if (anatomy.Sources.IsDefaultOrEmpty)
		{
			throw new ArgumentException("Anatomy contains no shader sources.", nameof(anatomy));
		}

		IGlobalSession global = GlobalSession.Value;
		SessionDesc sessionDescription = new()
		{
			Targets = [new TargetDesc { Format = SlangCompileTarget.Spirv, Profile = global.FindProfile("spirv_1_5") }],
		};
		Check(global.CreateSession(sessionDescription, out ISession session), "Failed to create Slang session.");

		List<IComponentType> components = [];
		for (int i = 0; i < anatomy.Sources.Length; i++)
		{
			// TODO: Use assets as filesystem to resolve include dependencies
			string path = $"source{i}.slang";
			IModule? module = session.LoadModuleFromSourceString($"source{i}", path, anatomy.Sources[i],
				out ISlangBlob? diagnostics);
			if (module == null)
			{
				throw new InvalidOperationException($"Failed to compile '{path}':\n{diagnostics?.AsString ?? "unknown error"}");
			}

			components.Add(module);
			for (int e = 0; e < module.GetDefinedEntryPointCount(); e++)
			{
				Check(module.GetDefinedEntryPoint(e, out IEntryPoint entryPoint),
					$"Failed to read entry point {e} of '{path}'.");
				components.Add(entryPoint);
			}
		}

		Check(session.CreateCompositeComponentType(components.ToArray(), out IComponentType composed,
				out ISlangBlob? composeDiagnostics),
			$"Failed to compose shader modules.\n{composeDiagnostics?.AsString}");
		Check(composed.Link(out IComponentType linked, out ISlangBlob? linkDiagnostics),
			$"Failed to link shader program.\n{linkDiagnostics?.AsString}");

		ShaderReflection reflection = linked.GetLayout(0, out ISlangBlob? layoutDiagnostics);
		ImmutableArray<ShaderResource> resources = ReadResources(reflection);

		ImmutableArray<CompiledShaderModule>.Builder stages = ImmutableArray.CreateBuilder<CompiledShaderModule>();
		ImmutableArray<ShaderVertexInput> vertexInputs = [];
		uint vertexStride = 0;
		for (uint i = 0; i < reflection.EntryPointCount; i++)
		{
			EntryPointReflection entryPoint = reflection.GetEntryPointByIndex(i);
			ShaderStages stage = entryPoint.Stage switch
			{
				SlangStage.Vertex => ShaderStages.Vertex,
				SlangStage.Pixel => ShaderStages.Fragment,
				_ => throw new NotSupportedException($"Shader stage {entryPoint.Stage} is not supported yet."),
			};

			if (stages.Any(s => s.Stage == stage))
			{
				throw new InvalidOperationException($"Anatomy contains more than one {stage} entry point.");
			}

			Check(linked.GetEntryPointCode((nint)i, 0, out ISlangBlob? code, out ISlangBlob? codeDiagnostics),
				$"Failed to compile entry point '{entryPoint.Name}'.\n{codeDiagnostics?.AsString}");
			stages.Add(new CompiledShaderModule(stage, code!.Buffer.ToArray()));

			if (stage == ShaderStages.Vertex)
			{
				vertexInputs = ReadVertexInputs(entryPoint, out vertexStride);
			}
		}

		if (stages.Count < 2)
		{
			throw new InvalidOperationException("Anatomy must contain a vertex and a fragment entry point.");
		}

		return new CompiledShader(stages.ToImmutable(), resources, vertexInputs, vertexStride);
	}

	private static IGlobalSession CreateGlobalSession()
	{
		Check(Slang.CreateGlobalSession(Slang.ApiVersion, out IGlobalSession session),
			"Failed to create the Slang global session.");
		return session;
	}

	private static ImmutableArray<ShaderResource> ReadResources(ShaderReflection reflection)
	{
		HashSet<string> names = [];
		ImmutableArray<ShaderResource>.Builder resources = ImmutableArray.CreateBuilder<ShaderResource>(
			(int)reflection.ParameterCount);
		for (uint i = 0; i < reflection.ParameterCount; i++)
		{
			VariableLayoutReflection parameter = reflection.GetParameterByIndex(i);
			string name = parameter.Name;
			if (!names.Add(name))
			{
				throw new InvalidOperationException($"Duplicate shader resource name '{name}'.");
			}

			BindingType type = parameter.TypeLayout.GetBindingRangeType(0) switch
			{
				SlangBindingType.ConstantBuffer => BindingType.UniformBuffer,
				SlangBindingType.CombinedTextureSampler => BindingType.CombinedImageSampler,
				SlangBindingType unsupported =>
					throw new NotSupportedException($"Shader resource '{name}' has unsupported type {unsupported}."),
			};

			uint size = type == BindingType.UniformBuffer
				? (uint)parameter.TypeLayout.ElementTypeLayout.GetSize(SlangParameterCategory.Uniform)
				: 0;

			resources.Add(new ShaderResource(name, parameter.BindingSpace, parameter.BindingIndex, type, size));
		}

		return resources.MoveToImmutable();
	}

	private static ImmutableArray<ShaderVertexInput> ReadVertexInputs(EntryPointReflection entryPoint, out uint stride)
	{
		List<(uint Location, VertexFormat Format)> inputs = [];
		for (uint p = 0; p < entryPoint.ParameterCount; p++)
		{
			TypeLayoutReflection type = entryPoint.GetParameterByIndex(p).TypeLayout;
			if (type.Kind != SlangTypeKind.Struct)
			{
				continue;
			}

			for (uint f = 0; f < type.FieldCount; f++)
			{
				VariableLayoutReflection field = type.GetFieldByIndex(f);
				inputs.Add((field.BindingIndex, ToVertexFormat(field.TypeLayout)));
			}
		}

		inputs.Sort(static (a, b) => a.Location.CompareTo(b.Location));

		ImmutableArray<ShaderVertexInput>.Builder attributes = ImmutableArray.CreateBuilder<ShaderVertexInput>(inputs.Count);
		uint offset = 0;
		uint location = 0;
		foreach ((uint fieldLocation, VertexFormat format) in inputs)
		{
			if (attributes.Count > 0 && fieldLocation == location)
			{
				throw new InvalidOperationException($"Vertex input location {fieldLocation} is used twice.");
			}

			location = fieldLocation;
			attributes.Add(new ShaderVertexInput(fieldLocation, format, offset));
			offset += SizeOf(format);
		}

		stride = offset;
		return attributes.MoveToImmutable();
	}

	private static VertexFormat ToVertexFormat(TypeLayoutReflection type)
	{
		if (type.Kind is not (SlangTypeKind.Scalar or SlangTypeKind.Vector))
		{
			throw new NotSupportedException($"Vertex input of kind {type.Kind} is not supported.");
		}

		uint count = type.Kind == SlangTypeKind.Scalar ? 1 : (uint)type.Type.ElementCount;
		return (type.ScalarType, count) switch
		{
			(SlangScalarType.Float32, 1) => VertexFormat.Float32,
			(SlangScalarType.Float32, 2) => VertexFormat.Float32x2,
			(SlangScalarType.Float32, 3) => VertexFormat.Float32x3,
			(SlangScalarType.Float32, 4) => VertexFormat.Float32x4,
			(SlangScalarType.UInt32, 1) => VertexFormat.UInt32,
			(SlangScalarType.UInt32, 2) => VertexFormat.UInt32x2,
			(SlangScalarType.UInt32, 3) => VertexFormat.UInt32x3,
			(SlangScalarType.UInt32, 4) => VertexFormat.UInt32x4,
			_ => throw new NotSupportedException($"Vertex input {type.ScalarType}x{count} is not supported."),
		};
	}

	private static uint SizeOf(VertexFormat format)
	{
		return format switch
		{
			VertexFormat.Float32 or VertexFormat.UInt32 => 4,
			VertexFormat.Float32x2 or VertexFormat.UInt32x2 => 8,
			VertexFormat.Float32x3 or VertexFormat.UInt32x3 => 12,
			VertexFormat.Float32x4 or VertexFormat.UInt32x4 => 16,
			_ => throw new ArgumentOutOfRangeException(nameof(format)),
		};
	}

	private static void Check(SlangResult result, string message)
	{
		if (result.Failed)
		{
			throw new InvalidOperationException(message);
		}
	}
}
