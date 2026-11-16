using RawSalt.Threading;

namespace RawSalt.Graphics.Resources;

/// <summary>
/// Dedicated GPU memory buffer. Render thread only; users hold a <see cref="Buffer"/> handle instead.
/// </summary>
[ThreadSafety(AllowedThread.Render)]
public abstract class BufferResource : Disposable
{
	/// <summary>
	/// Usable size of buffer.
	/// </summary>
	public abstract ulong Size { get; }
}