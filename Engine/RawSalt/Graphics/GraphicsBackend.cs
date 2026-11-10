using RawSalt.Graphics.Rendering;
using RawSalt.Platforms;

namespace RawSalt.Graphics;

public abstract class GraphicsBackend : Disposable
{
	public virtual WellKnownGraphicsApi Api => WellKnownGraphicsApi.Unknown;
	public Application Application { get; }

	protected GraphicsBackend(Application application)
	{
		Application = application;
	}

	/// <summary>
	/// Returns list of available adapters.
	/// </summary>
	/// <returns>Readonly list of available adapters, or <see langword="null"/> if adapter enumeration is not supported.</returns>
	public abstract IReadOnlyList<Adapter>? GetAdapters();

	public abstract GraphicsDevice CreateDevice(Adapter? device, RenderSurface renderSurface);

	public abstract RenderSurface CreateSurface(View view);
}