using System.Collections.Immutable;

namespace RawSalt.Graphics.Pipelines;

/// <summary>
/// Result of compiling a <see cref="PipelineAnatomy"/>.
/// </summary>
public sealed record CompiledShader(
	ImmutableArray<CompiledShaderModule> Stages,
	ImmutableArray<ShaderResource> Resources,
	ImmutableArray<ShaderVertexInput> VertexInputs,
	uint VertexStride);