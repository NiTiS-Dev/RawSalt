using System.Diagnostics;
using RawSalt.Graphics.Drawing;
using RawSalt.Logging;
using RawSalt.Platforms;

namespace RawSalt.Graphics.Rendering;

internal sealed class RenderThread
{
	private readonly Thread _thread;
	private readonly GraphicsBackend _backend;
	private readonly RenderSurface _surface;
	//private readonly Channel<RenderMessage> _messages = Channel.CreateUnbounded<RenderMessage>(new() { SingleReader = true });
	private volatile bool _stop;
	private Exception? _failure;

	public RenderThread(GraphicsBackend backend, RenderSurface surface)
	{
		Debug.Assert(backend != null);
		Debug.Assert(surface != null);

		_backend = backend;
		_surface = surface;
		_thread = new Thread(Run)
		{
			Name = "RawSalt/RenderThread"
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

			void OnSizeChanged(SizeChangedEventArgs args)
			{
				swapchain.Resize(args.SurfaceSize);
			}
			_surface.View.SizeChanged += OnSizeChanged;

			try
			{
				bool firstFrame = true;
				float deg = 0;
				while (!_stop)
				{
					if (!renderer.TryBeginFrame(out RenderFrame? frame))
					{
						Thread.Sleep(1);
						continue;
					}

					CommandList cmd = frame.Commands;
					deg += 0.15f;
					cmd.BeginRenderPass(new RenderPass(frame.BackBuffer, new Color4(0.3f, 0.3f, 0.2f).ShiftHue(deg)));

					cmd.EndRenderPass();

					renderer.EndFrame(frame);
					if (firstFrame)
					{
						firstFrame = false;
						(_surface.View as Window)?.Show();
					}
				}
			}
			finally
			{
				_surface.View.SizeChanged -= OnSizeChanged;
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