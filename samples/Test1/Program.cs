using RawSalt.Core;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Drawing;
using Buffer = RawSalt.Graphics.Memory.Buffer;
using VertexArray = RawSalt.Graphics.Memory.VertexArray;
using Shader = RawSalt.Graphics.Shaders.Shader;
using Silk.NET.Maths;
using Silk.NET.Input;

namespace Test1;

public unsafe class Program
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
			UpdatesPerSecond = 10,
			FramesPerSecond = 99999,
			VSync = false,
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
			Samples = 16,
		});

		window.Initialize();

		IInputContext inputContext = window.CreateInput();

		foreach(IKeyboard keyboard in inputContext.Keyboards)
		{
			keyboard.KeyDown += (_, key, _) =>
			{
				if (key == Key.F11)
					window.WindowState = WindowState.Normal != window.WindowState ? WindowState.Normal : WindowState.Fullscreen;
			};
		}

		GL gl = window.CreateOpenGL();

		Shader shader = Shader.Create(gl, File.ReadAllText("shader.vert"), File.ReadAllText("shader.frag"));

		Buffer buffer = new(gl.CreateBuffer(), BufferTargetARB.ArrayBuffer);
		
		VertexArray array = new(gl.CreateVertexArray());
		array.Bind(gl);
		
		buffer.Bind(gl);

		float[] vertexs = new float[]
		{
			0, 0, 0, 0, 0, 1,
			1, 0, 0, 1, 0, 0,
			1, 1, 0, 0, 0, 1,

			0, 0, 0, 0, 1, 0,
			1, 1, 0, 0, 0, 1,
			0, 1, 0, 0, 1, 0,
		};
		buffer.Data(gl, vertexs);

		gl.Enable(EnableCap.PolygonSmooth);
		gl.Enable(EnableCap.CullFace);

		gl.CullFace(CullFaceMode.Back);

		Matrix4X4<float> identity = Matrix4X4<float>.Identity;
		gl.ProgramUniformMatrix4(shader.Handle, gl.GetUniformLocation(shader.Handle, "uMat"), 1, false, (float*)&identity);
		array.AttributePointer(gl, 0, 3, VertexAttribPointerType.Float, sizeof(float) * 6, 0);
		array.AttributePointer(gl, 1, 3, VertexAttribPointerType.Float, sizeof(float) * 6, sizeof(float) * 3);

		

		gl.ClearColor(Color.Black);

		window.Resize += gl.Viewport;
		window.Render += (arg) =>
		{
			gl.Clear(ClearBufferMask.ColorBufferBit);

			gl.UseProgram(shader.Handle);
			gl.DrawArrays(PrimitiveType.Triangles, 0, 6);
		};

		window.Run();
	}
}
