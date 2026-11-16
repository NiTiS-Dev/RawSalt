using RawSalt.Graphics.Drawing;
using RawSalt.Graphics.Resources;
using RawSalt.Threading;

namespace RawSalt.Graphics;

/// <summary>
/// Main-thread safe handle to a one-dimensional GPU texture.
/// </summary>
[ThreadSafety(AllowedThread.Main | AllowedThread.Render)]
public readonly struct Texture1D : IDisposable
{
	public GraphicsDevice? Owner { get; }
	public ResourceHandle Handle { get; }
	public PixelFormat Format { get; }
	public int Width { get; }

	/// <summary>
	/// Whether the GPU texture exists. A texture created off the render thread is not ready until
	/// the render thread has created it.
	/// </summary>
	public bool IsReady => Owner?.IsTextureReady(Handle) ?? false;

	internal Texture1D(GraphicsDevice owner, ResourceHandle handle, PixelFormat format, int width)
	{
		Owner = owner;
		Handle = handle;
		Format = format;
		Width = width;
	}

	[ThreadSafety(AllowedThread.Render)]
	public void Dispose()
	{
		Owner?.Destroy(this);
	}
}
