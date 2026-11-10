namespace RawSalt.Graphics.Drawing;

public enum PixelFormat
{
	Unknown = 0,
	Rgba32UNorm,
	Rgba32Srgb,

	Bgra32UNorm,
	Bgra32Srgb,

	Rgb24UNorm,
	Rgb24Srgb,
}

public static class PixelFormatExtensions
{
	public static int GetBytesPerPixel(this PixelFormat format)
	{
		return format switch
		{
			PixelFormat.Rgba32UNorm or PixelFormat.Rgba32Srgb or
				PixelFormat.Bgra32UNorm or PixelFormat.Bgra32Srgb => Rgba32.ByteWidth,
			PixelFormat.Rgb24UNorm or PixelFormat.Rgb24Srgb => Rgb24.ByteWidth,
			_ => throw new PixelFormatNotSupportedException(format),
		};
	}
}