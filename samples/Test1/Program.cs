using RawSalt.Core;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Drawing;
using Buffer = RawSalt.Graphics.Memory.Buffer;
using VertexArray = RawSalt.Graphics.Memory.VertexArray;
using Shader = RawSalt.Graphics.Shaders.Shader;
using Silk.NET.Maths;
using Silk.NET.Input;
using RawSalt.Core.Scenes;
using RawSalt.Core.Desktop;
using RawSalt.Graphics.Textures;
using RawSalt;

namespace Test1;

public unsafe class Program
{
	private readonly DesktopApplication app;
	private static void Main(string[] args)
		=> _ = new Program(args);
	public Program(params string[] args)
	{
		app = new(new PlatformType(Side.Server, Platform.Windows), new WindowOptions()
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
		})
		{
			Scene = new()
			{
				Name = "loading.scn",
				UnloadType = SceneUnloadType.WaitUntilUnloadingEnd,
			}
		};
		app.Scene = new()
		{
			Name = "game.scn",
			UnloadType = SceneUnloadType.WaitUntilUnloadingEnd,
		};

		IInputContext inputContext = app.Window.CreateInput();

		IWindow window = app.Window;

		foreach(IKeyboard keyboard in inputContext.Keyboards)
		{
			keyboard.KeyDown += (_, key, _) =>
			{
				if (key == Key.F11)
					window.WindowState = WindowState.Normal != window.WindowState ? WindowState.Normal : WindowState.Fullscreen;
			};
		}

		GL gl = app.GL;
		Atlas atlas = app.Atlas;

		atlas.AddTexture(new(1), "blabla");

		Shader shader = Shader.Create(gl, File.ReadAllText("shader.vert"), File.ReadAllText("shader.frag"));

		Buffer buffer = new(gl.CreateBuffer(), BufferTargetARB.ArrayBuffer);
		
		VertexArray array = new(gl.CreateVertexArray());
		array.Bind(gl);
		
		buffer.Bind(gl);

		float[] vertexs = new float[]
		{
			-0.5f, -0.5f,  -0.5f,   1, 1, 1, 1,
			 0.5f, -0.5f,  -0.5f,   1, 1, 1, 1,
			 0.5f,  0.5f,  -0.5f,   1, 1, 1, 1,

			-0.5f, -0.5f,  -0.5f,   1, 1, 1, 1,
			 0.5f,  0.5f,  -0.5f,   1, 1, 1, 1,
			-0.5f,  0.5f,  -0.5f,   1, 1, 1, 1,
		};
		buffer.Data(gl, vertexs);

		gl.Enable(EnableCap.Blend);
		gl.BlendFuncSeparate(BlendingFactor.SrcAlpha, BlendingFactor.One, BlendingFactor.One, BlendingFactor.Zero);

		gl.Enable(EnableCap.PolygonSmooth);
		gl.Enable(EnableCap.CullFace);

		gl.CullFace(CullFaceMode.Back);

		
		array.AttributePointer(gl, 0, 3, VertexAttribPointerType.Float, sizeof(float) * 7, 0);
		array.AttributePointer(gl, 1, 4, VertexAttribPointerType.Float, sizeof(float) * 7, sizeof(float) * 3);

		

		gl.ClearColor(Color.Black);

		window.Resize += gl.Viewport;
		window.Render += (arg) =>
		{
			gl.Clear(ClearBufferMask.ColorBufferBit);

			gl.UseProgram(shader.Handle);
			Matrix4X4<float> identity = Matrix4X4<float>.Identity;
			identity *= Matrix4X4.CreateScale(0.5f);
			identity *= Matrix4X4.CreateRotationZ(SaltMath.DegressToRadiant(DateTime.Now.Microsecond));
			gl.ProgramUniformMatrix4(shader.Handle, gl.GetUniformLocation(shader.Handle, "uMat"), 1, false, (float*)&identity);
			gl.DrawArrays(PrimitiveType.Triangles, 0, 6);
		};

		window.Run();
	}
}
