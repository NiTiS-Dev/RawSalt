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

		window.Resize += Resize;
		window.FileDrop += FileDrop;
		window.Update += Update;
		window.Load += Initialize;
		window.Closing += Closing;
		window.Render += GraphicUpdate;


		window.Initialize();
		gl = window.CreateOpenGL();

		Resize(window.Size);
	}
	protected void Launch()
	{
		window.Run();
	}
	public virtual void FileDrop(string[] filePaths)
	{

	}
	public virtual void Resize(vec2i newSize)
	{

	}
	public virtual void Closing()
	{

	}
	public virtual void Initialize()
	{

	}
	public virtual void GraphicUpdate(double delta)
	{

	}
	public virtual void Update(double delta)
	{

	}
}
