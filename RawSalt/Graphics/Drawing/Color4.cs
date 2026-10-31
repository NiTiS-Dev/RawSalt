using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace RawSalt.Graphics.Drawing;

[DebuggerDisplay("{ToString(),nq}")]
public partial struct Color4 : IApproximatelyEquatable<Color4, double>, IEquatable<Color4?>
{
	public float Red, Green, Blue, Alpha;

	public Color4(float gray)
	{
		Red = gray;
		Green = gray;
		Blue = gray;
		Alpha = 1f;
	}

	public Color4(float red, float green, float blue)
	{
		Red = red;
		Green = green;
		Blue = blue;
		Alpha = 1f;
	}

	public Color4(float red, float green, float blue, float alpha)
	{
		Red = red;
		Green = green;
		Blue = blue;
		Alpha = alpha;
	}

	public bool Equals(Color4 other, double tolerance)
	{
		return Math.Abs(Red - other.Red) < tolerance &&
		       Math.Abs(Green - other.Green) < tolerance &&
		       Math.Abs(Blue - other.Blue) < tolerance &&
		       Math.Abs(Alpha - other.Alpha) < tolerance;
	}

	public bool Equals(Color4 other)
	{
		return Red == other.Red &&
		       Green == other.Green &&
		       Blue == other.Blue &&
		       Alpha == other.Alpha;
	}

	public bool Equals([NotNullWhen(true)] Color4? other)
	{
		return other is { } notNull && Equals(notNull);
	}

	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		return obj is Color4 color && Equals(color);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Red, Green, Blue, Alpha);
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
}