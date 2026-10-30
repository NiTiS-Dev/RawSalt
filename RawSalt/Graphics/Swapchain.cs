namespace RawSalt.Graphics;

public abstract class Swapchain : IDisposable
{
	public abstract void Present();

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