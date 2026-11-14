using System.Runtime.CompilerServices;
using RawSalt.Graphics.Drawing;
using RawSalt.Graphics.Resources;
using RawSalt.Threading;

namespace RawSalt.Graphics;

/// <summary>
/// Main-thread safe handle to a GPU texture. GPU work happens on the render thread.
/// </summary>
[ThreadSafety(AllowedThread.Main | AllowedThread.Render)]
public readonly struct Texture2D : IDisposable
{
	public GraphicsDevice? Owner { get; }
	public ResourceHandle Handle { get; }
	public PixelFormat Format { get; }
	public Vec2i Size { get; }

	internal Texture2D(GraphicsDevice owner, ResourceHandle handle, PixelFormat format, Vec2i size)
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

	public Texture2DRegion AsRegion()
	{
		return new Texture2DRegion(this, Vec2i.Zero, Size);
	}
}