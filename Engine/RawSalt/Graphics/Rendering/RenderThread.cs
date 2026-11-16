using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Threading.Channels;
using RawSalt.Logging;
using RawSalt.Threading;

namespace RawSalt.Graphics.Rendering;

// TODO: Clean and rework this clas
[ThreadSafety(AllowedThread.Main | AllowedThread.Render)]
internal sealed class RenderThread
{
	private const int SleepMs = 50;

	private readonly Thread _thread;
	private readonly Application _application;
	private readonly FramePacer _pacer;
	private readonly Action<GraphicsDevice, RenderFrame, CameraData>? _render;

	private readonly Channel<Action<GraphicsDevice>> _messages = Channel.CreateUnbounded<Action<GraphicsDevice>>(new() { SingleReader = true });
	private readonly AutoResetEvent _wake = new(false);
	private readonly ManualResetEventSlim _firstFramePresented = new(false);

	private Exception? _failure;
	private volatile GraphicsDevice? _device;
	private readonly CancellationTokenSource _cts;

	private RenderSurface RenderSurface => _application.MainSurface!;
	private GraphicsBackend Backend => _application.GraphicsBackend!;

	[ThreadSafety(AllowedThread.Main)]
	public bool FirstFramePresented => _firstFramePresented.IsSet;
	[ThreadSafety(AllowedThread.Main)]
	public bool IsValid => _failure == null;

	/// <summary>
	/// The device created by the render thread, or <see langword="null"/> before it exists and
	/// after the thread stopped.
	/// </summary>
	[ThreadSafety(AllowedThread.Main)]
	public GraphicsDevice? Device => _device;

	public RenderThread(Application application, FramePacer pacer, CancellationToken cancellationToken = default)
	{
		Debug.Assert(pacer != null);
		Debug.Assert(application != null);
		Debug.Assert(application.Platform != null);
		Debug.Assert(application.MainView != null);
		Debug.Assert(application.MainSurface != null);

		_application = application;
		_pacer = pacer;

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
		_device = null;
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
			using GraphicsDevice device = Backend.CreateDevice(RenderSurface, null);
			_device = device;
			using Swapchain swapchain = device.CreateSwapchain(RenderSurface);
			using Renderer renderer = device.CreateRenderer(swapchain);

			try
			{
				_application.LoadGraphics(device, renderer);

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

					_application.Render(device, frame, snapshot.Camera);

					renderer.EndFrame(frame);
					_firstFramePresented.Set();
					_pacer.Release(snapshot, cancellationToken);
				}
			}
			finally
			{
				device.WaitIdle();
				_application.UnloadGraphics();
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