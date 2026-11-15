using RawSalt.Graphics.Drawing;
using RawSalt.Graphics.Resources;
using RawSalt.Threading;

namespace RawSalt.Graphics;

/// <summary>
/// Main-thread safe handle to a three-dimensional GPU texture.
/// </summary>
[ThreadSafety(AllowedThread.Main | AllowedThread.Render)]
public readonly struct Texture3D : IDisposable
{
	public GraphicsDevice? Owner { get; }
	public ResourceHandle Handle { get; }
	public PixelFormat Format { get; }
	public Vec3i Size { get; }

	/// <summary>
	/// Whether the GPU texture exists. A texture created off the render thread is not ready until
	/// the render thread has created it.
	/// </summary>
	public bool IsReady => Owner?.IsTextureReady(Handle) ?? false;

	internal Texture3D(GraphicsDevice owner, ResourceHandle handle, PixelFormat format, Vec3i size)
	{
		Owner = owner;
		Handle = handle;
		Format = format;
		Size = size;
	}

	[ThreadSafety(AllowedThread.Render)]
	public void Dispose()
	{
		Owner?.Destroy(this);
	}
}
