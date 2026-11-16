using System.Runtime.InteropServices;
using RawSalt.Graphics.Devices;
using RawSalt.Graphics.Rendering;
using RawSalt.Platforms;
using RawSalt.Threading;

namespace RawSalt.Graphics;

[ThreadSafety(AllowedThread.Main | AllowedThread.Render)]
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

	/// <summary>
	/// Creates logical graphics device.
	/// </summary>
	/// <param name="renderSurface">Target render surface.</param>
	/// <param name="adapter">Preferred adapter device.</param>
	/// <returns></returns>
	[ThreadSafety(AllowedThread.Render)]
	public abstract GraphicsDevice CreateDevice(RenderSurface renderSurface, [Optional] Adapter? adapter);

	[ThreadSafety(AllowedThread.Main)]
	public abstract RenderSurface CreateSurface(View view);
}