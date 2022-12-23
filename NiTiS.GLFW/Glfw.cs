using NiTiS.GLFW.Enums;
using NiTiS.Native;

namespace NiTiS.GLFW;

[NativeAPI(MethodPrefix = "glfw")]
public static unsafe partial class Glfw
{
	public const int DontCare = -1;

	public static readonly delegate* unmanaged[Stdcall]<GlfwBool> Init;
	public static readonly delegate* unmanaged[Stdcall]<void> Terminate;
	public static readonly delegate* unmanaged[Stdcall]<GlfwInitHint, int, void> InitHint;
	public static readonly delegate* unmanaged[Stdcall]<int*, int*, int*, void> GetVersion;
	public static readonly delegate* unmanaged[Stdcall]<CString> GetVersionString;
	public static readonly delegate* unmanaged[Stdcall]<CString*, GlfwError> GetError;

	public static readonly delegate* unmanaged[Stdcall]<delegate* unmanaged[Cdecl]<GlfwError, CString, void>, delegate* unmanaged[Cdecl]<GlfwError, CString, void>> SetErrorCallback;

	public static readonly delegate* unmanaged[Stdcall]<int*, GlfwMonitorHandle**> GetMonitors;
	public static readonly delegate* unmanaged[Stdcall]<GlfwMonitorHandle*> GetPrimaryMonitor;
	public static readonly delegate* unmanaged[Stdcall]<GlfwMonitorHandle*, int*, int*, void> GetMonitorPos;
	public static readonly delegate* unmanaged[Stdcall]<GlfwMonitorHandle*, int*, int*, int*, int*, void> GetMonitorWorkarea;
	public static readonly delegate* unmanaged[Stdcall]<GlfwMonitorHandle*, int*, int*, void> GetMonitorPhysicalSize;
	public static readonly delegate* unmanaged[Stdcall]<GlfwMonitorHandle*, float*, float*, void> GetMonitorContentScale;
	public static readonly delegate* unmanaged[Stdcall]<GlfwMonitorHandle*, CString> GetMonitorName;
	public static readonly delegate* unmanaged[Stdcall]<GlfwMonitorHandle*, void*, void> SetMonitorUserPointer;
	public static readonly delegate* unmanaged[Stdcall]<GlfwMonitorHandle*, void*> GetMonitorUserPointer;

	public static readonly delegate* unmanaged[Stdcall]<delegate* unmanaged[Cdecl]<GlfwMonitorHandle*, GlfwEvent, void>, delegate* unmanaged[Cdecl]<GlfwMonitorHandle*, GlfwEvent, void>> SetMonitorCallback;

	public static readonly delegate* unmanaged[Stdcall]<GlfwMonitorHandle*, int*, GlfwVideoMode*> GetVideoModes;
	public static readonly delegate* unmanaged[Stdcall]<GlfwMonitorHandle*, GlfwVideoMode*> GetVideoMode;
	public static readonly delegate* unmanaged[Stdcall]<GlfwMonitorHandle*, float, void> SetGamma;
	public static readonly delegate* unmanaged[Stdcall]<GlfwMonitorHandle*, GlfwGammaRamp*> GetGammaRamp;
	public static readonly delegate* unmanaged[Stdcall]<GlfwMonitorHandle*, GlfwGammaRamp*, void> SetGammaRamp;


	public static readonly delegate* unmanaged[Stdcall]<void> DefaultWindowHints;
	public static readonly delegate* unmanaged[Stdcall]<int, int, void> WindowHint;
	public static readonly delegate* unmanaged[Stdcall]<WindowHintString, CString, void> WindowHintString;
	public static readonly delegate* unmanaged[Stdcall]<int, int, CString, GlfwMonitorHandle*, GlfwWindowHandle*, GlfwWindowHandle*> CreateWindow;
	public static readonly delegate* unmanaged[Stdcall]<GlfwWindowHandle*, void> DestroyWindow;
	public static readonly delegate* unmanaged[Stdcall]<GlfwWindowHandle*, int> WindowShouldClose;
	public static readonly delegate* unmanaged[Stdcall]<GlfwWindowHandle*, int, void> SetWindowShouldClose;
	public static readonly delegate* unmanaged[Stdcall]<GlfwWindowHandle*, CString, void> SetWindowTitle;
	public static readonly delegate* unmanaged[Stdcall]<GlfwWindowHandle*, CString> GetWindowTitle;
	public static readonly delegate* unmanaged[Stdcall]<GlfwWindowHandle*, int, GlfwImage*, void> SetWindowIcon;
	public static readonly delegate* unmanaged[Stdcall]<GlfwWindowHandle*, int*, int*, void> GetWindowPos;
	public static readonly delegate* unmanaged[Stdcall]<GlfwWindowHandle*, int, int, void> SetWindowPos;
	public static readonly delegate* unmanaged[Stdcall]<GlfwWindowHandle*, int*, int*, void> GetWindowSize;
	public static readonly delegate* unmanaged[Stdcall]<GlfwWindowHandle*, int, int, void> SetWindowSize;
	public static readonly delegate* unmanaged[Stdcall]<GlfwWindowHandle*, int, int, int, int, void> SetWindowSizeLimits;
	public static readonly delegate* unmanaged[Stdcall]<GlfwWindowHandle*, int, int, void> SetWindowAspectRatio;
	public static readonly delegate* unmanaged[Stdcall]<GlfwWindowHandle*, int*, int*, void> GetFramebufferSize;
	public static readonly delegate* unmanaged[Stdcall]<GlfwWindowHandle*, int*, int*, int*, int*, void> GetWindowFrameSize;
	public static readonly delegate* unmanaged[Stdcall]<GlfwWindowHandle*, float*, float*, void> GetWindowContentScale;
	public static readonly delegate* unmanaged[Stdcall]<GlfwWindowHandle*, float> GetWindowOpacity;
	public static readonly delegate* unmanaged[Stdcall]<GlfwWindowHandle*, float, void> SetWindowOpacity;
	public static readonly delegate* unmanaged[Stdcall]<GlfwWindowHandle*, void> IconifyWindow;
	public static readonly delegate* unmanaged[Stdcall]<GlfwWindowHandle*, void> RestoreWindow;
	public static readonly delegate* unmanaged[Stdcall]<GlfwWindowHandle*, void> MaximizeWindow;
	public static readonly delegate* unmanaged[Stdcall]<GlfwWindowHandle*, void> ShowWindow;
	public static readonly delegate* unmanaged[Stdcall]<GlfwWindowHandle*, void> HideWindow;
	public static readonly delegate* unmanaged[Stdcall]<GlfwWindowHandle*, void> FocusWindow;
	public static readonly delegate* unmanaged[Stdcall]<GlfwWindowHandle*, void> RequestWindowAttention;
	public static readonly delegate* unmanaged[Stdcall]<GlfwWindowHandle*, GlfwMonitorHandle*> GetWindowMonitor;
	public static readonly delegate* unmanaged[Stdcall]<GlfwWindowHandle*, GlfwMonitorHandle*, int, int, int, int, int, void> SetWindowMonitor;

	public static readonly delegate* unmanaged[Stdcall]<GlfwWindowHandle*, WindowHintGetter, GlfwBool> GetWindowAttrib;
	public static readonly delegate* unmanaged[Stdcall]<GlfwWindowHandle*, WindowHintSetter, GlfwBool, void> SetWindowAttrib;

	public static readonly delegate* unmanaged[Stdcall]<GlfwWindowHandle*, void*> GetWindowUserPointer;
	public static readonly delegate* unmanaged[Stdcall]<GlfwWindowHandle*, void*, void> SetWindowUserPointer;

	public static readonly delegate* unmanaged[Stdcall]<GlfwWindowHandle*, delegate* unmanaged[Stdcall]<GlfwWindowHandle*, int, int, void>, delegate* unmanaged[Stdcall]<GlfwWindowHandle*, int, int, void>> SetWindowPosCallback;
	public static readonly delegate* unmanaged[Stdcall]<GlfwWindowHandle*, delegate* unmanaged[Stdcall]<GlfwWindowHandle*, int, int, void>, delegate* unmanaged[Stdcall]<GlfwWindowHandle*, int, int, void>> SetWindowSizeCallback;
	public static readonly delegate* unmanaged[Stdcall]<GlfwWindowHandle*, delegate* unmanaged[Stdcall]<GlfwWindowHandle*, void>, delegate* unmanaged[Stdcall]<GlfwWindowHandle*, void>> SetWindowCloseCallback;
	public static readonly delegate* unmanaged[Stdcall]<GlfwWindowHandle*, delegate* unmanaged[Stdcall]<GlfwWindowHandle*, void>, delegate* unmanaged[Stdcall]<GlfwWindowHandle*, void>> SetWindowRefreshCallback;
	public static readonly delegate* unmanaged[Stdcall]<GlfwWindowHandle*, delegate* unmanaged[Stdcall]<GlfwWindowHandle*, GlfwBool, void>, delegate* unmanaged[Stdcall]<GlfwWindowHandle*, GlfwBool, void>> SetWindowFocusCallback;
	public static readonly delegate* unmanaged[Stdcall]<GlfwWindowHandle*, delegate* unmanaged[Stdcall]<GlfwWindowHandle*, GlfwBool, void>, delegate* unmanaged[Stdcall]<GlfwWindowHandle*, GlfwBool, void>> SetWindowIconifyCallback;
	public static readonly delegate* unmanaged[Stdcall]<GlfwWindowHandle*, delegate* unmanaged[Stdcall]<GlfwWindowHandle*, GlfwBool, void>, delegate* unmanaged[Stdcall]<GlfwWindowHandle*, GlfwBool, void>> SetWindowMaximizeCallback;
	public static readonly delegate* unmanaged[Stdcall]<GlfwWindowHandle*, delegate* unmanaged[Stdcall]<GlfwWindowHandle*, int, int, void>, delegate* unmanaged[Stdcall]<GlfwWindowHandle*, int, int, void>> SetFramebufferSizeCallback;
	public static readonly delegate* unmanaged[Stdcall]<GlfwWindowHandle*, delegate* unmanaged[Stdcall]<GlfwWindowHandle*, float, float, void>, delegate* unmanaged[Stdcall]<GlfwWindowHandle*, float, float, void>> SetWindowContentScaleCallback;
	public static readonly delegate* unmanaged[Stdcall]<GlfwWindowHandle*, delegate* unmanaged[Stdcall]<GlfwWindowHandle*, int, CString*>, delegate* unmanaged[Stdcall]<GlfwWindowHandle*, int, CString*>> SetDropCallback;
	public static readonly delegate* unmanaged[Stdcall]<void> PollEvents;
	public static readonly delegate* unmanaged[Stdcall]<void> WaitEvents;
	public static readonly delegate* unmanaged[Stdcall]<double, void> WaitEventsTimeout;
	public static readonly delegate* unmanaged[Stdcall]<double, void> PostEmptyEvent;
	public static readonly delegate* unmanaged[Stdcall]<GlfwWindowHandle*, void> SwapBuffers;
	public static readonly delegate* unmanaged[Stdcall]<int, void> SwapInterval;

	public static readonly delegate* unmanaged[Stdcall]<GlfwWindowHandle*, void> MakeContextCurrent;
	public static readonly delegate* unmanaged[Stdcall]<CString, void*> GetProcAddress;





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