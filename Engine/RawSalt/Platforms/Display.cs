using NiTiS.Mathematics;

namespace RawSalt.Platforms;

public abstract class Display
{
	/// <summary>
	/// Human-readable display name.
	/// </summary>
	public abstract string Name { get; }

	/// <summary>
	/// The display rect in virtual space.
	/// </summary>
	public abstract Region2<int> Region { get; }

	/// <summary>
	/// Region of display, that can be used for work.
	/// </summary>
	public virtual Region2<int> WorkRegion => Region;
}