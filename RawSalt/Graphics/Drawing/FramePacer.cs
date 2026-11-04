using System.Collections.Concurrent;

namespace RawSalt.Graphics.Drawing;

internal sealed class FramePacer
{
	private readonly BlockingCollection<RenderSnapshot> _free;
	private readonly BlockingCollection<RenderSnapshot> _ready;
	private readonly CancellationToken _stoppingToken;

	public FramePacer(int count = 2, CancellationToken stoppingToken = default)
	{
		_stoppingToken = stoppingToken;
		_free = new BlockingCollection<RenderSnapshot>(new ConcurrentQueue<RenderSnapshot>(), count);
		_ready = new BlockingCollection<RenderSnapshot>(new ConcurrentQueue<RenderSnapshot>(), count);

		for (int i = 0; i < count; i++)
		{
			_free.Add(new RenderSnapshot());
		}
		_stoppingToken.ThrowIfCancellationRequested();
	}

	public RenderSnapshot? AcquireForUpdate(TimeSpan timeout)
	{
		try
		{
			return _free.TryTake(out var s, timeout.Milliseconds, _stoppingToken) ? s : null;
		}
		catch (OperationCanceledException)
		{
			return null;
		}
	}

	public void Submit(RenderSnapshot s)
	{
		_ready.Add(s, _stoppingToken);
	}

	public RenderSnapshot? AcquireForRender(TimeSpan timeout)
	{
		try
		{
			return _ready.TryTake(out var s, timeout.Milliseconds, _stoppingToken) ? s : null;
		}
		catch (OperationCanceledException)
		{
			return null;
		}
	}

	public void Release(RenderSnapshot s)
	{
		s.Reset();
		_free.Add(s, _stoppingToken);
	}
}