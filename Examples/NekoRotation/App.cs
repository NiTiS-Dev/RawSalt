using System.Numerics;
using System.Runtime.InteropServices;
using NiTiS.Mathematics;
using RawSalt;
using RawSalt.Graphics;
using RawSalt.Graphics.Drawing;
using RawSalt.Graphics.Pipelines;
using RawSalt.Graphics.Rendering;
using RawSalt.Graphics.Vulkan;
using RawSalt.Input;
using RawSalt.Logging;
using RawSalt.Platforms.Win32;
using RawSalt.Scenes;

namespace NekoRotation;

internal sealed class App : Application
{
	private readonly Camera _camera = new(CameraProjection.Perspective)
	{
		Position = new Vector3(0, 0, 2f)
	};

	private readonly WorldObject _cube = new();
	private readonly WorldObject _skybox = new() { Layer = 1 };

	private Pipeline? _pipeline;
	private Pipeline? _skyboxPipeline;
	private Texture2D? _texture;
	private TextureCube? _skyboxTexture;
	private StaticMesh<Vertex>? _cubeMesh;
	private StaticMesh<Vector2>? _skyMesh;
	private Material? _cubeMaterial;
	private Material? _skyMaterial;
	private float _pos;

	public App()
	{
		Scene.MainCamera = _camera;
		Scene.Add(_cube);
		Scene.Add(_skybox);
		_cube.Scale = new Vector3(0.5f);
	}

	public static void Main()
	{
		Log.AddSink(new ConsoleSink());

		App app = new();

		var config = LaunchConfiguration.CreateBuilder()
			.AddWin32Platform()
			.AddVulkanBackend()
			.Build();

		bool success = app.Launch(config);

		Environment.Exit(success ? 0 : -1);
	}

	protected override void Update(ulong updateId, float deltaTime)
	{
		if (Keyboard.IsDown(Key.A))
		{
			_pos -= deltaTime;
		}
		else if (Keyboard.IsDown(Key.D))
		{
			_pos += deltaTime;
		}

		float x = float.Sin(_pos * float.Pi);
		float z = float.Cos(_pos * float.Pi);

		_camera.Position = new Vector3(2f * x, x, 2f * z);
		_camera.LookAt(Vector3.Zero);
	}

	protected override void LoadGraphics(GraphicsDevice device, Renderer renderer)
	{
		SlangCompiler compiler = new();
		_pipeline = device.CreatePipeline(new PipelineDescriptor
		{
			Anatomy = new PipelineAnatomy(compiler, File.ReadAllText("triangle.slang")),
			ColorFormat = renderer.BackBufferFormat,
			CullMode = CullMode.Back
		});

		using PixelMap2D pixels = PixelMap2D.FromImageBytes(File.ReadAllBytes("test-image.png"));
		_texture = device.CreateTexture(pixels, Sampling.ClampToEdge);

		_skyboxPipeline = device.CreatePipeline(new PipelineDescriptor
		{
			Anatomy = new PipelineAnatomy(compiler, File.ReadAllText("skybox.slang")),
			ColorFormat = renderer.BackBufferFormat,
			CullMode = CullMode.None
		});

		using PixelMap2D cross = PixelMap2D.FromImageBytes(File.ReadAllBytes("test-cubemap-image.png"));
		int face = cross.Width / 4;
		_skyboxTexture = device.CreateTextureCube(face, face, cross.Format, Sampling.ClampToEdge, CubeFaces(cross));

		ReadOnlySpan<Vector2> skyVertices = [new(-1f, -1f), new(3f, -1f), new(-1f, 3f)];
		_skyMesh = new StaticMesh<Vector2>(device, skyVertices, []);

		ReadOnlySpan<Vertex> cube =
		[
			// Front
			new() { X = -1f, Y = +1f, Z = +1f, U = 0, V = 0 },
			new() { X = -1f, Y = -1f, Z = +1f, U = 0, V = 1 },
			new() { X = +1f, Y = +1f, Z = +1f, U = 1, V = 0 },
			new() { X = +1f, Y = -1f, Z = +1f, U = 1, V = 1 },

			// Back
			new() { X = +1f, Y = +1f, Z = -1f, U = 0, V = 0 },
			new() { X = +1f, Y = -1f, Z = -1f, U = 0, V = 1 },
			new() { X = -1f, Y = +1f, Z = -1f, U = 1, V = 0 },
			new() { X = -1f, Y = -1f, Z = -1f, U = 1, V = 1 },

			// Top
			new() { X = -1f, Y = +1f, Z = -1f, U = 0, V = 0 },
			new() { X = -1f, Y = +1f, Z = +1f, U = 0, V = 1 },
			new() { X = +1f, Y = +1f, Z = -1f, U = 1, V = 0 },
			new() { X = +1f, Y = +1f, Z = +1f, U = 1, V = 1 },

			// Bottom
			new() { X = -1f, Y = -1f, Z = +1f, U = 0, V = 0 },
			new() { X = -1f, Y = -1f, Z = -1f, U = 0, V = 1 },
			new() { X = +1f, Y = -1f, Z = +1f, U = 1, V = 0 },
			new() { X = +1f, Y = -1f, Z = -1f, U = 1, V = 1 },

			// Left
			new() { X = -1f, Y = +1f, Z = -1f, U = 0, V = 0 },
			new() { X = -1f, Y = -1f, Z = -1f, U = 0, V = 1 },
			new() { X = -1f, Y = +1f, Z = +1f, U = 1, V = 0 },
			new() { X = -1f, Y = -1f, Z = +1f, U = 1, V = 1 },

			// Right
			new() { X = +1f, Y = +1f, Z = +1f, U = 0, V = 0 },
			new() { X = +1f, Y = -1f, Z = +1f, U = 0, V = 1 },
			new() { X = +1f, Y = +1f, Z = -1f, U = 1, V = 0 },
			new() { X = +1f, Y = -1f, Z = -1f, U = 1, V = 1 },
		];

		ReadOnlySpan<uint> indexes =
		[
			0, 1, 2, 2, 1, 3,
			4, 5, 6, 6, 5, 7,
			8, 9, 10, 10, 9, 11,
			12, 13, 14, 14, 13, 15,
			16, 17, 18, 18, 17, 19,
			20, 21, 22, 22, 21, 23
		];
		_cubeMesh = new StaticMesh<Vertex>(device, cube, indexes);

		_cubeMaterial = new CubeMaterial(_pipeline.Value, _texture.Value);
		_skyMaterial = new SkyMaterial(_skyboxPipeline.Value, _skyboxTexture.Value);

		_cube.Mesh = _cubeMesh;
		_cube.Material = _cubeMaterial;
		_skybox.Mesh = _skyMesh;
		_skybox.Material = _skyMaterial;

		// Sets _graphicsLoaded, so collection only sees objects with complete graphics.
		base.LoadGraphics(device, renderer);
	}

	protected override void UnloadGraphics()
	{
		base.UnloadGraphics();

		_cube.Mesh = null;
		_cube.Material = null;
		_skybox.Mesh = null;
		_skybox.Material = null;

		_cubeMesh?.Dispose();
		_skyMesh?.Dispose();
		_pipeline?.Dispose();
		_skyboxPipeline?.Dispose();
		_texture?.Dispose();
		_skyboxTexture?.Dispose();
	}

	/// <summary>
	/// Splits a 4x3 horizontal cross into the six cube faces, in +X, -X, +Y, -Y, +Z, -Z order.
	/// </summary>
	private static byte[] CubeFaces(PixelMap2D cross)
	{
		int face = cross.Width / 4;
		if (face * 4 != cross.Width || face * 3 != cross.Height)
		{
			throw new ArgumentException("Cubemap image must be a 4x3 cross.", nameof(cross));
		}

		ReadOnlySpan<(int Column, int Row)> layout =
		[
			(2, 1), // +X
			(0, 1), // -X
			(1, 0), // +Y
			(1, 2), // -Y
			(1, 1), // +Z
			(3, 1), // -Z
		];

		int bpp = cross.Format.BytesPerPixel;
		int faceRowBytes = face * bpp;
		ReadOnlySpan<byte> source = cross.GetBytes();
		byte[] faces = new byte[6 * face * face * bpp];

		for (int f = 0; f < layout.Length; f++)
		{
			(int column, int row) = layout[f];
			for (int y = 0; y < face; y++)
			{
				source.Slice(((row * face + y) * cross.Width + column * face) * bpp, faceRowBytes)
					.CopyTo(faces.AsSpan((f * face + y) * faceRowBytes, faceRowBytes));
			}
		}

		return faces;
	}
}

[StructLayout(LayoutKind.Sequential)]
file struct SkyboxData
{
	public Matrix4x4 InverseViewProjection;
}

[StructLayout(LayoutKind.Sequential)]
internal struct Vertex()
{
	public float X, Y, Z;
	public float R = 1f, G = 1f, B = 1f;
	public float U, V;
}

file sealed class CubeMaterial(Pipeline pipeline, Texture2D texture) : Material(pipeline)
{
	public override void Apply(CommandList cmd, in RenderItem item, in CameraData camera)
	{
		cmd.SetPipeline(Pipeline);
		cmd.SetUniform("camera", camera);
		cmd.SetUniform("draw", item.World);
		cmd.SetTexture("tex", texture);
	}
}

file sealed class SkyMaterial(Pipeline pipeline, TextureCube skybox) : Material(pipeline)
{
	public override void Apply(CommandList cmd, in RenderItem item, in CameraData camera)
	{
		Matrix4x4 view = camera.View;
		view.Translation = Vector3.Zero;
		Matrix4x4.Invert(view * camera.Projection, out Matrix4x4 inverseViewProjection);

		cmd.SetPipeline(Pipeline);
		cmd.SetUniform("sky", new SkyboxData { InverseViewProjection = inverseViewProjection });
		cmd.SetTexture("skybox", skybox);
	}
}