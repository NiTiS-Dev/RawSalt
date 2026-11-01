using RawSalt.Graphics.Rendering;

namespace RawSalt.Graphics;

public abstract class GraphicsDevice : Disposable
{
	public abstract Swapchain CreateSwapchain(RenderSurface surface);
	public abstract Renderer CreateRenderer(Swapchain swapchain);

	public abstract void WaitIdle();
}