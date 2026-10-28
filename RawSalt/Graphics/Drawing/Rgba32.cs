using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace RawSalt.Graphics.Drawing;

/// <summary>
/// Represents 32-bit pixel format.
/// Each element takes 8 bits of unsigned integer.
/// Supports transparency.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct Rgba32 : IPixelFormat<Rgba32>, IEquatable<Rgba32>
{
	public static PixelFormat Format => PixelFormat.Rgba32UNorm;

	public byte Red, Green, Blue, Alpha;

	public Rgba32(byte red, byte green, byte blue, byte alpha)
	{
		Red = red;
		Green = green;
		Blue = blue;
		Alpha = alpha;
	}

	public static Rgba32 FromColor(Color4 color)
	{
		Rgba32 rgba = default;
		rgba.Red = (byte)(color.Red * 255f);
		rgba.Green = (byte)(color.Green * 255f);
		rgba.Blue = (byte)(color.Blue * 255f);
		rgba.Alpha = (byte)(color.Alpha * 255f);
		return rgba;
	}

	public Color4 ToColor()
	{
		return new Color4(Red / 255f, Green / 255f, Blue / 255f, Alpha / 255f);
	}

	public bool Equals(Rgba32 other)
	{
		return Red == other.Red && Green == other.Green && Blue == other.Blue && Alpha == other.Alpha;
	}

	public override bool Equals(object? obj)
	{
		return obj is Rgba32 other && Equals(other);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Red, Green, Blue, Alpha);
	}

	public static bool operator ==(Rgba32 left, Rgba32 right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(Rgba32 left, Rgba32 right)
	{
		return !left.Equals(right);
	}
}