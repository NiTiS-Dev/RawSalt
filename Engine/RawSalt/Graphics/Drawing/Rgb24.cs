using System.Runtime.InteropServices;

namespace RawSalt.Graphics.Drawing;

/// <summary>
/// Represents 24-bit pixel format.
/// Each element takes 8 bits of unsigned integer.
/// Does not support transparency.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct Rgb24 : IPixelFormat<Rgb24>, IEquatable<Rgb24>
{
	public static PixelFormat Format => PixelFormat.Rgb24UNorm;
	public static int ByteWidth => 3;

	public byte Red, Green, Blue;

	public Rgb24(byte red, byte green, byte blue)
	{
		Red = red;
		Green = green;
		Blue = blue;
	}

	public static Rgb24 FromColor(Color4 color)
	{
		Rgb24 rgb = default;
		rgb.Red = (byte)(color.Red * 255f);
		rgb.Green = (byte)(color.Green * 255f);
		rgb.Blue = (byte)(color.Blue * 255f);
		return rgb;
	}

	public Rgb24 InvertColors()
	{
		return new Rgb24(
			(byte)(byte.MaxValue - Red),
			(byte)(byte.MaxValue - Green),
			(byte)(byte.MaxValue - Blue)
		);
	}

	public Color4 ToColor()
	{
		return new Color4(Red / 255f, Green / 255f, Blue / 255f, 1f);
	}

	public bool Equals(Rgb24 other)
	{
		return Red == other.Red && Green == other.Green && Blue == other.Blue;
	}

	public override bool Equals(object? obj)
	{
		return obj is Rgb24 other && Equals(other);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Red, Green, Blue);
	}

	public static bool operator ==(Rgb24 left, Rgb24 right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(Rgb24 left, Rgb24 right)
	{
		return !left.Equals(right);
	}
}
