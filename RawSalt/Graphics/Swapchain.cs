namespace RawSalt.Graphics;

public abstract class Swapchain : IDisposable
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

	protected abstract void Dispose(bool disposing);

	public void Dispose()
	{
		GC.SuppressFinalize(this);
		Dispose(true);
	}

	~Swapchain()
	{
		Dispose(false);
	}
}