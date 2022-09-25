using RawSalt.Core;
using RawSalt.Graphics.Memory;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Buffer = RawSalt.Graphics.Memory.Buffer;

namespace Test1;

public class Program
{
	private static void Main(string[] args)
		=> new Program(args);
	public Program(params string[] args)
	{
		Application application = new(new PlatformType(Side.Server, Platform.Windows));

		IWindow window = Window.Create(new WindowOptions()
		{
			Size = new(1280, 720),
			Title = "Abobus",
			IsVisible = true,
			Position = new(50, 50),
			UpdatesPerSecond = 0,
			FramesPerSecond = 0,
			VSync = true,
			VideoMode = VideoMode.Default,
			ShouldSwapAutomatically = true,
			WindowBorder = WindowBorder.Resizable,
			WindowState = WindowState.Normal,
			API = new GraphicsAPI()
			{
				API = ContextAPI.OpenGL,
				Profile = ContextProfile.Core,
				Version = new APIVersion(3, 3),
			},
		});

		GL gl = window.CreateOpenGL();

		Buffer buffer = new(gl.CreateBuffer());

		buffer.Bind(gl, BufferTargetARB.ArrayBuffer);

		window.Run();
	}
}
