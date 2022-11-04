using NiTiS.IO;
using RawSalt.App.Desktop;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System;
using System.Drawing;

namespace Test1;
public unsafe class TestApplication : DesktopApplication
{
	private static TestApplication? instance;
	public static TestApplication Isntance => Isntance;
	public TestApplication(WindowOptions options) : base(options) { }
	private static void Main(string[] args)
	{
		TestApplication? app = null;
		try
		{
			app = new TestApplication(new WindowOptions()
			{
				ShouldSwapAutomatically = true,
				WindowBorder = WindowBorder.Resizable,
				IsVisible = true,
				FramesPerSecond = 60,
				UpdatesPerSecond = 100,
				Size = new(1020, 700),
				Title = "TestApplication",
				Position = new(200, 200),
				API = new GraphicsAPI()
				{
					API = ContextAPI.OpenGL,
					Profile = ContextProfile.Core,
					Version = new APIVersion(3, 3),
				},
				Samples = 8,
			});

			instance ??= app;

			app.mainWindow.Run();
		}
		finally
		{
			app?.Closing();
			instance = null;
		}
	}
	private GL gl;
	private Buffer vbo;
	private uint vao;
	private Shader shader;
	private Texture texture;
	public override void Initialize()
	{
		gl = mainWindow.CreateOpenGL();
		gl.ClearColor(Color.Black);

		texture = Texture.Create(gl, new File(@"A:\NiTiS\NiTiS\nitis-logo-low.png"));
		texture.Bind(gl, TextureUnit.Texture0);

		vbo = new(gl.GenBuffer(), BufferTargetARB.ArrayBuffer);
		vao = gl.GenVertexArray();

		shader = Shader.Create(gl, new File("shader.vert").ReadAllText(), new File("shader.frag").ReadAllText());

		gl.BindVertexArray(vao);
		vbo.Bind(gl);

		ReadOnlySpan<float> data = stackalloc float[]
		{
			-.5f, -.5f, 0, 0, 0,
			 .5f, -.5f, 0, 1, 0,
			 .5f,  .5f, 0, 1, 1,

			 .5f,  .5f, 0, 1, 1,
			-.5f, -.5f, 0, 0, 0,
			-.5f,  .5f, 0, 0, 1,
		};
		gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float)*5, (void*)0);
		gl.EnableVertexAttribArray(0);
		gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, sizeof(float)*5, (void*)(sizeof(float) * 3));
		gl.EnableVertexAttribArray(1);

		gl.BufferData(BufferTargetARB.ArrayBuffer, data, BufferUsageARB.StaticDraw);
	}
	public override void Resize(vec2i size)
	{
		gl.Viewport(size);
	}
	public override void Draw(double delta)
	{
		mainWindow.Title = $"TestApplication " + 1/delta;
		gl.Clear(ClearBufferMask.ColorBufferBit);
		gl.UseProgram(shader.Handle);
		mat4 uMat = mat4.Identity;

		uMat *= Mat4.CreateScale(100f, 100f, 1f);
		uMat *= Mat4.CreateOrthographic(mainWindow.FramebufferSize.X, mainWindow.FramebufferSize.Y, float.Epsilon, 10000f);

		shader.UniformMat4(gl, "uMat", uMat);

		gl.UniformMatrix4(gl.GetUniformLocation(shader.Handle, "uMat"), 1, true, (float*)&uMat);
		
		shader.UniformTex(gl, "uTex0", TextureUnit.Texture0);

		shader.Uniform4(gl, "uColor", new Color32(122, 255, 255));

		gl.BindVertexArray(vao);
		vbo.Bind(gl);
		gl.DrawArrays(PrimitiveType.Triangles, 0, 12);
	}
}