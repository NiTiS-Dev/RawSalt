namespace RawSalt.Platforms;

public abstract class View
{
	public abstract bool ShouldUpdate { get; }

	public abstract event Action<CloseRequest>? CloseRequest;

	public virtual void PumpEvents() {}
}