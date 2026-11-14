using RawSalt.Graphics.Resources;
using RawSalt.Threading;

namespace RawSalt.Graphics;

/// <summary>
/// Main-thread safe handle to a GPU buffer. GPU work happens on the render thread.
/// </summary>
[ThreadSafety(AllowedThread.Main | AllowedThread.Render)]
public readonly struct Buffer : IDisposable
{
	internal GraphicsDevice? Owner { get; }

	internal Buffer(GraphicsDevice owner, ResourceHandle handle, ulong size)
	{
		Owner = owner;
		Handle = handle;
		Size = size;
	}

	public ResourceHandle Handle { get; }
	public ulong Size { get; }

	[ThreadSafety(AllowedThread.Render)]
	public void Dispose() => Owner?.Destroy(this);
}