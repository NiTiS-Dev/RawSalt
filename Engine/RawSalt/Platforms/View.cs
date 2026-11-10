namespace RawSalt.Platforms;

public abstract class View
{
	public abstract Platform Platform { get; }

	public abstract bool ShouldUpdate { get; }

	public abstract event Action<CloseRequest>? CloseRequest;

	/// <summary>
	/// Total size of the view in virtual units.
	/// </summary>
	public abstract Vec2i Size { get; }

	/// <summary>
	/// Surface size in physical pixels.
	/// </summary>
	public virtual Vec2i SurfaceSize => Size;

	public abstract event Action<SizeChangedEventArgs>? SizeChanged;

	public virtual void PumpEvents() {}
}