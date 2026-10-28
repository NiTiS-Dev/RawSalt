using System.Buffers;
using CommunityToolkit.Diagnostics;
using NiTiS.Mathematics;

namespace RawSalt.Graphics.Drawing;

public abstract class PixelMap2d : IDisposable
{
	public abstract PixelFormat Format { get; }
	public abstract int Width { get; }
	public abstract int Height { get; }
	public Region2<int> Region => new(Vec2i.Zero, new(Width, Height));

	public virtual void Dispose()
	{
		GC.SuppressFinalize(this);
	}

	//public abstract void FlipX();
	//public abstract void FlipY();

	/// <summary>
	/// Fills pixel map with provided <paramref name="color"/>.
	/// </summary>
	/// <param name="color">The operation color.</param>
	public abstract void Fill(Color4 color);
}

public class PixelMap2d<TPixel> : PixelMap2d
	where TPixel : unmanaged, IPixelFormat<TPixel>
{
	public override PixelFormat Format => TPixel.Format;
	public override int Width { get; }
	public override int Height { get; }
	private readonly IMemoryOwner<TPixel> _pixels;

	/// <summary>
	/// Initialize pixel map of provided size.
	/// </summary>
	/// <param name="width">Width of pixel map in pixels.</param>
	/// <param name="height">Height of pixel map in pixels.</param>
	/// <exception cref="ArgumentOutOfRangeException">Provided width or height is zero or negative.</exception>
	public PixelMap2d(int width, int height)
	{
		Width = width;
		Height = height;
		Guard.IsGreaterThan(width, 0);
		Guard.IsGreaterThan(height, 0);

		_pixels = MemoryPool<TPixel>.Shared.Rent(width * height);
	}

	/// <inheritdoc/>
	public override void Fill(Color4 color)
	{
		Fill(TPixel.FromColor(color));
	}

	/// <inheritdoc cref="Fill(Color4)"/>
	public void Fill(TPixel color)
	{
		_pixels.Memory.Span[..(Width * Height)].Fill(color);
	}

	public override void Dispose()
	{
		base.Dispose();
		_pixels.Dispose();
	}
}