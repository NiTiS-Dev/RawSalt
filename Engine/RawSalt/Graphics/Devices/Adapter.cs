namespace RawSalt.Graphics.Devices;

/// <summary>
/// Physical graphics card.
/// </summary>
public abstract class Adapter
{
	/// <summary>
	/// Adapter device graphics capability.
	/// </summary>
	public abstract DeviceLimits Limits { get; }

	/// <summary>
	/// User-friendly adapter name.
	/// </summary>
	public abstract string Name { get; }

	/// <inheritdoc/>
	public override string ToString()
	{
		return Name;
	}
}