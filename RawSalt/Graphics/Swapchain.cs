namespace RawSalt.Graphics;

public abstract class Swapchain : Disposable
{
	public abstract bool TryAcquire();
	public abstract void Present();

	/// <summary>
	/// Call when the view's framebuffer size changes.
	/// </summary>
	/// <remarks>
	/// Thread-safe. Can be safely called on disposed swapchain.
	/// </remarks>
	public abstract void Resize(Vec2i newSize);
}