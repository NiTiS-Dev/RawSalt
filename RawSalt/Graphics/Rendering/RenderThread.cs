using System.Diagnostics;
using System.Net;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using RawSalt.Graphics.Drawing;
using RawSalt.Graphics.Pipelines;
using RawSalt.Logging;

namespace RawSalt.Graphics.Rendering;

// TODO: Clean and rework this clas
internal sealed class RenderThread
{
	private const int SleepMs = 50;

	private readonly Thread _thread;
	private readonly FramePacer _pacer;
	private readonly GraphicsBackend _backend;
	private readonly RenderSurface _surface;

	//private readonly Channel<RenderMessage> _messages = Channel.CreateUnbounded<RenderMessage>(new() { SingleReader = true });
	private readonly AutoResetEvent _wake = new(false);
	private readonly ManualResetEventSlim _firstFramePresented = new(false);

	private Exception? _failure;
	private readonly CancellationTokenSource _cts;

	public bool FirstFramePresented => _firstFramePresented.IsSet;
	public bool IsValid => _failure == null;

	public RenderThread(FramePacer pacer, GraphicsBackend backend, RenderSurface surface, CancellationToken cancellationToken = default)
	{
		Debug.Assert(pacer != null);
		Debug.Assert(backend != null);
		Debug.Assert(surface != null);

		_pacer = pacer;
		_backend = backend;
		_surface = surface;

		// Wrap cancellationToken, so we can cancel operation both from outside and inside the thread.
		_cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

		_thread = new Thread(Run)
		{
			Name = "RawSalt/RenderThread",
			IsBackground = true
		};
	}

	public void Start() => _thread.Start();
	//public void Post(RenderMessage m) => _messages.Writer.TryWrite(m);

	public void Stop()
	{
		_cts.Cancel();
		_wake.Set();
		_thread.Join();
		_cts.Dispose();

		if (_failure != null)
		{
			ExceptionDispatchInfo.Capture(new RenderThreadException(_failure)).Throw();
		}
	}

	private void Run()
	{
		CancellationToken cancellationToken = _cts.Token;
		try
		{
			using GraphicsDevice device = _backend.CreateDevice(null, _surface);
			using Swapchain swapchain = device.CreateSwapchain(_surface);
			using Renderer renderer = device.CreateRenderer(swapchain);
			using ShaderModule vs =
				device.CreateShader(ShaderStages.Vertex, File.ReadAllBytes("triangle.vert.spv"));
			using ShaderModule fs =
				device.CreateShader(ShaderStages.Fragment, File.ReadAllBytes("triangle.frag.spv"));
			using BindGroupLayout bindCameraGroupLayout = device.CreateBindGroupLayout(
				new BindingEntry(0, BindingType.UniformBuffer, ShaderStages.Vertex, (uint)Unsafe.SizeOf<CameraData>())
			);
			using BindGroupLayout bindModelGroupLayout = device.CreateBindGroupLayout(
				new BindingEntry(0, BindingType.UniformBuffer, ShaderStages.Vertex, (uint)Unsafe.SizeOf<Matrix4x4>())
			);
			using Pipeline pipeline = device.CreatePipeline(new PipelineDescriptor
			{
				VertexShader = vs,
				FragmentShader = fs,
				BindGroupLayout = [bindCameraGroupLayout, bindModelGroupLayout],
				VertexLayout = Vertex.Layout,
				ColorFormat = renderer.BackBufferFormat,
				CullMode = CullMode.Back,
				FrontFace = FrontFace.CounterClockWise,
			});

			ReadOnlySpan<Vertex> square =
			[
				new() { X = -0.6f, Y = +0.6f, R = 1, G = 1, B = 1 }, // tl
				new() { X = -0.6f, Y = -0.6f, R = 1, G = 1, B = 0 }, // bl
				new() { X = +0.6f, Y = +0.6f, R = 0, G = 1, B = 1 }, // tr
				new() { X = +0.6f, Y = -0.6f, R = 0, G = 1, B = 1 }, // br
			];
			using Buffer vertices = device.CreateBuffer(BufferUsage.Vertex, square);
			ReadOnlySpan<uint> squareIndexes = [ 0, 1, 2, 2, 1, 3 ];
			using Buffer indexes = device.CreateBuffer(BufferUsage.Index, squareIndexes);


			try
			{
				float deg = 0;
				while (!cancellationToken.IsCancellationRequested)
				{
					RenderSnapshot? snapshot = _pacer.AcquireForRender(TimeSpan.FromMilliseconds(SleepMs));
					if (snapshot == null) continue;

					Vec2i size = snapshot.Size;
					if (size.X == 0 || size.Y == 0)
					{
						_pacer.Release(snapshot, cancellationToken);
						_wake.WaitOne(SleepMs);
						continue;
					}

					if (swapchain.SurfaceSize != size)
					{
						swapchain.Resize(size);
					}

					if (!renderer.TryBeginFrame(out RenderFrame? frame))
					{
						_pacer.Release(snapshot, cancellationToken);
						_wake.WaitOne(SleepMs);
						continue;
					}

					CommandList cmd = frame.Commands;
					deg += 0.65f;
					cmd.BeginRenderPass(new RenderPass(frame.BackBuffer, new Color4(0.3f, 0.3f, 0.2f).ShiftHue(deg)));
					{
						cmd.SetPipeline(pipeline);
						cmd.SetUniform(0, 0, new CameraData { View = Matrix4x4.Identity, Projection = Matrix4x4.Identity });
						cmd.SetUniform(1, 0, Matrix4x4.Identity * Matrix4x4.CreateRotationZ(deg / 10));
						cmd.SetVertexBuffer(0, vertices);
						cmd.SetIndexBuffer(indexes);
						cmd.DrawIndexed(6);
					}
					cmd.EndRenderPass();

					renderer.EndFrame(frame);
					_firstFramePresented.Set();
					_pacer.Release(snapshot, cancellationToken);
				}
			}
			finally
			{
				device.WaitIdle();
			}
		}
		catch (OperationCanceledException)
		{
			Log.Debug("RenderThread is closed.");
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