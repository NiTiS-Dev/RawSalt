using NiTiS.GLFW;
using NiTiS.Native.Loaders;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

internal unsafe class Program
{
	private static void Main(string[] args)
	{
		if (GlfwBool.True == Glfw.Init())
		{
			char* title = stackalloc char[] {'H', 'i' };
			GlfwWindow* window = Glfw.CreateWindow(400, 400, title, null, null);

			Task.Delay(1000).Wait();

			Glfw.DestroyWindow(window);
		}

		Glfw.Terminate();
	}
}