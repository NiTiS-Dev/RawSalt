namespace RawSalt.Graphics;

public abstract class Adapter
{
	public abstract DeviceLimits Limits { get; }
	public abstract string Name { get; }
}