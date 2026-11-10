using System.Collections.Concurrent;
using RawSalt.Threading;

namespace RawSalt.Graphics.Drawing;

[ThreadSafety(AllowedThread.Main | AllowedThread.Render)]
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

	[ThreadSafety(AllowedThread.Main)]
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

	[ThreadSafety(AllowedThread.Main)]
	public void Submit(RenderSnapshot s, CancellationToken cancellationToken = default)
	{
		_ready.Add(s, cancellationToken);
	}

	[ThreadSafety(AllowedThread.Render)]
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

	[ThreadSafety(AllowedThread.Render)]
	public void Release(RenderSnapshot s, CancellationToken cancellationToken = default)
	{
		s.Reset();
		_free.Add(s, cancellationToken);
	}
}