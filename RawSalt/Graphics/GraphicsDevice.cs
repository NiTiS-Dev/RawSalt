using RawSalt.Platforms;

namespace RawSalt.Graphics;

public abstract class GraphicsDevice : IDisposable
{
	public abstract GraphicsSwapchain CreateSwapchain(View view);

	public abstract void WaitIdle();

	protected abstract void Dispose(bool disposing);

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	~GraphicsDevice()
	{
		Dispose(false);
	}
}