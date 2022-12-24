using RawSalt.Resources;
using System.Runtime.InteropServices;

namespace RawSalt.Structs;

[StructLayout(LayoutKind.Sequential)]
// TODO?: Replace with Color<T> where T : IPixel
public readonly unsafe struct Color32
{
	/// <summary>
	/// Color element <see cref="R"/>ed <see cref="G"/>reen <see cref="B"/>lue
	/// </summary>
	public readonly byte R, G, B;
	/// <summary>
	/// Alpha element<br/>
	/// 0 - for full transperency<br/>
	/// 255 - for fully visible
	/// </summary>
	public readonly byte A;
	public Color32()
	{
		A = 0xFF;
	}
	public Color32(byte rgb)
	{
		R = rgb;
		G = rgb;
		B = rgb;
		A = 0xFF;
	}
	public Color32(byte r, byte g, byte b)
	{
		R = r;
		G = g;
		B = b;
		A = 0xFF;
	}
	public Color32(byte r, byte g, byte b, byte a)
	{
		R = r;
		G = g;
		B = b;
		A = a;
	}

	public static explicit operator vec4(Color32 color)
		=> new(
			0xFF / (float)color.R,
			0xFF / (float)color.G,
			0xFF / (float)color.B,
			0xFF / (float)color.A
			);
}