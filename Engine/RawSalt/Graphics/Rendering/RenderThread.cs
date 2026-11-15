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
	private readonly FramePacer _pacer;
	private readonly GraphicsBackend _backend;
	private readonly RenderSurface _surface;
	private readonly Action<GraphicsDevice, Renderer>? _load;
	private readonly Action<GraphicsDevice, RenderFrame, CameraData>? _render;
	private readonly Action? _unload;

	private readonly Channel<Action<GraphicsDevice>> _messages = Channel.CreateUnbounded<Action<GraphicsDevice>>(new() { SingleReader = true });
	private readonly AutoResetEvent _wake = new(false);
	private readonly ManualResetEventSlim _firstFramePresented = new(false);

	private Exception? _failure;
	private volatile GraphicsDevice? _device;
	private readonly CancellationTokenSource _cts;

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

	public RenderThread(FramePacer pacer, GraphicsBackend backend, RenderSurface surface,
		Action<GraphicsDevice, Renderer>? load = null,
		Action<GraphicsDevice, RenderFrame, CameraData>? render = null,
		Action? unload = null,
		CancellationToken cancellationToken = default)
	{
		Debug.Assert(pacer != null);
		Debug.Assert(backend != null);
		Debug.Assert(surface != null);

		_pacer = pacer;
		_backend = backend;
		_surface = surface;
		_load = load;
		_render = render;
		_unload = unload;

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
			using GraphicsDevice device = _backend.CreateDevice(_surface, null);
			_device = device;
			using Swapchain swapchain = device.CreateSwapchain(_surface);
			using Renderer renderer = device.CreateRenderer(swapchain);

			try
			{
				_load?.Invoke(device, renderer);

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

					_render?.Invoke(device, frame, snapshot.Camera);

					renderer.EndFrame(frame);
					_firstFramePresented.Set();
					_pacer.Release(snapshot, cancellationToken);
				}
			}
			finally
			{
				device.WaitIdle();
				_unload?.Invoke();
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