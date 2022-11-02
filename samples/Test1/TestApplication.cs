using NiTiS.IO;
using RawSalt.App.Desktop;
using RawSalt.Graphics.Shaders;
using RawSalt.Structs;
using Silk.NET.Core.Contexts;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System;
using System.Drawing;
using System.Reflection;

namespace Test1;
public unsafe class TestApplication : DesktopApplication
{
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

			app.mainWindow.Run();
		}
		finally
		{
			app?.Closing();
		}
	}
	private GL gl;
	private Buffer vbo;
	private uint vao;
	private Shader shader;
	public override void Initialize()
	{
		gl = mainWindow.CreateOpenGL();
		gl.ClearColor(Color.Black);

		vbo = new(gl.GenBuffer(), BufferTargetARB.ArrayBuffer);
		vao = gl.GenVertexArray();

		shader = Shader.Create(gl, new File("shader.vert").ReadAllText(), new File("shader.frag").ReadAllText());

		gl.BindVertexArray(vao);
		vbo.Bind(gl);

		ReadOnlySpan<float> data = stackalloc float[]
		{
			0, 0, 0, 0, 0,
			1, 0, 0, 1, 0,
			1, 1, 0, 1, 1,

			0, 0, 0, 0, 0,
			1, 1, 0, 1, 1,
			0, 1, 0, 0, 1,
		};
		gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float)*5, (void*)0);
		gl.EnableVertexAttribArray(0);
		gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, sizeof(float)*5, (void*)(sizeof(float) * 2));
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
		uMat *= Mat4.CreateTranslation(new vec3(-0.25f, -0.25f, 0));
		gl.UniformMatrix4(gl.GetUniformLocation(shader.Handle, "uMat"), 1, true, (float*)&uMat);

		gl.BindVertexArray(vao);
		vbo.Bind(gl);
		gl.DrawArrays(PrimitiveType.Triangles, 0, 12);
	}
}
