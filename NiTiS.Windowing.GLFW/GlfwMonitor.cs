using NiTiS.GLFW;

namespace NiTiS.Windowing.GLFW;

public unsafe class GlfwMonitor : IMonitor, IWindowHost
{
	private GlfwMonitorHandle* handle;

	public GlfwMonitor(GlfwMonitorHandle* handle)
	{
		this.handle = handle;
	}

	private static GlfwMonitor primary;
	public static GlfwMonitor GetPrimary()
	{
		primary = new(Glfw.GetPrimaryMonitor());

		return primary;
	}

	public IWindow CreateWindow(WindowOptions options)
		=> new GlfwWindow(options, this);

	public static explicit operator GlfwMonitorHandle*(GlfwMonitor monitor)
		=> monitor.handle;
}
