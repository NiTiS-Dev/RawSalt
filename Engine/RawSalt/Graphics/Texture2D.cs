using RawSalt.Graphics.Drawing;

namespace RawSalt.Graphics;

public abstract class Texture2D : Disposable
{
	public abstract PixelFormat Format { get; }
	public abstract Vec2i Size { get; }

	/// <summary>
	/// Replaces the whole texture with <paramref name="pixels"/>.
	/// </summary>
	/// <param name="pixels">Tightly packed pixel data matching <see cref="Format"/>.</param>
	public abstract void Update(ReadOnlySpan<byte> pixels);

	/// <summary>
	/// Copies the whole texture into <paramref name="destination"/>.
	/// </summary>
	/// <param name="destination">Tightly packed pixel data matching <see cref="Format"/>.</param>
	public abstract void GetPixels(Span<byte> destination);

	public void Update(PixelMap2D pixels)
	{
		if (pixels.Width != Size.X || pixels.Height != Size.Y || pixels.Format != Format)
		{
			throw new ArgumentException("Pixel map format and size must match the texture.", nameof(pixels));
		}

		Update(pixels.GetBytes());
	}

	public byte[] GetPixels()
	{
		byte[] pixels = new byte[Size.X * Size.Y * Format.GetBytesPerPixel()];
		GetPixels(pixels);
		return pixels;
	}
}
