using NiTiS.GLFW.Enums;
using NiTiS.Native;

namespace NiTiS.GLFW;

[NativeAPI(MethodPrefix = "glfw")]
public static unsafe partial class Glfw
{
	public const int DontCare = -1;

	public static readonly delegate* unmanaged[Cdecl]<GlfwBool> Init;
	public static readonly delegate* unmanaged[Cdecl]<void> Terminate;
	public static readonly delegate* unmanaged[Cdecl]<GlfwInitHint, int, void> InitHint;
	public static readonly delegate* unmanaged[Cdecl]<int*, int*, int*, void> GetVersion;
	public static readonly delegate* unmanaged[Cdecl]<CString> GetVersionString;
	public static readonly delegate* unmanaged[Cdecl]<CString*, GlfwError> GetError;
	
	public static readonly delegate* unmanaged[Cdecl]<delegate* unmanaged[Cdecl]<GlfwError, CString, void>, delegate* unmanaged[Cdecl]<GlfwError, CString, void>> SetErrorCallback;

	public static readonly delegate* unmanaged[Cdecl]<int*, GlfwMonitor**> GetMonitors;
	public static readonly delegate* unmanaged[Cdecl]<GlfwMonitor*> GetPrimaryMonitor;
	public static readonly delegate* unmanaged[Cdecl]<GlfwMonitor*, int*, int*, void> GetMonitorPos;
	public static readonly delegate* unmanaged[Cdecl]<GlfwMonitor*, int*, int*, int*, int*, void> GetMonitorWorkarea;
	public static readonly delegate* unmanaged[Cdecl]<GlfwMonitor*, int*, int*, void> GetMonitorPhysicalSize;
	public static readonly delegate* unmanaged[Cdecl]<GlfwMonitor*, float*, float*, void> GetMonitorContentScale;
	public static readonly delegate* unmanaged[Cdecl]<GlfwMonitor*, CString> GetMonitorName;
	public static readonly delegate* unmanaged[Cdecl]<GlfwMonitor*, void*, void> SetMonitorUserPointer;
	public static readonly delegate* unmanaged[Cdecl]<GlfwMonitor*, void*> GetMonitorUserPointer;

	public static readonly delegate* unmanaged[Cdecl]<delegate* unmanaged[Cdecl]<GlfwMonitor*, GlfwEvent, void>, delegate* unmanaged[Cdecl]<GlfwMonitor*, GlfwEvent, void>> SetMonitorCallback;

	public static readonly delegate* unmanaged[Cdecl]<GlfwMonitor*, int*, GlfwVideoMode*> GetVideoModes;
	public static readonly delegate* unmanaged[Cdecl]<GlfwMonitor*, GlfwVideoMode*> GetVideoMode;
	public static readonly delegate* unmanaged[Cdecl]<GlfwMonitor*, float, void> SetGamma;
	public static readonly delegate* unmanaged[Cdecl]<GlfwMonitor*, GlfwGammaRamp*> GetGammaRamp;
	public static readonly delegate* unmanaged[Cdecl]<GlfwMonitor*, GlfwGammaRamp*, void> SetGammaRamp;


	public static readonly delegate* unmanaged[Cdecl]<void> DefaultWindowHints;
	public static readonly delegate* unmanaged[Cdecl]<int, int, void> WindowHint;
	public static readonly delegate* unmanaged[Cdecl]<WindowHintString, CString, void> WindowHintString;
	public static readonly delegate* unmanaged[Cdecl]<int, int, CString, GlfwMonitor*, GlfwWindowHandle*, GlfwWindowHandle*> CreateWindow;
	public static readonly delegate* unmanaged[Cdecl]<GlfwWindowHandle*, void> DestroyWindow;
	public static readonly delegate* unmanaged[Cdecl]<GlfwWindowHandle*, int> WindowShouldClose;
	public static readonly delegate* unmanaged[Cdecl]<GlfwWindowHandle*, int, void> SetWindowShouldClose;
	public static readonly delegate* unmanaged[Cdecl]<GlfwWindowHandle*, CString, void> SetWindowTitle;
	public static readonly delegate* unmanaged[Cdecl]<GlfwWindowHandle*, CString> GetWindowTitle;
	public static readonly delegate* unmanaged[Cdecl]<GlfwWindowHandle*, int, GlfwImage*, void> SetWindowIcon;
	public static readonly delegate* unmanaged[Cdecl]<GlfwWindowHandle*, int*, int*, void> GetWindowPos;
	public static readonly delegate* unmanaged[Cdecl]<GlfwWindowHandle*, int, int, void> SetWindowPos;
	public static readonly delegate* unmanaged[Cdecl]<GlfwWindowHandle*, int*, int*, void> GetWindowSize;
	public static readonly delegate* unmanaged[Cdecl]<GlfwWindowHandle*, int, int, void> SetWindowSize;
	public static readonly delegate* unmanaged[Cdecl]<GlfwWindowHandle*, int, int, int, int, void> SetWindowSizeLimits;
	public static readonly delegate* unmanaged[Cdecl]<GlfwWindowHandle*, int, int, void> SetWindowAspectRatio;
	public static readonly delegate* unmanaged[Cdecl]<GlfwWindowHandle*, int*, int*, void> GetFramebufferSize;
	public static readonly delegate* unmanaged[Cdecl]<GlfwWindowHandle*, int*, int*, int*, int*, void> GetWindowFrameSize;
	public static readonly delegate* unmanaged[Cdecl]<GlfwWindowHandle*, float*, float*, void> GetWindowContentScale;
	public static readonly delegate* unmanaged[Cdecl]<GlfwWindowHandle*, float> GetWindowOpacity;
	public static readonly delegate* unmanaged[Cdecl]<GlfwWindowHandle*, float, void> SetWindowOpacity;
	public static readonly delegate* unmanaged[Cdecl]<GlfwWindowHandle*, void> IconifyWindow;
	public static readonly delegate* unmanaged[Cdecl]<GlfwWindowHandle*, void> RestoreWindow;
	public static readonly delegate* unmanaged[Cdecl]<GlfwWindowHandle*, void> MaximizeWindow;
	public static readonly delegate* unmanaged[Cdecl]<GlfwWindowHandle*, void> ShowWindow;
	public static readonly delegate* unmanaged[Cdecl]<GlfwWindowHandle*, void> HideWindow;
	public static readonly delegate* unmanaged[Cdecl]<GlfwWindowHandle*, void> FocusWindow;
	public static readonly delegate* unmanaged[Cdecl]<GlfwWindowHandle*, void> RequestWindowAttention;
	public static readonly delegate* unmanaged[Cdecl]<GlfwWindowHandle*, GlfwMonitor*> GetWindowMonitor;
	public static readonly delegate* unmanaged[Cdecl]<GlfwWindowHandle*, GlfwMonitor*, int, int, int, int, int, void> SetWindowMonitor;

	public static readonly delegate* unmanaged[Cdecl]<GlfwWindowHandle*, WindowHintGetter, GlfwBool> GetWindowAttrib;
	public static readonly delegate* unmanaged[Cdecl]<GlfwWindowHandle*, WindowHintSetter, GlfwBool, void> SetWindowAttrib;

	public static readonly delegate* unmanaged[Cdecl]<GlfwWindowHandle*, void*> GetWindowUserPointer;
	public static readonly delegate* unmanaged[Cdecl]<GlfwWindowHandle*, void*, void> SetWindowUserPointer;

	public static readonly delegate* unmanaged[Cdecl]<GlfwWindowHandle*, delegate* unmanaged[Cdecl]<GlfwWindowHandle*, int, int, void>, delegate* unmanaged[Cdecl]<GlfwWindowHandle*, int, int, void>> SetWindowPosCallback;
	public static readonly delegate* unmanaged[Cdecl]<GlfwWindowHandle*, delegate* unmanaged[Cdecl]<GlfwWindowHandle*, int, int, void>, delegate* unmanaged[Cdecl]<GlfwWindowHandle*, int, int, void>> SetWindowSizeCallback;
	public static readonly delegate* unmanaged[Cdecl]<GlfwWindowHandle*, delegate* unmanaged[Cdecl]<GlfwWindowHandle*, void>, delegate* unmanaged[Cdecl]<GlfwWindowHandle*, void>> SetWindowCloseCallback;
	public static readonly delegate* unmanaged[Cdecl]<GlfwWindowHandle*, delegate* unmanaged[Cdecl]<GlfwWindowHandle*, void>, delegate* unmanaged[Cdecl]<GlfwWindowHandle*, void>> SetWindowRefreshCallback;
	public static readonly delegate* unmanaged[Cdecl]<GlfwWindowHandle*, delegate* unmanaged[Cdecl]<GlfwWindowHandle*, int, void>, delegate* unmanaged[Cdecl]<GlfwWindowHandle*, int, void>> SetWindowFocusCallback;
	public static readonly delegate* unmanaged[Cdecl]<GlfwWindowHandle*, delegate* unmanaged[Cdecl]<GlfwWindowHandle*, int, void>, delegate* unmanaged[Cdecl]<GlfwWindowHandle*, int, void>> SetWindowIconifyCallback;
	public static readonly delegate* unmanaged[Cdecl]<GlfwWindowHandle*, delegate* unmanaged[Cdecl]<GlfwWindowHandle*, int, void>, delegate* unmanaged[Cdecl]<GlfwWindowHandle*, int, void>> SetWindowMaximizeCallback;
	public static readonly delegate* unmanaged[Cdecl]<GlfwWindowHandle*, delegate* unmanaged[Cdecl]<GlfwWindowHandle*, int, int, void>, delegate* unmanaged[Cdecl]<GlfwWindowHandle*, int, int, void>> SetFramebufferSizeCallback;
	public static readonly delegate* unmanaged[Cdecl]<GlfwWindowHandle*, delegate* unmanaged[Cdecl]<GlfwWindowHandle*, float, float, void>, delegate* unmanaged[Cdecl]<GlfwWindowHandle*, float, float, void>> SetWindowContentScaleCallback;
	public static readonly delegate* unmanaged[Cdecl]<void> PollEvents;
	public static readonly delegate* unmanaged[Cdecl]<void> WaitEvents;
	public static readonly delegate* unmanaged[Cdecl]<double, void> WaitEventsTimeout;
	public static readonly delegate* unmanaged[Cdecl]<double, void> PostEmptyEvent;
	public static readonly delegate* unmanaged[Cdecl]<GlfwWindowHandle*, void> SwapBuffers;
	public static readonly delegate* unmanaged[Cdecl]<int, void> SwapInterval;

	public static readonly delegate* unmanaged[Cdecl]<GlfwWindowHandle*, void> MakeContextCurrent;
	public static readonly delegate* unmanaged[Cdecl]<CString, void*> GetProcAddress;





	static Glfw()
	{
		// Initialize glfw methods
		NativeAPI.Initialize(typeof(Glfw));
	}

	private class __import : INativeLibraryContainer
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

		public LibFileSearchPath SearchType
			=> LibFileSearchPath.NearByExeFile;
	}
}