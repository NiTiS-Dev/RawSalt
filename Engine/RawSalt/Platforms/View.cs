using RawSalt.Threading;

namespace RawSalt.Platforms;

[ThreadSafety(AllowedThread.Main)]
public abstract class View
{
	public abstract Platform Platform { get; }

	public abstract bool ShouldUpdate { get; }

	/// <summary>
	/// Total size of the view in virtual units.
	/// </summary>
	public abstract Vec2i Size { get; }

	/// <summary>
	/// Surface size in physical pixels.
	/// </summary>
	public virtual Vec2i SurfaceSize => Size;

	public virtual void PumpEvents() {}
}