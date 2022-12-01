using NiTiS.Core;
using NiTiS.IO;
using NiTiS.Math;
using RawSalt;
using RawSalt.App.Desktop;
using RawSalt.Engine;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System;
using System.Drawing;
using System.Numerics;

namespace Tutorial;

public unsafe class TestApplication : DesktopApplication
{
	private static Buffer Vbo;
	private static Buffer Ebo;
	private static VertexArray Vao;
	private static Shader Shader;
	private static Texture Texture;

	private static Vector3 CameraPosition = new Vector3(0.0f, 0.0f, 3.0f);
	private static Vector3 CameraFront = new Vector3(0.0f, 0.0f, -1.0f);
	private static Vector3 CameraUp = Vector3.UnitY;
	private static Vector3 CameraDirection = Vector3.Zero;


	//Vertex data, uploaded to the VBO.
	private static readonly float[] Vertices =
	{
		//X    Y      Z     U   V
		// Front
		-0.5f, -0.5f, -0.5f, 0.0f, 0.0f, // 0
		 0.5f, -0.5f, -0.5f, 1.0f, 0.0f, // 1
		 0.5f,  0.5f, -0.5f, 1.0f, 1.0f, // 2
		-0.5f,  0.5f, -0.5f, 0.0f, 1.0f, // 3

		// Right
		 0.5f, -0.5f, -0.5f, 0.0f, 0.0f, // 4
		 0.5f, -0.5f,  0.5f, 1.0f, 0.0f, // 5
		 0.5f,  0.5f,  0.5f, 1.0f, 1.0f, // 6
		 0.5f,  0.5f, -0.5f, 0.0f, 1.0f, // 7

		 // Back
		 0.5f, -0.5f,  0.5f, 0.0f, 0.0f, // 8
		-0.5f, -0.5f,  0.5f, 1.0f, 0.0f, // 9
		-0.5f,  0.5f,  0.5f, 1.0f, 1.0f, // 10
		 0.5f,  0.5f,  0.5f, 0.0f, 1.0f, // 11

		 // Left
		-0.5f, -0.5f,  0.5f, 0.0f, 0.0f, // 12
		-0.5f, -0.5f, -0.5f, 1.0f, 0.0f, // 13
		-0.5f,  0.5f, -0.5f, 1.0f, 1.0f, // 14
		-0.5f,  0.5f,  0.5f, 0.0f, 1.0f, // 15

		// Top
		-0.5f,  0.5f, -0.5f, 0.0f, 0.0f, // 16
		 0.5f,  0.5f, -0.5f, 1.0f, 0.0f, // 17

		// Bottom
		 0.5f, -0.5f,  0.5f, 1.0f, 1.0f, // 18
	};

	//Index data, uploaded to the EBO.
	private static readonly uint[] Indices =
	{
		0, 1, 2,
		0, 2, 3,

		4, 5, 6,
		4, 6, 7,

		8, 9, 10,
		8, 10, 11,

		12, 13, 14,
		12, 14, 15,

		16, 17, 6,
		16, 6, 15,

		0, 1, 18,
		0, 18, 9
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
	private static IKeyboard primaryKeyboard;
	public override unsafe void Initialize()
	{
		base.Initialize();
		gl.ClearColor(Color.White);
		//sGL.glClearColor(0f, 255f, 0f, 255f);

		gl.Enable(EnableCap.DepthTest);
		//gl.Enable(EnableCap.CullFace);


		IInputContext input = mainView.CreateInput();
		for (int i = 0; i < input.Keyboards.Count; i++)
		{
			primaryKeyboard = input.Keyboards[i];
		}

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
		position = new(1.1f, 0.25f, 0f),
		rotation = default,
	};
	public override unsafe void Draw(double delta)
	{

		fps = 1 / delta;

		//Clear the color channel.
		gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

		//Bind the geometry and shader.
		Vao.Bind(gl);
		Shader.Use(gl);
		Texture.Bind(gl, 1);
		Shader.UniformTex(gl, "uTex0", 1);

		mat4 model = trans.CreateModelMatrix();
		mat4 view = Matrix4x4.CreateLookAt(CameraPosition, CameraPosition + CameraFront, CameraUp);
		mat4 projection = Matrix4x4.CreatePerspectiveFieldOfView(SaltMath.DegreesToRadians(70f), (float)mainWindow.FramebufferSize.X / mainWindow.FramebufferSize.Y, 0.1f, 100.0f);

		Shader.Uniform4(gl, "uColor", new Color32(0, 255, 122));
		Shader.UniformMat4(gl, "uMVP", model * view * projection);

		gl.DrawElements(PrimitiveType.Triangles, (uint)Indices.Length, DrawElementsType.UnsignedInt, null);

		model = (mat4)trans2.CreateModelMatrix();
		Shader.UniformMat4(gl, "uMVP", model * view * projection);

		gl.DrawElements(PrimitiveType.Triangles, (uint)Indices.Length, DrawElementsType.UnsignedInt, null);
	}
	public override void Update(double delta)
	{
		//Camera.Position = new(0, 0, MathF.Sin((float)mainWindow.Time) * 5);

		mainWindow.Title = $"CameraPos: {CameraPosition} CameraDir: {CameraDirection} CameraUp: {CameraUp} CameraFront: {CameraFront}";

		float moveSpeed = 2.5f * (float)delta;

		if (primaryKeyboard.IsKeyPressed(Key.W))
		{
			//Move forwards
			CameraPosition += moveSpeed * CameraFront;
		}
		if (primaryKeyboard.IsKeyPressed(Key.S))
		{
			//Move backwards
			CameraPosition -= moveSpeed * CameraFront;
		}
		if (primaryKeyboard.IsKeyPressed(Key.A))
		{
			//Move left
			CameraPosition -= Vector3.Normalize(Vector3.Cross(CameraFront, CameraUp)) * moveSpeed;
		}
		if (primaryKeyboard.IsKeyPressed(Key.D))
		{
			//Move right
			CameraPosition += Vector3.Normalize(Vector3.Cross(CameraFront, CameraUp)) * moveSpeed;
		}
	}

	public override void Resize(vec2i size)
	{
		gl.Viewport(size);
	}
	public override void Closing()
	{
		base.Closing();

		Vbo.Dispose(gl);
		Ebo.Dispose(gl);
		Vao.Dispose(gl);
		Shader.Dispose(gl);
		Texture.Dispose(gl);
	}
}