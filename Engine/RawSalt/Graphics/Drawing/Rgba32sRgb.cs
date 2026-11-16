using System.Runtime.InteropServices;

namespace RawSalt.Graphics.Drawing;

/// <summary>
/// Represents 32-bit sRGB pixel format.
/// Each element takes 8 bits of unsigned integer.
/// Supports transparency.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct Rgba32sRgb : IPixelFormat<Rgba32sRgb>, IEquatable<Rgba32sRgb>
{
	public static PixelFormat Format => PixelFormat.Rgba32Srgb;
	public static int ByteWidth => 4;

	public byte Red, Green, Blue, Alpha;

	public Rgba32sRgb(byte red, byte green, byte blue, byte alpha)
	{
		Red = red;
		Green = green;
		Blue = blue;
		Alpha = alpha;
	}

	public static Rgba32sRgb FromColor(Color4 color)
	{
		Rgba32sRgb rgba = default;
		rgba.Red = Srgb.ToByte(color.Red);
		rgba.Green = Srgb.ToByte(color.Green);
		rgba.Blue = Srgb.ToByte(color.Blue);
		rgba.Alpha = (byte)(color.Alpha * 255f + 0.5f);
		return rgba;
	}

	public Rgba32sRgb InvertColors()
	{
		return new Rgba32sRgb(
			(byte)(byte.MaxValue - Red),
			(byte)(byte.MaxValue - Green),
			(byte)(byte.MaxValue - Blue),
			Alpha
		);
	}

	public Color4 ToColor()
	{
		return new Color4(
			Srgb.FromByte(Red),
			Srgb.FromByte(Green),
			Srgb.FromByte(Blue),
			Alpha / 255f
		);
	}

	public bool Equals(Rgba32sRgb other)
	{
		return Red == other.Red && Green == other.Green && Blue == other.Blue && Alpha == other.Alpha;
	}

	public override bool Equals(object? obj)
	{
		return obj is Rgba32sRgb other && Equals(other);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Red, Green, Blue, Alpha);
	}

	public static bool operator ==(Rgba32sRgb left, Rgba32sRgb right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(Rgba32sRgb left, Rgba32sRgb right)
	{
		return !left.Equals(right);
	}
}
