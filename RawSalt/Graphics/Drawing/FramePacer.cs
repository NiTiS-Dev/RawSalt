using System.Collections.Concurrent;

namespace RawSalt.Graphics.Drawing;

internal sealed class FramePacer
{
	private readonly BlockingCollection<RenderSnapshot> _free;
	private readonly BlockingCollection<RenderSnapshot> _ready;

	public FramePacer(int count = 2)
	{
		_free = new BlockingCollection<RenderSnapshot>(new ConcurrentQueue<RenderSnapshot>(), count);
		_ready = new BlockingCollection<RenderSnapshot>(new ConcurrentQueue<RenderSnapshot>(), count);

		for (int i = 0; i < count; i++)
		{
			_free.Add(new RenderSnapshot());
		}
	}

	public RenderSnapshot? AcquireForUpdate(TimeSpan timeout, CancellationToken cancellationToken = default)
	{
		try
		{
			return _free.TryTake(out var s, timeout.Milliseconds, cancellationToken) ? s : null;
		}
		catch (OperationCanceledException)
		{
			return null;
		}
	}

	public void Submit(RenderSnapshot s, CancellationToken cancellationToken = default)
	{
		_ready.Add(s, cancellationToken);
	}

	public RenderSnapshot? AcquireForRender(TimeSpan timeout, CancellationToken cancellationToken = default)
	{
		try
		{
			return _ready.TryTake(out var s, timeout.Milliseconds, cancellationToken) ? s : null;
		}
		catch (OperationCanceledException)
		{
			return null;
		}
	}

	public void Release(RenderSnapshot s, CancellationToken cancellationToken = default)
	{
		s.Reset();
		_free.Add(s, cancellationToken);
	}
}