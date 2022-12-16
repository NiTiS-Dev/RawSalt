using NiTiS.Native;
using System.Runtime.InteropServices;

namespace NiTiS.GLFW;

public static unsafe class GlfwCallbacks
{
	/// <summary>
	/// This is the function pointer type for error callbacks
	/// </summary>
	/// <param name="errorCode">An error code</param>
	/// <param name="description">A UTF-8 encoded string describing the error</param>
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	public delegate void Error(GlfwError errorCode, CString description);

	/// <summary>
	/// This is the function pointer type for path drop callbacks
	/// </summary>
	/// <param name="window">The window that received the event</param>
	/// <param name="pathCount">The number of dropped paths</param>
	/// <param name="paths">The UTF-8 encoded file and/or directory path names</param>
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	public delegate void Drop(GlfwWindowHandle* window, int pathCount, CString* paths);

	/// <summary>
	/// This is the function pointer type for window position callbacks
	/// </summary>
	/// <param name="window">The window that was moved</param>
	/// <param name="xpos">The new x-coordinate, in screen coordinates, of the upper-left corner of the content area of the window</param>
	/// <param name="ypos">	The new y-coordinate, in screen coordinates, of the upper-left corner of the content area of the window</param>
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	public delegate void WindowPos(GlfwWindowHandle* window, int xpos, int ypos);

	/// <summary>
	/// This is the function pointer type for window size callbacks
	/// </summary>
	/// <param name="window">The window that was resized</param>
	/// <param name="width">The new width, in screen coordinates, of the window</param>
	/// <param name="height">The new height, in screen coordinates, of the window</param>
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	public delegate void WindowSize(GlfwWindowHandle* window, int width, int height);

	/// <summary>
	/// This is the function pointer type for window close callbacks
	/// </summary>
	/// <param name="window">The window that the user attempted to close</param>
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	public delegate void WindowClose(GlfwWindowHandle* window);

	/// <summary>
	/// This is the function pointer type for window content refresh callbacks
	/// </summary>
	/// <param name="window">The window whose content needs to be refreshed</param>
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	public delegate void WindowRefresh(GlfwWindowHandle* window);

	/// <summary>
	/// This is the function pointer type for window focus callbacks
	/// </summary>
	/// <param name="window">The window that gained or lost input focus</param>
	/// <param name="focused"><see cref="GlfwBool.True"/> if the window was given input focus, or <see cref="GlfwBool.False"/> if it lost it.</param>
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	public delegate void WindowFocus(GlfwWindowHandle* window, GlfwBool focused);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	public delegate void WindowIconify(GlfwWindowHandle* window, GlfwBool iconified);

	/// <summary>
	/// This is the function pointer type for window maximize callbacks
	/// </summary>
	/// <param name="window">The window that was maximized or restored</param>
	/// <param name="maximized"><see cref="GlfwBool.True"/> if the window was maximized, or <see cref="GlfwBool.False"/> if it was restored.</param>
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	public delegate void WindowMaximize(GlfwWindowHandle* window, GlfwBool maximized);

	/// <summary>
	/// This is the function pointer type for framebuffer size callbacks
	/// </summary>
	/// <param name="window">The window whose framebuffer was resized</param>
	/// <param name="width">The new width, in pixels, of the framebuffer</param>
	/// <param name="height">The new height, in pixels, of the framebuffer</param>
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	public delegate void FramebufferSize(GlfwWindowHandle* window, int width, int height);

	/// <summary>
	/// This is the function pointer type for window content scale callbacks
	/// </summary>
	/// <param name="window">The window whose content scale changed</param>
	/// <param name="xscale">The new x-axis content scale of the window</param>
	/// <param name="yscale">The new y-axis content scale of the window</param>
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	public delegate void WindowContentScale(GlfwWindowHandle* window, float xscale, float yscale);
}