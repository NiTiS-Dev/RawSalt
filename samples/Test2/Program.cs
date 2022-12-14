using NiTiS.GLFW;
using NiTiS.OpenGL;
using NiTiS.Windowing;
using NiTiS.Windowing.GLFW;
using System;

internal unsafe class Program
{
	private static void Main(string[] args)
	{
		try
		{
			if (GlfwBool.True == Glfw.Init())
			{
				GlfwWindow window = new(WindowOptions.Default with
				{
					Title = "Test window",
					Graphics = GraphicsAPIOptions.DefaultOpenGL with
					{
						Flags = ContextAPIFlags.Default,
						Profile = ContextAPIProfile.Core,
						Version = new(3, 3)
					},
					Border = WindowBorder.Hidden,
					TransparentFramebuffer = true,
					PreferredBitDepth = new(8, 8, 8, 8),
				});

				window.Initialize();

				Glfw.PrepareForOpenGL();

				GL.ClearColor(1f, 0f, 1f, 0f);

				while (true)
				{
					GL.Clear(ClearBufferMask.ColorBufferBit);

					Glfw.PollEvents();

					window.SwapBuffers();

					GlfwError error = Glfw.GetError(null);

					if (error != GlfwError.NoError)
						Console.WriteLine("Glfw error: " + error);

				}
			}
		}
		finally
		{
			Glfw.Terminate();
		}
	}
}