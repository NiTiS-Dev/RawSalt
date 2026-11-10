using NiTiS.Mathematics;

namespace RawSalt.Platforms.Win32;

public class Win32Display : Display
{
	public override string Name { get; }

	public override Region2<int> Region { get; }
	public override Region2<int> WorkRegion { get; }

	internal Win32Display(string name, Region2<int> region, Region2<int> workRegion)
	{
		Name = name;
		Region = region;
		WorkRegion = workRegion;
	}
}