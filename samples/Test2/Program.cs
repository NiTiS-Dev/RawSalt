using NiTiS.GLFW;
using NiTiS.IO;
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
					State = WindowState.Fullscreen,
					RefreshRate = 120,
					VSync = true,
				});

				window.Initialize();

				MemorySize heapSize = new(GC.GetAllocatedBytesForCurrentThread());

				Console.WriteLine($"Boot heap size: {heapSize:MiB!0.00 MiB}");

				Glfw.SwapInterval(1);

				while (true)
				{
					if (0 != Glfw.WindowShouldClose((GlfwWindowHandle*)window))
						break;

					float idt = Math.Abs(MathF.Cos(DateTime.Now.Millisecond / 200f));
					GL.ClearColor(idt, idt, idt, 1f);

					GL.Clear(ClearBufferMask.ColorBufferBit);

					

					window.SwapBuffers();

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