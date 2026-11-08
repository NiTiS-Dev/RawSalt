using System.Buffers;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using CommunityToolkit.Diagnostics;
using NiTiS.Mathematics;
using StbImageNext;
using StbImageNext.Encoders;

namespace RawSalt.Graphics.Drawing;

public abstract class PixelMap2d : Disposable
{
	public abstract PixelFormat Format { get; }
	public abstract int Width { get; }
	public abstract int Height { get; }
	public Region2<int> Region => new(Vec2i.Zero, new(Width, Height));

	public abstract void FlipX();
	public abstract void FlipY();

	public abstract void Invert();

	public abstract void Fill(byte[] source);

	/// <summary>
	/// Fills pixel map with provided <paramref name="color"/>.
	/// </summary>
	/// <param name="color">The operation color.</param>
	public abstract void Fill(Color4 color);

	public static PixelMap2d FromImageBytes(byte[] bytes)
	{
		ImageResult image = ImageResult.FromMemory(bytes, ColorComponents.RedGreenBlueAlpha);
		return FromImageResult(image);
	}

	public static PixelMap2d FromImageStream(Stream stream)
	{
		ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
		return FromImageResult(image);
	}

	private static PixelMap2d FromImageResult(ImageResult image)
	{
		int width = image.Width;
		int height = image.Height;
		byte[] pixelData = image.Data;
		PixelMap2d<Rgba32> pixels = new(width, height);
		pixels.Fill(pixelData);
		return pixels;
	}

	/// <summary>
	/// Saves current pixel map as an image into provided <paramref name="stream"/>.
	/// </summary>
	/// <param name="stream">Writing stream to save current image into.</param>
	/// <param name="format">Image format.</param>
	/// <exception cref="NotSupportedException ">Stream does not support writing.</exception>
	/// <exception cref="PixelFormatNotSupportedException">Pixel format is not supported for requested image format.</exception>
	public abstract void Save(Stream stream, ImageFormat format);

	public enum ImageFormat
	{
		Png,
		Jpg,
		Bmp,
	}
}

public class PixelMap2d<TPixel> : PixelMap2d
	where TPixel : unmanaged, IPixelFormat<TPixel>
{
	public override PixelFormat Format => TPixel.Format;
	public override int Width { get; }
	public override int Height { get; }
	public int Length => Width * Height;
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

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			_pixels.Dispose();
		}
	}

	public Span<TPixel> GetRow(int row)
	{
		ThrowIfDisposed();
		Guard.IsGreaterThan(row, 0);
		Guard.IsLessThan(row, Height);

		int index = row * Width;
		return _pixels.Memory.Span[index..(index + Width)];
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

	public override void Save(Stream stream, ImageFormat format)
	{
		// TODO: Add support for Rgb24
		byte[] data = [.. MemoryMarshal.Cast<TPixel, byte>(_pixels.Memory.Span)];
		switch (format)
		{
			case ImageFormat.Png:
			{
				if (Format == PixelFormat.Rgba32UNorm)
				{
					PngEncoder.EncodeToStream(stream, data, Width, Height, ColorComponents.RedGreenBlueAlpha);
				}
				else
				{
					throw new PixelFormatNotSupportedException();
				}
			} break;
			default:
				throw new NotImplementedException();
		}
	}
}