using RawSalt.Threading;

namespace RawSalt.Graphics;

[ThreadSafety(AllowedThread.Render)]
public abstract class Swapchain : Disposable
{
	public abstract bool TryAcquire();
	public abstract void Present();

	public abstract Vec2i SurfaceSize { get; }

	/// <summary>
	/// Call when the view's framebuffer size changes.
	/// </summary>
	/// <remarks>
	/// Thread-safe. Can be safely called on disposed swapchain.
	/// </remarks>
	[ThreadSafety(AllowedThread.Main | AllowedThread.Render)]
	public abstract void Resize(Vec2i newSize);
}