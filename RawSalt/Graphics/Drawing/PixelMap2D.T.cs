using System.Buffers;
using System.Diagnostics;
using System.Runtime.InteropServices;
using CommunityToolkit.Diagnostics;
using StbImageNext;
using StbImageNext.Encoders;

namespace RawSalt.Graphics.Drawing;

public class PixelMap2D<TPixel> : PixelMap2D
	where TPixel : unmanaged, IPixelFormat<TPixel>
{
	public override PixelFormat Format => TPixel.Format;
	public override int Width { get; }
	public override int Height { get; }
	public int Length => Width * Height;
	private readonly IMemoryOwner<TPixel> _pixels;

	static PixelMap2D()
	{
		Debug.Assert(TPixel.ByteWidth == TPixel.Format.GetBytesPerPixel());
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

	public override void Save(Stream stream, ImageFormat format)
	{
		// TODO: Add support for Rgb24
		byte[] data = [.. MemoryMarshal.Cast<TPixel, byte>(_pixels.Memory.Span)];
		switch (format)
		{
			case ImageFormat.Png:
			{
				if (Format is PixelFormat.Rgba32UNorm or PixelFormat.Rgba32Srgb)
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