using System.Numerics;

namespace RawSalt.Graphics.Drawing;

public interface IPixelFormat<Self>
	where Self : IPixelFormat<Self>
{
	static abstract PixelFormat Format { get; }

	static abstract int ByteWidth { get; }

	static abstract Self FromColor(Color4 color);

	/// <summary>
	/// Invert color attributes of pixel. Do not touch alpha channel.
	/// </summary>
	/// <returns></returns>
	Self InvertColors();

	Color4 ToColor();
}