using System.Numerics;

namespace RawSalt.Graphics.Drawing;

public interface IPixelFormat<TSelf>
	where TSelf : IPixelFormat<TSelf>
{
	static abstract PixelFormat Format { get; }

	static abstract int ByteWidth { get; }

	static abstract TSelf FromColor(Color4 color);

	/// <summary>
	/// Invert color attributes of pixel. Do not touch alpha channel.
	/// </summary>
	/// <returns></returns>
	TSelf InvertColors();

	Color4 ToColor();
}