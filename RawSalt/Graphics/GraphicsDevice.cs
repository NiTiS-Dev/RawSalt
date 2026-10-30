using RawSalt.Platforms;

namespace RawSalt.Graphics;

public abstract class GraphicsDevice : IDisposable
{
	public abstract GraphicsSwapchain CreateSwapchain(View view);

	public abstract void Dispose();
}