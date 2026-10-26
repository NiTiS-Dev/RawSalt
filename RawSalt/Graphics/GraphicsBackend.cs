namespace RawSalt.Graphics;

public abstract class GraphicsBackend
{
	public virtual WellKnownGraphicsApi Api => WellKnownGraphicsApi.Unknown;

	public abstract IReadOnlyList<Adapter> GetAdapters();

	public abstract GraphicsContext CreateContext(Adapter device);
}