using NiTiS.GLFW;
using NiTiS.GLFW.Enums;
using NiTiS.Native;
using NiTiS.OpenGL;
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
				GlfwWindow window = new();

				GlfwWindowHandle* pWindow;

				ReadOnlySpan<byte> text = "Hello World"u8;

				Span<byte> text2 = stackalloc byte[text.Length];

				text.CopyTo(text2);

				fixed (byte* pChars = text2)
				{
					window = Glfw.CreateWindow(700, 500, (CString)pChars, null, null);
				}


				Console.WriteLine(Glfw.GetMonitorName(Glfw.GetPrimaryMonitor()));


				Glfw.WindowHint((int)WindowIntAttribute.ContextVersionMajor, 3);
				Glfw.WindowHint((int)WindowIntAttribute.ContextVersionMinor, 3);
				Glfw.WindowHint((int)WindowOpenGLProfileAttribute.OpenGlProfile, (int)OpenGLProfile.Any);
				Glfw.WindowHint((int)WindowBoolAttribute.DoubleBuffer, (int)GlfwBool.True);
				Glfw.WindowHint((int)WindowBoolAttribute.Decorated, (int)GlfwBool.True);

				Glfw.MakeContextCurrent(window);

				Glfw.SwapInterval(1);

				Glfw.PrepareForOpenGL();

				GL.ClearColor(1f, 0f, 1f, 1f);
				while (0 == Glfw.WindowShouldClose(window))
				{
					GL.Clear(ClearBufferMask.ColorBufferBit);

					Glfw.SwapBuffers(window);
					Glfw.PollEvents();

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