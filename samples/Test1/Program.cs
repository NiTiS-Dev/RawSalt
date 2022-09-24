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
		IWindow window = Window.Create(new WindowOptions()
		{
			API = new GraphicsAPI()
			{
				API = ContextAPI.OpenGL,
				Version = new APIVersion(3, 3),
				Profile = ContextProfile.Core,
				Flags = ContextFlags.Default,
			},
			Title = "Test1",
		});

		PlatformType type = new(Side.Server, Platform.Windows);

		window.Run();
	}
}
