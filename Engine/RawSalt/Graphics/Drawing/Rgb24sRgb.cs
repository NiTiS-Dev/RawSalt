using System.Runtime.InteropServices;

namespace RawSalt.Graphics.Drawing;

/// <summary>
/// Represents 24-bit sRGB pixel format.
/// Each element takes 8 bits of unsigned integer.
/// Does not support transparency.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct Rgb24sRgb : IPixelFormat<Rgb24sRgb>, IEquatable<Rgb24sRgb>
{
	public static PixelFormat Format => PixelFormat.Rgb24Srgb;
	public static int ByteWidth => 3;

	public byte Red, Green, Blue;

	public Rgb24sRgb(byte red, byte green, byte blue)
	{
		Red = red;
		Green = green;
		Blue = blue;
	}

	public static Rgb24sRgb FromColor(Color4 color)
	{
		Rgb24sRgb rgb = default;
		rgb.Red = Srgb.ToByte(color.Red);
		rgb.Green = Srgb.ToByte(color.Green);
		rgb.Blue = Srgb.ToByte(color.Blue);
		return rgb;
	}

	public Rgb24sRgb InvertColors()
	{
		return new Rgb24sRgb(
			(byte)(byte.MaxValue - Red),
			(byte)(byte.MaxValue - Green),
			(byte)(byte.MaxValue - Blue)
		);
	}

	public Color4 ToColor()
	{
		return new Color4(
			Srgb.FromByte(Red),
			Srgb.FromByte(Green),
			Srgb.FromByte(Blue),
			1f
		);
	}

	public bool Equals(Rgb24sRgb other)
	{
		return Red == other.Red && Green == other.Green && Blue == other.Blue;
	}

	public override bool Equals(object? obj)
	{
		return obj is Rgb24sRgb other && Equals(other);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Red, Green, Blue);
	}

	public static bool operator ==(Rgb24sRgb left, Rgb24sRgb right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(Rgb24sRgb left, Rgb24sRgb right)
	{
		return !left.Equals(right);
	}
}
