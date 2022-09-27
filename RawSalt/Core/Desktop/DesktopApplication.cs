using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace RawSalt.Core.Desktop;


public class DesktopApplication : Application
{
	private protected IWindow window;
	public IWindow Window => window;
	public DesktopApplication(PlatformType platform, WindowOptions options) : base(platform)
	{
		window = Silk.NET.Windowing.Window.Create(options);

		window.Initialize();

		gl = window.CreateOpenGL();
	}
}
