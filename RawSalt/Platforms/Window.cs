namespace RawSalt.Platforms;

public abstract class Window : View
{
	// Windows MUST define surface size
	public abstract override Vec2i SurfaceSize { get; }
}