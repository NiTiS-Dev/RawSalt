using System.Diagnostics;
using System.Threading.Channels;
using RawSalt.Logging;
using RawSalt.Platforms;

namespace RawSalt.Graphics.Rendering;

internal sealed class RenderThread
{
	private readonly Thread _thread;
	private readonly GraphicsBackend _backend;
	private readonly View _view;
	//private readonly Channel<RenderMessage> _messages = Channel.CreateUnbounded<RenderMessage>(new() { SingleReader = true });
	private volatile bool _stop;
	private Exception? _failure;

	public RenderThread(GraphicsBackend backend, View view)
	{
		Debug.Assert(backend != null);
		Debug.Assert(view != null);

		_backend = backend;
		_view = view;
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
		if (_failure != null) throw new AggregateException(_failure);
	}

	private void Run()
	{
		try
		{
			using GraphicsDevice device = _backend.CreateDevice(null);
			// using var swapchain = device.CreateSwapchain(_view);
			// var triangle = new TriangleRenderer(device);
			//
			while (!_stop)
			{
			// 	DrainMessages(swapchain);
			// 	if (swapchain.IsZeroSized)
			// 	{
			// 		Thread.Sleep(10);
			// 		continue;
			}
			//
			// 	triangle.Render(swapchain);
			// 	swapchain.Present();
			// }
			//
			// device.WaitIdle();
		}
		catch (Exception ex)
		{
			_failure = ex;
			Log.Error(ex, "Render thread crashed.");
		}
	}
}