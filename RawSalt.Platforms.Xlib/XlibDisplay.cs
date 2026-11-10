using NiTiS.Mathematics;
using RawSalt.Platforms;

namespace RawSalt.Windowing.Xlib;

public sealed class XlibDisplay : Display
{
	public override string Name { get; }
	public override Region2<int> Region { get; }
	public override Region2<int> WorkRegion { get; }

	internal XlibDisplay(string name, Region2<int> region)
	{
		Name = name;
		Region = region;
		WorkRegion = region;
	}
}
