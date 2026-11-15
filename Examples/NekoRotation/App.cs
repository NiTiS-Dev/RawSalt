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
	private Texture2D _texture;
	private Buffer _vertices;
	private Buffer _indexes;
	private uint _indexCount;
	private float _hue;
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
		    CullMode = CullMode.Back,
		    FrontFace = FrontFace.CounterClockWise,
	    });

	    using PixelMap2D pixels = PixelMap2D.FromImageBytes(File.ReadAllBytes("test-image.png"));
	    _texture = device.CreateTexture(pixels, Sampling.ClampToEdge);

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
	    _hue += 0.65f;

	    cmd.BeginRenderPass(new RenderPass(frame.BackBuffer, new Color4(0.3f, 0.3f, 0.2f).ShiftHue(_hue)));
	    {
		    cmd.SetPipeline(_pipeline);
		    cmd.SetUniform("camera", camera);
		    cmd.SetUniform("draw", Matrix4x4.CreateScale(0.5f));
		    cmd.SetTexture("tex", _texture);
		    cmd.SetVertexBuffer(0, _vertices);
		    cmd.SetIndexBuffer(_indexes);
		    cmd.DrawIndexed(_indexCount);
	    }
	    cmd.EndRenderPass();
    }

    protected override void UnloadGraphics()
    {
	    _pipeline?.Dispose();
	    _texture.Dispose();
	    _vertices.Dispose();
	    _indexes.Dispose();
    }
}

[StructLayout(LayoutKind.Sequential)]
file struct Vertex()
{
	public float X, Y, Z;
	public float R = 1f, G = 1f, B = 1f;
	public float U, V;
}
