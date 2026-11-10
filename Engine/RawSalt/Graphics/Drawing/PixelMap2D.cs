using NiTiS.Mathematics;
using StbImageNext;

namespace RawSalt.Graphics.Drawing;

public abstract class PixelMap2D : Disposable
{
	public abstract PixelFormat Format { get; }
	public abstract int Width { get; }
	public abstract int Height { get; }
	public Region2<int> Region => new(Vec2i.Zero, new(Width, Height));

	public abstract void FlipX();
	public abstract void FlipY();

	public abstract void Invert();

	/// <summary>
	/// Returns the raw pixel bytes of the whole map, row by row.
	/// </summary>
	public abstract ReadOnlySpan<byte> GetBytes();

	public virtual void Fill(byte[] source)
	{
		Fill(source.AsSpan());
	}

	public abstract void Fill(ReadOnlySpan<byte> source);

	/// <summary>
	/// Fills pixel map with provided <paramref name="color"/>.
	/// </summary>
	/// <param name="color">The operation color.</param>
	public abstract void Fill(Color4 color);

	public static PixelMap2D FromImageBytes(byte[] bytes)
	{
		ImageResult image = ImageResult.FromMemory(bytes, ColorComponents.RedGreenBlueAlpha);
		return FromImageResult(image);
	}

	public static PixelMap2D FromImageStream(Stream stream)
	{
		ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
		return FromImageResult(image);
	}

	private static PixelMap2D FromImageResult(ImageResult image)
	{
		int width = image.Width;
		int height = image.Height;
		byte[] pixelData = image.Data;
		// Decoded images are sRGB-encoded by default.
		PixelMap2D<Rgba32sRgb> pixels = new(width, height);
		pixels.Fill(pixelData.AsSpan()[..(width * height * Rgba32sRgb.ByteWidth)]);
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