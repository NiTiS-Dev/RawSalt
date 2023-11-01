using System.Runtime.InteropServices;

namespace RawSalt.Graphics;

[StructLayout(LayoutKind.Sequential, Size = 4)]
public struct RGBA32
{
	/// <summary>
	/// The red color channel.
	/// </summary>
	public byte R;
	/// <summary>
	/// The green color channel.
	/// </summary>
	public byte G;
	/// <summary>
	/// The blue color channel.
	/// </summary>
	public byte B;
	/// <summary>
	/// The alpha color channel.
	/// </summary>
	public byte A;

	public RGBA32(byte r, byte g, byte b, byte a)
	{
		R = r;
		G = g;
		B = b;
		A = a;
	}
	public RGBA32(byte r, byte g, byte b)
	{
		R = r;
		G = g;
		B = b;
		A = byte.MaxValue;
	}
	public RGBA32(byte rgb)
	{
		R = rgb;
		G = rgb;
		B = rgb;
		A = byte.MaxValue;
	}

	/// <inheritdoc/>
	public readonly override int GetHashCode()
		=> (R << 24) & (G << 16) & (B << 8) & (A << 0);
}