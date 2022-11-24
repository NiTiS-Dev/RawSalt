using NiTiS.IO;
using RawSalt.App.Desktop;
using RawSalt.Engine;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System;
using System.Drawing;

namespace Tutorial;

public class TestApplication : DesktopApplication
{
	private static Buffer Vbo;
	private static Buffer Ebo;
	private static VertexArray Vao;
	private static Shader Shader;
	private static Texture Texture;

	//Vertex data, uploaded to the VBO.
	private static readonly float[] Vertices =
	{
		-0.5f, -0.5f, 0.0f,  0f, 0f,
		 0.5f, -0.5f, 0.0f,  0f, 1f,
		 0.5f,  0.5f, 0.0f,  1f, 1f,
		-0.5f,  0.5f, 0.5f,  1f, 0f,
	};

	//Index data, uploaded to the EBO.
	private static readonly uint[] Indices =
	{
		0, 1, 3,
		1, 2, 3
	};

	public TestApplication(WindowOptions options) : base(options) { }
	private static void Main(string[] args)
	{
		WindowOptions options = new WindowOptions() with
		{
			Size = new(1200, 700),
			API = new(ContextAPI.OpenGL, new(3, 3)),
			FramesPerSecond = 144,
			UpdatesPerSecond = 100,
			ShouldSwapAutomatically = true,
			Title = "Test 1",
			IsVisible = true,
		};

		TestApplication apl = new TestApplication(options);

		apl.Run();
	}

	public override unsafe void Initialize()
	{
		base.Initialize();

		gl.ClearColor(Color.White);

		//IInputContext input = mainView.CreateInput();
		//for (int i = 0; i < input.Keyboards.Count; i++)
		//{
		//	input.Keyboards[i].KeyDown += KeyDown;
		//}

		//Creating a vertex array.
		Vao = new(gl);
		Vao.Bind(gl);

		//Initializing a vertex buffer that holds the vertex data.
		Vbo = new(gl, BufferTargetARB.ArrayBuffer);
		Vbo.Bind(gl);
		Vbo.Data(gl, Vertices);

		//Initializing a element buffer that holds the index data.
		Ebo = new(gl, BufferTargetARB.ElementArrayBuffer);
		Ebo.Bind(gl);
		Ebo.Data(gl, Indices);

		Shader = Shader.Create(gl, new File("shader.vert").ReadAllText(), new File("shader.frag").ReadAllText());

		Texture = Texture.Create(gl, new File("cringe.png"));

		//Tell opengl how to give the data to the shaders.
		Vao.AttributePointer<float>(gl, 0, 3, VertexAttribPointerType.Float, 5, 0);
		Vao.AttributePointer<float>(gl, 1, 2, VertexAttribPointerType.Float, 5, 3);
	}
	private double ups, fps;
	private Transform3D trans = new Transform3D() with
	{
		position = default,
		scale = vec3.One,
		rotation = default,
	};
	private Transform3D trans2 = new Transform3D() with
	{
		position = new(0.25f, 0f, -0.25f),
		scale = vec3.One / 2,
		rotation = default,
	};
	public override unsafe void Draw(double delta)
	{
		fps = 1 / delta;

		//Clear the color channel.
		gl.Clear(ClearBufferMask.ColorBufferBit);

		//Bind the geometry and shader.
		Vao.Bind(gl);
		Shader.Use(gl);
		Texture.Bind(gl, 1);
		Shader.UniformTex(gl, "uTex0", 1);

		
		Shader.UniformMat4(gl, "uMat", trans.CreateView());

		//Draw the geometry.
		gl.DrawElements(PrimitiveType.Triangles, (uint)Indices.Length, DrawElementsType.UnsignedInt, null);

		Shader.UniformMat4(gl, "uMat", trans2.CreateView());
		gl.DrawElements(PrimitiveType.Triangles, (uint)Indices.Length, DrawElementsType.UnsignedInt, null);
	}
	public override void Update(double delta)
	{
		ups = 1 / delta;

		mainWindow.Title = $"Test 1 UPS: {Math.Floor(ups)}, FPS: {Math.Floor(fps)}";

		trans.scale = new((float)Math.Abs(Math.Cos(mainWindow.Time)));

		trans2.rotation = quat.CreateFromAxisAngle(vec3.UnitZ, (float)Math.Abs(Math.Cos(mainWindow.Time)));
	}
	public override void Resize(vec2i size)
	{
		gl.Viewport(size);
	}
	public override void Closing()
	{
		base.Closing();

		//Remember to delete the buffers.
		Vbo.Dispose(gl);
		Ebo.Dispose(gl);
		Vao.Dispose(gl);
		Shader.Dispose(gl);
		Texture.Dispose(gl);
	}
}