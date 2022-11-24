using NiTiS.IO;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System;
using System.Linq;

namespace RawSalt.App.Desktop;

public unsafe class DesktopApplication : Application
{
	protected IWindow mainWindow;
	protected GL gl;
	public DesktopApplication(WindowOptions options) : base()
	{
		mainWindow = Window.Create(options);
		mainView = mainWindow;

		mainWindow.FileDrop += (strs) => FileDropped(strs.Cast<File>().ToArray());
		mainView.Resize += Resize;
		mainView.Update += Update;
		mainView.Render += Draw;
		mainView.Closing += Closing;
		mainView.Load += Initialize;
	}
	public override void Initialize()
	{
		gl = GL.GetApi(mainView);

		Resize(mainView.FramebufferSize);
	}
	public override void Run()
	{
		if (mainView is null)
			throw new NullReferenceException($"{nameof(DesktopApplication)}.{nameof(mainView)} is null");

		mainView.Run();
	}
	public override void Closing()
	{
		base.Closing();
	}
}
