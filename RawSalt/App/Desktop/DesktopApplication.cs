using NiTiS.IO;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System;
using System.Linq;

namespace RawSalt.App.Desktop;

public unsafe class DesktopApplication : Application
{
	protected readonly IWindow mainWindow;
	protected GL gl;
	public DesktopApplication(WindowOptions options)
	{
		mainWindow = Window.Create(options);

		mainWindow.FileDrop += (strs) => FileDropped(strs.Cast<File>().ToArray());
		mainWindow.Resize += Resize;
		mainWindow.Update += Update;
		mainWindow.Render += Draw;
		mainWindow.Closing += Closing;
		mainWindow.Load += Initialize;

		mainWindow.Initialize();

		Resize(mainWindow.Size);
	}
	public override void Initialize()
	{
		gl = GL.GetApi(mainWindow);
	}
	public override void Run()
	{
		if (mainWindow is null)
			throw new NullReferenceException($"{nameof(DesktopApplication)}.{nameof(mainWindow)} is null");

		mainWindow.Run();
	}
	public override void Closing()
	{
		base.Closing();
	}
}
