namespace RawSalt.Graphics.Drawing;

public interface IPixelFormat<TSelf>
	where TSelf : IPixelFormat<TSelf>
{
	static abstract PixelFormat Format { get; }

	static abstract TSelf FromColor(Color4 color);

	Color4 ToColor();
}