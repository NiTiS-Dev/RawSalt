using RawSalt.Core;
using Silk.NET.Windowing;
using System.Diagnostics;

namespace Test1;

public class Program
{
	private static void Main(string[] args)
		=> new Program(args);
	public Program(params string[] args)
	{
		IWindow window = Window.Create(WindowOptions.Default);

		PlatformType type = new(Side.Server, Platform.Windows);

		Console.WriteLine(type.Platform);
		Console.WriteLine(type.Side);

		//window.Run();
	}
}
