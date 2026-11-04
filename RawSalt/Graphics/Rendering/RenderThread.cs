using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using RawSalt.Graphics.Drawing;
using RawSalt.Logging;
using RawSalt.Platforms;

namespace RawSalt.Graphics.Rendering;

internal sealed class RenderThread
{
	private readonly Thread _thread;
	private readonly FramePacer _pacer;
	private readonly GraphicsBackend _backend;
	private readonly RenderSurface _surface;
	//private readonly Channel<RenderMessage> _messages = Channel.CreateUnbounded<RenderMessage>(new() { SingleReader = true });
	private volatile bool _stop;
	private Exception? _failure;

	public RenderThread(FramePacer pacer, GraphicsBackend backend, RenderSurface surface)
	{
		Debug.Assert(pacer != null);
		Debug.Assert(backend != null);
		Debug.Assert(surface != null);

		_pacer = pacer;
		_backend = backend;
		_surface = surface;
		_thread = new Thread(Run)
		{
			Name = "RawSalt/RenderThread",
			IsBackground = true,
			Priority = ThreadPriority.Highest
		};
	}

	public void Start() => _thread.Start();
	//public void Post(RenderMessage m) => _messages.Writer.TryWrite(m);

	public void Stop()
	{
		_stop = true;
		_thread.Join();

		if (_failure != null)
		{
			throw new AggregateException(_failure);
		}
	}

	private void Run()
	{
		try
		{
			using GraphicsDevice device = _backend.CreateDevice(null, _surface);
			using Swapchain swapchain = device.CreateSwapchain(_surface);
			using Renderer renderer = device.CreateRenderer(swapchain);
			using ShaderModule vs = device.CreateShader(ShaderModuleStage.Vertex, File.ReadAllBytes("triangle.vert.spv"));
			using ShaderModule fs = device.CreateShader(ShaderModuleStage.Fragment, File.ReadAllBytes("triangle.frag.spv"));
			using Pipeline pipeline = device.CreatePipeline(new PipelineDescriptor
			{
				VertexShader = vs,
				FragmentShader = fs,
				VertexLayout = Vertex.Layout,
				ColorFormat = renderer.BackBufferFormat,
				CullMode = CullMode.Back,
				FrontFace = FrontFace.CounterClockWise,
			});

			ReadOnlySpan<Vertex> square =
			[
				new() { X = -0.6f, Y = +0.6f,  R = 1, G = 0, B = 0 }, // tl
				new() { X = -0.6f, Y = -0.6f,  R = 1, G = 1, B = 0 }, // bl
				new() { X = +0.6f, Y = +0.6f,  R = 0, G = 1, B = 1 }, // tr

				new() { X = -0.6f, Y = -0.6f,  R = 1, G = 1, B = 0 }, // bl
				new() { X = +0.6f, Y = -0.6f,  R = 0, G = 1, B = 1 }, // br
				new() { X = +0.6f, Y = +0.6f,  R = 0, G = 1, B = 1 }, // tr
			];
			using Buffer vertices = device.CreateBuffer(BufferUsage.Vertex, square);

			try
			{
				bool firstFrame = true;
				float deg = 0;
				while (!_stop)
				{
					RenderSnapshot? snapshot = _pacer.AcquireForRender(TimeSpan.FromMilliseconds(50));
					if (snapshot == null) continue;

					if (!renderer.TryBeginFrame(out RenderFrame? frame))
					{
						_pacer.Release(snapshot);
						continue;
					}

					CommandList cmd = frame.Commands;
					deg += 0.65f;
					cmd.BeginRenderPass(new RenderPass(frame.BackBuffer, new Color4(0.3f, 0.3f, 0.2f).ShiftHue(deg)));
					{
						cmd.SetPipeline(pipeline);
						cmd.SetVertexBuffer(0, vertices);
						cmd.Draw(6);
					}
					cmd.EndRenderPass();

					renderer.EndFrame(frame);
					_pacer.Release(snapshot);
					if (firstFrame)
					{
						firstFrame = false;
						(_surface.View as Window)?.Show();
					}
				}
			}
			finally
			{
				device.WaitIdle();
			}
		}
		catch (Exception ex)
		{
			_failure = ex;
			Log.Error(ex, "Render thread crashed.");
		}
	}
}

[StructLayout(LayoutKind.Sequential)]
file struct Vertex
{
	public float X, Y, R, G, B;

	public static readonly VertexLayout Layout = new(20,
	[
		new VertexAttribute(0, VertexFormat.Float32x2, 0),
		new VertexAttribute(1, VertexFormat.Float32x3, 8),
	]);
}