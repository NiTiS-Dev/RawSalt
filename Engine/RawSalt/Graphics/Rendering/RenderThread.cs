using System.Diagnostics;
using System.Numerics;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Threading.Channels;
using RawSalt.Graphics.Drawing;
using RawSalt.Graphics.Pipelines;
using RawSalt.Logging;
using RawSalt.Threading;

namespace RawSalt.Graphics.Rendering;

// TODO: Clean and rework this clas
[ThreadSafety(AllowedThread.Main | AllowedThread.Render)]
internal sealed class RenderThread
{
	private const int SleepMs = 50;

	private readonly Thread _thread;
	private readonly FramePacer _pacer;
	private readonly GraphicsBackend _backend;
	private readonly RenderSurface _surface;

	private readonly Channel<Action<GraphicsDevice>> _messages = Channel.CreateUnbounded<Action<GraphicsDevice>>(new() { SingleReader = true });
	private readonly AutoResetEvent _wake = new(false);
	private readonly ManualResetEventSlim _firstFramePresented = new(false);

	private Exception? _failure;
	private readonly CancellationTokenSource _cts;

	[ThreadSafety(AllowedThread.Main)]
	public bool FirstFramePresented => _firstFramePresented.IsSet;
	[ThreadSafety(AllowedThread.Main)]
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

	[ThreadSafety(AllowedThread.Main)]
	public void Start() => _thread.Start();

	[ThreadSafety(AllowedThread.Main)]
	public void Post(Action<GraphicsDevice> action)
	{
		_messages.Writer.TryWrite(action);
	}

	[ThreadSafety(AllowedThread.Main)]
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

	[ThreadSafety(AllowedThread.Render)]
	private void Run()
	{
		CancellationToken cancellationToken = _cts.Token;
		try
		{
			using GraphicsDevice device = _backend.CreateDevice(null, _surface);
			using Swapchain swapchain = device.CreateSwapchain(_surface);
			using Renderer renderer = device.CreateRenderer(swapchain);
			SlangCompiler compiler = new();
			using Pipeline pipeline = device.CreatePipeline(new PipelineDescriptor
			{
				Anatomy = new PipelineAnatomy(compiler, File.ReadAllText("triangle.slang")),
				ColorFormat = renderer.BackBufferFormat,
				CullMode = CullMode.Back,
				FrontFace = FrontFace.CounterClockWise,
			});
			using PixelMap2D catPixelMap = PixelMap2D.FromImageBytes(File.ReadAllBytes("test-image.png"));
			using Texture2D catTexture = device.CreateTexture(new Texture2DDescriptor(catPixelMap.Region.Size, catPixelMap.Format), []);
			catTexture.Update(catPixelMap);

			ReadOnlySpan<Vertex> square =
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
			using Buffer vertices = device.CreateBuffer(BufferUsage.Vertex, square);
			ReadOnlySpan<uint> squareIndexes = [
				0, 1, 2, 2, 1, 3,
				4, 5, 6, 6, 5, 7,
				8, 9, 10, 10, 9, 11,
				12, 13, 14, 14, 13, 15,
				16, 17, 18, 18, 17, 19,
				20, 21, 22, 22, 21, 23
			];
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
					Vector3 cameraPosition = new Vector3(0f, 0f, 2f);
					Vector3 cameraTarget = Vector3.Zero;
					Vector3 upVector = +Vector3.UnitY;
					float aspect = (float)size.X / size.Y;
					float fieldOfView = MathF.PI / 4f;

					Matrix4x4 view = Matrix4x4.CreateLookAt(cameraPosition, cameraTarget, upVector);
					Matrix4x4 projection = Matrix4x4.CreatePerspectiveFieldOfView(fieldOfView, aspect, 0.001f, 100f);
					cmd.BeginRenderPass(new RenderPass(frame.BackBuffer, new Color4(0.3f, 0.3f, 0.2f).ShiftHue(deg)));
					{
						cmd.SetPipeline(pipeline);
						cmd.SetUniform("camera", new CameraData { View = view, Projection = projection });
						cmd.SetUniform("draw", Matrix4x4.CreateScale(0.5f) * Matrix4x4.CreateRotationY(deg / 120) * Matrix4x4.CreateRotationZ(deg / 310));
						cmd.SetTexture("tex", catTexture);
						cmd.SetVertexBuffer(0, vertices);
						cmd.SetIndexBuffer(indexes);
						cmd.DrawIndexed((uint)squareIndexes.Length);
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
file struct Vertex()
{
	public float X, Y, Z;
	public float R = 1f, G = 1f, B = 1f;
	public float U, V;
}