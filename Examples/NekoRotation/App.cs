using System.Numerics;
using System.Runtime.InteropServices;
using NiTiS.Mathematics;
using RawSalt;
using RawSalt.Graphics;
using RawSalt.Graphics.Drawing;
using RawSalt.Graphics.Pipelines;
using RawSalt.Graphics.Rendering;
using RawSalt.Graphics.Vulkan;
using RawSalt.Logging;
using RawSalt.Platforms.Win32;
using RawSalt.Scenes;
using Buffer = RawSalt.Graphics.Buffer;

namespace NekoRotation;

internal sealed class App : Application
{
	private Camera _camera = new(CameraProjection.Perspective)
	{
		Position = new Vector3(0, 0, 2f)
	};

	private Pipeline? _pipeline;
	private Pipeline? _skyboxPipeline;
	private Texture2D _texture;
	private TextureCube _skybox;
	private Buffer _skyboxVertices;
	private Buffer _vertices;
	private Buffer _indexes;
	private uint _indexCount;
	private float _pos;

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
	    _pos += deltaTime;

	    float x = float.Cos(_pos * float.Pi);
	    float z = float.Sin(_pos * float.Pi);

	    _camera.Position = new Vector3(2f * x, x, 2f * z);
	    _camera.LookAt(Vector3.Zero);
    }

    protected override void Draw(ref CameraData camera, Vector2<int> viewport, ulong updateId, float deltaTime)
    {
	    _camera.SetViewport(viewport.X, viewport.Y);
	    camera.Projection = _camera.Projection;
	    camera.View = _camera.View;
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
	    _skybox = device.CreateTextureCube(face, face, cross.Format, Sampling.ClampToEdge, CubeFaces(cross));

	    ReadOnlySpan<Vector2> skyVertices = [new(-1f, -1f), new(3f, -1f), new(-1f, 3f)];
	    _skyboxVertices = device.CreateBuffer(BufferUsage.Vertex, skyVertices);

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
	    _vertices = device.CreateBuffer(BufferUsage.Vertex, cube);

	    ReadOnlySpan<uint> indexes =
	    [
		    0, 1, 2, 2, 1, 3,
		    4, 5, 6, 6, 5, 7,
		    8, 9, 10, 10, 9, 11,
		    12, 13, 14, 14, 13, 15,
		    16, 17, 18, 18, 17, 19,
		    20, 21, 22, 22, 21, 23
	    ];
	    _indexes = device.CreateBuffer(BufferUsage.Index, indexes);
	    _indexCount = (uint)indexes.Length;
    }

    protected override void Render(GraphicsDevice device, RenderFrame frame, in CameraData camera)
    {
	    if (_pipeline == null)
	    {
		    return;
	    }

	    CommandList cmd = frame.Commands;

	    Matrix4x4 view = camera.View;
	    view.Translation = Vector3.Zero;
	    Matrix4x4.Invert(view * camera.Projection, out Matrix4x4 inverseViewProjection);
	    SkyboxData sky = new() { InverseViewProjection = inverseViewProjection };

	    cmd.BeginRenderPass(new RenderPass(frame.BackBuffer));
	    {
		    cmd.SetPipeline(_pipeline);
		    cmd.SetUniform("camera", camera);
		    cmd.SetUniform("draw", Matrix4x4.CreateScale(0.5f));
		    cmd.SetTexture("tex", _texture);
		    cmd.SetVertexBuffer(0, _vertices);
		    cmd.SetIndexBuffer(_indexes);
		    cmd.DrawIndexed(_indexCount);

		    cmd.SetPipeline(_skyboxPipeline!);
		    cmd.SetUniform("sky", sky);
		    cmd.SetTexture("skybox", _skybox);
		    cmd.SetVertexBuffer(0, _skyboxVertices);
		    cmd.Draw(3);
	    }
	    cmd.EndRenderPass();
    }

    protected override void UnloadGraphics()
    {
	    _pipeline?.Dispose();
	    _skyboxPipeline?.Dispose();
	    _texture.Dispose();
	    _skybox.Dispose();
	    _skyboxVertices.Dispose();
	    _vertices.Dispose();
	    _indexes.Dispose();
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
file struct Vertex()
{
	public float X, Y, Z;
	public float R = 1f, G = 1f, B = 1f;
	public float U, V;
}
