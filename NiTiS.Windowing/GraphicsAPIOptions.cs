namespace NiTiS.Windowing;

public record struct GraphicsAPIOptions
	(
		ContextAPIType API,
		ContextAPIFlags Flags,
		ContextAPIProfile Profile,
		ContextAPIVersion Version
	)
{
	public static GraphicsAPIOptions DefaultOpenGL => new(ContextAPIType.OpenGL, ContextAPIFlags.ForwardCompatible, ContextAPIProfile.Compatability, new(3, 3));
	public static GraphicsAPIOptions None => default;
}