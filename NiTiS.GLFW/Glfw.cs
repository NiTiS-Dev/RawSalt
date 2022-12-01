using NiTiS.Native;

namespace NiTiS.GLFW;

[NativeAPI(MethodPrefix = "glfw")]
[LibraryContainer(Container = typeof(GlfwContainer))]
public static unsafe partial class Glfw
{
	public static readonly delegate* unmanaged[Cdecl]<GlfwBool> Init;
	public static readonly delegate* unmanaged[Cdecl]<void> Terminate;
	public static readonly delegate* unmanaged[Cdecl]<GlfwInitHint, int, void> InitHint;
	public static readonly delegate* unmanaged[Cdecl]<int*, int*, int*, void> GetVersion;
	public static readonly delegate* unmanaged[Cdecl]<char*> GetVersionString;
	public static readonly delegate* unmanaged[Cdecl]<char**, GlfwError> GetError;
	
	public static readonly delegate* unmanaged[Cdecl]<delegate* unmanaged[Cdecl]<GlfwError, char*, void>, delegate* unmanaged[Cdecl]<GlfwError, char*, void>> SetErrorCallback;

	public static readonly delegate* unmanaged[Cdecl]<int*, GlfwMonitor**> GetMonitors;
	public static readonly delegate* unmanaged[Cdecl]<GlfwMonitor*> GetPrimaryMonitor;
	public static readonly delegate* unmanaged[Cdecl]<GlfwMonitor*, int*, int*, void> GetMonitorPos;
	public static readonly delegate* unmanaged[Cdecl]<GlfwMonitor*, int*, int*, int*, int*, void> GetMonitorWorkarea;
	public static readonly delegate* unmanaged[Cdecl]<GlfwMonitor*, int*, int*, void> GetMonitorPhysicalSize;
	public static readonly delegate* unmanaged[Cdecl]<GlfwMonitor*, float*, float*, void> GetMonitorContentScale;
	public static readonly delegate* unmanaged[Cdecl]<GlfwMonitor*, char*> GetMonitorName;
	public static readonly delegate* unmanaged[Cdecl]<GlfwMonitor*, void*, void> SetMonitorUserPointer;
	public static readonly delegate* unmanaged[Cdecl]<GlfwMonitor*, void*> GetMonitorUserPointer;

	public static readonly delegate* unmanaged[Cdecl]<delegate* unmanaged[Cdecl]<GlfwMonitor*, GlfwEvent, void>, delegate* unmanaged[Cdecl]<GlfwMonitor*, GlfwEvent, void>> SetMonitorCallback;

	public static readonly delegate* unmanaged[Cdecl]<GlfwMonitor*, int*, GlfwVideoMode*> GetVideoModes;
	public static readonly delegate* unmanaged[Cdecl]<GlfwMonitor*, GlfwVideoMode*> GetVideoMode;
	public static readonly delegate* unmanaged[Cdecl]<GlfwMonitor*, float, void> SetGamma;
	public static readonly delegate* unmanaged[Cdecl]<GlfwMonitor*, GlfwGammaRamp*> GetGammaRamp;
	public static readonly delegate* unmanaged[Cdecl]<GlfwMonitor*, GlfwGammaRamp*, void> SetGammaRamp;


	public static readonly delegate* unmanaged[Cdecl]<void> DefaultWindowHints;
	public static readonly delegate* unmanaged[Cdecl]<int, int, void> WindowHint; //TODO: Ints hint, value : void https://www.glfw.org/docs/3.3/window_guide.html#window_hints
	public static readonly delegate* unmanaged[Cdecl]<int, char*, void> WindowHintString; //TODO: Ints hint, value : void https://www.glfw.org/docs/3.3/window_guide.html#window_hints
	public static readonly delegate* unmanaged[Cdecl]<int, int, char*, GlfwMonitor*, GlfwWindow*, GlfwWindow*> CreateWindow;
	public static readonly delegate* unmanaged[Cdecl]<GlfwWindow*, void> DestroyWindow;




	static Glfw()
	{
		// Initialize glfw methods
		NativeAPI.Initialize(typeof(Glfw));
	}

	private class GlfwContainer : INativeLibraryContainer
	{
		public string WindowsX86 => "glfw3.dll";
		public string WindowsX64
			=> WindowsX86;
		public string Linux => "libglfw.so.3.3";
		public string Android
			=> Linux;
		public string MacOS => "libglfw.3.dylib";
		public string IOS
			=> MacOS;
	}
}