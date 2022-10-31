using NiTiS.IO;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Linq;

namespace RawSalt.App.Desktop;

public unsafe class DesktopApplication : Application
{
	protected readonly IWindow mainWindow;
	protected readonly GL gl;
	public DesktopApplication(WindowOptions options)
	{
		mainWindow = Window.Create(options);

		mainWindow.FileDrop += (strs) => { this.FileDropped(strs.Cast<File>().ToArray()); };
		mainWindow.Resize += Resize;
		mainWindow.Update += Update;
		mainWindow.Render += Draw;
		mainWindow.Closing += Closing;
		mainWindow.Load += Initialize;

		mainWindow.Initialize();
		gl = mainWindow.CreateOpenGL();

		Resize(mainWindow.Size);
	}
}
