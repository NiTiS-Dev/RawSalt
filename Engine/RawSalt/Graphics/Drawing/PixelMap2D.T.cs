using System.Buffers;
using System.Diagnostics;
using System.Runtime.InteropServices;
using CommunityToolkit.Diagnostics;
using StbImageNext;
using StbImageNext.Encoders;

namespace RawSalt.Graphics.Drawing;

/// <summary>
/// Mutable pixel map of <typeparamref name="TPixel"/> format.
/// </summary>
/// <typeparam name="TPixel">
/// Pixel format. Use <see cref="Rgb24"/> for basic RGB support,
/// and <see cref="Rgba32"/> for additional transparency.
/// </typeparam>
public class PixelMap2D<TPixel> : PixelMap2D, ICloneable
	where TPixel : unmanaged, IPixelFormat<TPixel>
{
	/// <inheritdoc/>
	public sealed override PixelFormat Format => TPixel.Format;
	/// <inheritdoc/>
	public sealed override int Width { get; }
	/// <inheritdoc/>
	public sealed override int Height { get; }
	/// <summary>
	/// Length of continuous memory in pixels.
	/// </summary>
	public int Length => Width * Height;
	private readonly IMemoryOwner<TPixel> _pixels;

	static PixelMap2D()
	{
		Debug.Assert(TPixel.ByteWidth == TPixel.Format.BytesPerPixel);
	}

	/// <summary>
	/// Initialize pixel map of provided size.
	/// </summary>
	/// <param name="width">Width of pixel map in pixels.</param>
	/// <param name="height">Height of pixel map in pixels.</param>
	/// <exception cref="ArgumentOutOfRangeException">Provided width or height is zero or negative.</exception>
	public PixelMap2D(int width, int height)
	{
		Width = width;
		Height = height;
		Guard.IsGreaterThan(width, 0);
		Guard.IsGreaterThan(height, 0);

		_pixels = MemoryPool<TPixel>.Shared.Rent(Length);
	}

	private PixelMap2D(PixelMap2D<TPixel> original)
	{
		Width = original.Width;
		Height = original.Height;

		_pixels = MemoryPool<TPixel>.Shared.Rent(Length);
		original._pixels.Memory[..Length].CopyTo(_pixels.Memory);
		Debug.Assert(original._pixels != _pixels);
	}

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			_pixels.Dispose();
		}
	}

	/// <inheritdoc cref="ICloneable.Clone" />
	public PixelMap2D<TPixel> Clone()
	{
		ThrowIfDisposed();
		return new PixelMap2D<TPixel>(original: this);
	}

	object ICloneable.Clone() => Clone();

	public Span<TPixel> GetRow(int row)
	{
		ThrowIfDisposed();
		ArgumentOutOfRangeException.ThrowIfNegative(row);
		ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(row, Height);

		int index = row * Width;
		return _pixels.Memory.Span[index..(index + Width)];
	}

	/// <summary>
	/// Get reference to pixel at provided location.
	/// </summary>
	/// <param name="x">Pixel column.</param>
	/// <param name="y">Pixel row.</param>
	/// <returns>Reference to the pixel.</returns>
	public ref TPixel Get(int x, int y)
	{
		return ref GetRow(y)[x];
	}

	/// <summary>
	/// Get pixel value at provided location.
	/// </summary>
	/// <param name="x">Pixel column.</param>
	/// <param name="y">Pixel row.</param>
	/// <param name="onDefault">Return value, if <paramref name="x"/> or <paramref name="y"/> is out of bounds.</param>
	/// <returns>Pixel value.</returns>
	public TPixel GetOrDefault(int x, int y, TPixel onDefault = default)
	{
		if (Region.Contains(new Vec2i(x, y)))
		{
			return Get(x, y);
		}

		return onDefault;
	}

	/// <summary>
	/// Set pixel at provided location.
	/// </summary>
	/// <param name="x">Pixel column.</param>
	/// <param name="y">Pixel row.</param>
	/// <param name="pixel">Pixel new value.</param>
	public void Set(int x, int y, TPixel pixel)
	{
		Get(x, y) = pixel;
	}

	public override void FlipX()
	{
		ThrowIfDisposed();
		for (int y = 0; y < Height; y++)
		{
			Span<TPixel> row = GetRow(y);
			row.Reverse();
		}
	}

	public override void FlipY()
	{
		ThrowIfDisposed();
		Span<TPixel> tmp = stackalloc TPixel[Width];
		for (int y = 0; y < Height / 2; y++)
		{
			Span<TPixel> src = GetRow(y);
			Span<TPixel> dst = GetRow(Height - 1 - y);
			src.CopyTo(tmp);
			dst.CopyTo(src);
			tmp.CopyTo(dst);
		}
	}

	public override void Invert()
	{
		ThrowIfDisposed();
		for (int i = 0; i < Length; i++)
		{
			_pixels.Memory.Span[i] = _pixels.Memory.Span[i].InvertColors();
		}
	}

	public override ReadOnlySpan<byte> GetBytes()
	{
		ThrowIfDisposed();
		return MemoryMarshal.AsBytes(_pixels.Memory.Span[..Length]);
	}

	public override void Fill(byte[] source)
	{
		ThrowIfDisposed();
		Guard.IsNotNull(source);
		if (source.Length != Length)
		{
			ThrowHelper.ThrowArgumentException(nameof(source), "Source array is not same size of pixel map.");
		}

		source.CopyTo(MemoryMarshal.Cast<TPixel, byte>(_pixels.Memory.Span));
	}

	public override void Fill(ReadOnlySpan<byte> source)
	{
		ThrowIfDisposed();
		if (source.Length != Length * TPixel.ByteWidth)
		{
			ThrowHelper.ThrowArgumentException(nameof(source), "Source span is not same size of pixel map.");
		}

		source.CopyTo(MemoryMarshal.Cast<TPixel, byte>(_pixels.Memory.Span));
	}

	/// <inheritdoc/>
	public override void Fill(Color4 color)
	{
		Fill(TPixel.FromColor(color));
	}

	/// <inheritdoc cref="Fill(Color4)"/>
	public void Fill(TPixel color)
	{
		ThrowIfDisposed();
		_pixels.Memory.Span[..(Width * Height)].Fill(color);
	}

	/// <inheritdoc/>
	public override void Outline(Color4 outline, int radius, Predicate<Color4> threshold)
	{
		Outline(TPixel.FromColor(outline), radius, pixel => threshold(pixel.ToColor()));
	}

	/// <inheritdoc cref="Outline(Color4, int, Predicate{Color4})"/>
	public void Outline(TPixel outline, int radius, Predicate<TPixel> threshold)
	{
		ThrowIfDisposed();
		using PixelMap2D<TPixel> original = Clone();

		for (int y = 0; y < Height; y++)
		{
			next:
			for (int x = 0; x < Width; x++)
			{
				if (!threshold(original.Get(x, y))) continue;

				for (int dX = -radius; dX <= radius; dX++)
				{
					for (int dY = -radius; dY <= radius; dY++)
					{
						if (dX * dX + dY * dY > radius * radius) continue;

						if (!threshold(original.GetOrDefault(x + dX, y + dY)))
						{
							Set(x, y, outline);
							continue next;
						}
					}
				}
			}
		}
	}

	/* Not tested yet.
	public void AlphaBleed(int radius, Predicate<TPixel> threshold)
	{
		ThrowIfDisposed();
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(radius);

		using PixelMap2D<TPixel> original = Clone();

		for (int y = 0; y < Height; y++)
		{
			for (int x = 0; x < Width; x++)
			{
				TPixel current = original.Get(x, y);

				if (current.ToColor().Alpha > 0) continue;

				float bestDistance = float.MaxValue;
				TPixel bestPixel = default;

				for (int dX = -radius; dX <= radius; dX++)
				{
					for (int dY = -radius; dY <= radius; dY++)
					{
						int distanceSquared = dX * dX + dY * dY;

						if (distanceSquared > radius * radius || distanceSquared >= bestDistance) continue;

						TPixel neighbor = original.GetOrDefault(x + dX, y + dY);

						if (neighbor.ToColor().Alpha == 0) continue;

						bestDistance = distanceSquared;
						bestPixel = neighbor;
					}
				}

				if (bestDistance != float.MaxValue)
				{
					Color4 color = bestPixel.ToColor();
					color.Alpha = 0f;

					Set(x, y, TPixel.FromColor(color));
				}
			}
		}
	}
	*/

	public override void Save(Stream stream, ImageFormat format)
	{
		byte[] data = [.. MemoryMarshal.Cast<TPixel, byte>(_pixels.Memory.Span)];
		switch (format)
		{
			case ImageFormat.Png:
			{
				// TODO: Need redecode UNorm into sRGB format?
				if (Format is PixelFormat.Rgba32UNorm or PixelFormat.Rgba32Srgb)
				{
					PngEncoder.EncodeToStream(stream, data, Width, Height, ColorComponents.RedGreenBlueAlpha);
				}
				else if (Format is PixelFormat.Rgb24UNorm or PixelFormat.Rgb24Srgb)
				{
					PngEncoder.EncodeToStream(stream, data, Width, Height, ColorComponents.RedGreenBlue);
				}
				else
				{
					throw new PixelFormatNotSupportedException(Format);
				}
			} break;
			default:
				throw new NotImplementedException();
		}
	}
}