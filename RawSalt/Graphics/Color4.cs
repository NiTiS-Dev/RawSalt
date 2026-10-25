using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace RawSalt.Graphics;

[DebuggerDisplay("{ToString(),nq}")]
public struct Color4 : IEquatable<Color4>, IEquatable<Color4?>
{
	public byte Red, Green, Blue, Alpha;

	public Color4(byte gray)
	{
		Red = gray;
		Green = gray;
		Blue = gray;
		Alpha = 255;
	}

	public Color4(byte red, byte green, byte blue)
	{
		Red = red;
		Green = green;
		Blue = blue;
		Alpha = 255;
	}

	public Color4(byte red, byte green, byte blue, byte alpha)
	{
		Red = red;
		Green = green;
		Blue = blue;
		Alpha = alpha;
	}

	public bool Equals(Color4 other)
	{
		return Red == other.Red &&
		       Green == other.Green &&
		       Blue == other.Blue &&
		       Alpha == other.Alpha;
	}

	public bool Equals(Color4? other)
	{
		return other is { } notNull && Equals(notNull);
	}

	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		return obj is Color4 color && Equals(color);
	}

	public override int GetHashCode()
	{
		return Unsafe.BitCast<Color4, int>(this);
	}

	public override string ToString()
	{
		return $"<{Red}, {Green}, {Blue}, {Alpha}>";
	}

	public static bool operator ==(Color4 left, Color4 right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(Color4 left, Color4 right)
	{
		return !(left == right);
	}

	public Vector4 ToNormalized()
	{
		return new Vector4(Red / 255f, Green / 255f, Blue / 255f, Alpha / 255f);
	}
}