using RawSalt.Graphics.Rendering;

namespace RawSalt.Graphics;

public abstract class GraphicsDevice : IDisposable
{
	public abstract Swapchain CreateSwapchain(RenderSurface surface);
	public abstract Renderer CreateRenderer(Swapchain swapchain);

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