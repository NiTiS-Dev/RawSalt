using RawSalt.Graphics.Drawing;
using RawSalt.Graphics.Resources;
using RawSalt.Threading;

namespace RawSalt.Graphics;

/// <summary>
/// Main-thread safe handle to a cubemap GPU texture.
/// </summary>
[ThreadSafety(AllowedThread.Main | AllowedThread.Render)]
public readonly struct TextureCube : IDisposable
{
	public GraphicsDevice? Owner { get; }
	public ResourceHandle Handle { get; }
	public PixelFormat Format { get; }

	internal TextureCube(GraphicsDevice owner, ResourceHandle handle, PixelFormat format, int size)
	{
		Owner = owner;
		Handle = handle;
		Format = format;
		Size = size;
	}

	/// <summary>
	/// Side length of one square face, in pixels.
	/// </summary>
	public int Size { get; }

	/// <summary>
	/// Whether the GPU texture exists. A texture created off the render thread is not ready until
	/// the render thread has created it.
	/// </summary>
	public bool IsReady => Owner?.IsTextureReady(Handle) ?? false;

	[ThreadSafety(AllowedThread.Render)]
	public void Dispose()
	{
		Owner?.Destroy(this);
	}
}
