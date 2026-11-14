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

	[ThreadSafety(AllowedThread.Render)]
	public void Dispose()
	{
		Owner?.Destroy(this);
	}
}
