using NiTiS.IO;
using RawSalt;
using RawSalt.Core;
using RawSalt.Core.Desktop;
using RawSalt.Core.Scenes.Objects;
using RawSalt.Graphics.Textures;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System;
using System.Drawing;
using System.Numerics;
using Buffer = RawSalt.Graphics.Memory.Buffer;
using Shader = RawSalt.Graphics.Shaders.Shader;
using Texture = RawSalt.Graphics.Textures.Texture;
using VertexArray = RawSalt.Graphics.Memory.VertexArray;

namespace Test1;

public unsafe class GameApplication : DesktopApplication
{
	private Shader shader;
	private VertexArray array;
	private Buffer buffer;
	private Texture texture;
	private Camera camera;
	private IKeyboard keyboard;

	private static readonly float[] Vertices =
		{
            //X    Y      Z     U   V
            -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
			 0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
			 0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
			 0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
			-0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
			-0.5f, -0.5f, -0.5f,  0.0f, 0.0f,

			-0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
			 0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
			 0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
			 0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
			-0.5f,  0.5f,  0.5f,  0.0f, 1.0f,
			-0.5f, -0.5f,  0.5f,  0.0f, 0.0f,

			-0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
			-0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
			-0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
			-0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
			-0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
			-0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

			 0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
			 0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
			 0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
			 0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
			 0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
			 0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

			-0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
			 0.5f, -0.5f, -0.5f,  1.0f, 1.0f,
			 0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
			 0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
			-0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
			-0.5f, -0.5f, -0.5f,  0.0f, 1.0f,

			-0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
			 0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
			 0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
			 0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
			-0.5f,  0.5f,  0.5f,  0.0f, 0.0f,
			-0.5f,  0.5f, -0.5f,  0.0f, 1.0f
		};

	private static void Main(string[] args)
	{
		WindowOptions options = new WindowOptions()
		{
			Size = new(1280, 720),
			Title = "Application",
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
			Samples = 8,
		};

		new GameApplication(options, args);
	}
	public GameApplication(WindowOptions options, params string[] args) : base(new PlatformType(Side.User, RawSalt.Core.Platform.Windows), options)
	{
		camera = new(Vector3D<float>.UnitZ * 6, -Vector3D<float>.UnitZ, Vector3D<float>.UnitY, (float)Window.Size.X / Window.Size.Y); 
		IInputContext inputContext = Window.CreateInput();

		foreach(IKeyboard keyboard in inputContext.Keyboards)
		{
			keyboard.KeyDown += (_, key, _) =>
			{
				if (key == Key.F11)
					Window.WindowState = WindowState.Normal != Window.WindowState ? WindowState.Normal : WindowState.Fullscreen;
			};
			this.keyboard = keyboard;
		}
		foreach (IMouse mouse in inputContext.Mice)
		{
			mouse.Scroll += (mc, scrl) =>
			{
				camera.IncresseZoom(scrl.Y);
			};
		}

		GL gl = GL;


		texture = Texture.Create(gl, new(@"B:\User\Desktop\triangles.png"));

		Atlas.AddTexture(texture, "t");

		shader = Shader.Create(gl, new File("shader.vert").ReadAllText(), new File("shader.frag").ReadAllText());

		buffer = new(gl.CreateBuffer(), BufferTargetARB.ArrayBuffer);
		
		array = new(gl.CreateVertexArray());
		array.Bind(gl);
		
		buffer.Bind(gl);

		buffer.Data(gl, Vertices);

		gl.Enable(EnableCap.Blend);
		gl.BlendFuncSeparate(BlendingFactor.SrcAlpha, BlendingFactor.One, BlendingFactor.One, BlendingFactor.Zero);
		
		array.AttributePointer(gl, 0, 3, VertexAttribPointerType.Float, sizeof(float) * 5, 0);
		array.AttributePointer(gl, 1, 2, VertexAttribPointerType.Float, sizeof(float) * 5, sizeof(float) * 3);
		gl.ClearColor(Color.Black);

		Launch();
	}
	public override void GraphicUpdate(double delta)
	{
		GL.Enable(EnableCap.DepthTest);
		GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

		texture.Bind(GL, 1);
		GL.UseProgram(shader.Handle);

		Vector4D<float> color = new(1f, 1f, 1f, 1);
		Matrix4X4<float> model = Matrix4X4<float>.Identity;
		model *= Matrix4X4.CreateScale(0.5f);
		model *= Matrix4X4.CreateTranslation(new Vector3D<float>(1f, 1f, 2f));
		model *= Matrix4X4.CreateRotationZ(SaltMath.DegreesToRadians(45f));
		model *= Matrix4X4.CreateRotationX(SaltMath.DegreesToRadians(45f));
		model *= Matrix4X4.CreateRotationY(SaltMath.DegreesToRadians(45f));


		shader.UniformMat4(GL, "uMat", model * camera.GetView() * camera.GetProjection());
		shader.UniformVec4(GL, "uColor", color);
		shader.UniformInt32(GL, "uTex0", 1);

		GL.DrawArrays(PrimitiveType.Triangles, 0, 36);

		
		shader.UniformMat4(GL, "uMat", Matrix4X4<float>.Identity);
		GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
	}
	public override void Update(double delta)
	{

	}
	public override void Resize(Vector2D<int> newSize)
	{
		GL.Viewport(newSize);
	}
}
