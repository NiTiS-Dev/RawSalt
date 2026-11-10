using System.Diagnostics;

namespace RawSalt.Graphics.Drawing;

/// <summary>
/// Conversion between linear and sRGB-encoded color channels.
/// </summary>
internal static class Srgb
{
	static Srgb()
	{
		// TODO: Move this to the tests
		Debug.Assert(MathF.Abs(ToLinear(FromLinear(0f))) < 1e-6f);
		Debug.Assert(MathF.Abs(ToLinear(FromLinear(0.5f)) - 0.5f) < 1e-4f);
		Debug.Assert(MathF.Abs(ToLinear(FromLinear(1f)) - 1f) < 1e-4f);
		Debug.Assert(ToByte(0f) == 0);
		Debug.Assert(ToByte(1f) == 255);
		Debug.Assert(ToByte(FromByte(128)) == 128);
	}

	public static float ToLinear(float value)
	{
		value = Math.Clamp(value, 0f, 1f);
		return value <= 0.04045f
			? value / 12.92f
			: MathF.Pow((value + 0.055f) / 1.055f, 2.4f);
	}

	public static float FromLinear(float value)
	{
		value = Math.Clamp(value, 0f, 1f);
		return value <= 0.0031308f
			? value * 12.92f
			: 1.055f * MathF.Pow(value, 1f / 2.4f) - 0.055f;
	}

	public static byte ToByte(float linear)
	{
		return (byte)(FromLinear(linear) * 255f + 0.5f);
	}

	public static float FromByte(byte value)
	{
		return ToLinear(value / 255f);
	}
}
