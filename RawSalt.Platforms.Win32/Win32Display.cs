using NiTiS.Mathematics;
using RawSalt.Platforms;

namespace RawSalt.Windowing.Win32;

public class Win32Display : Display
{
	public override string Name { get; }

	internal Win32Display(string name, Region2<int> region)
	{
		Name = name;
	}
}