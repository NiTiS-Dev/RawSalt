using RawSalt.App.Desktop;
using Silk.NET.Windowing;

namespace Test1;
public class TestApplication : DesktopApplication
{
	public TestApplication(WindowOptions options) : base(options)
	{
	}
	private static void Main(string[] args)
	{
		TestApplication? app = null;
		try
		{
			app = new TestApplication(new WindowOptions()
			{
				API = new GraphicsAPI(ContextAPI.OpenGL, new APIVersion(3, 2)),
				WindowBorder = WindowBorder.Resizable,
				IsVisible = true,
				FramesPerSecond = 60,
				UpdatesPerSecond = 100,
				Size = new(1020, 700),
				Title = "TestApplication",
				Position = new(200, 200),
			});

			app.mainWindow.Run();
		}
		finally
		{
			app?.Closing();
		}
	}
}
