using System.Runtime.InteropServices;

namespace RawSalt.Windowing.Xlib;

/// <summary>
/// Minimal Xlib/XRandR bindings required by the platform.
/// </summary>
/// <remarks>
/// Sonames are used explicitly so the platform works without X11 development packages installed.
/// </remarks>
internal static class XlibNative
{
	internal const string X11 = "libX11.so.6";
	internal const string XRandR = "libXrandr.so.2";

	// Event types.
	internal const int KeyPress = 2;
	internal const int KeyRelease = 3;
	internal const int ButtonPress = 4;
	internal const int ButtonRelease = 5;
	internal const int MotionNotify = 6;
	internal const int EnterNotify = 7;
	internal const int LeaveNotify = 8;
	internal const int FocusIn = 9;
	internal const int FocusOut = 10;
	internal const int DestroyNotify = 17;
	internal const int ConfigureNotify = 22;
	internal const int ClientMessage = 33;

	// Modifier masks.
	internal const long ShiftMask = 1L << 0;
	internal const long LockMask = 1L << 1;
	internal const long ControlMask = 1L << 2;
	internal const long Mod1Mask = 1L << 3;
	internal const long Mod2Mask = 1L << 4;
	internal const long Mod4Mask = 1L << 6;

	// Structure notify / input event masks.
	internal const long StructureNotifyMask = 1L << 17;
	internal const long ExposureMask = 1L << 15;
	internal const long KeyPressMask = 1L << 0;
	internal const long KeyReleaseMask = 1L << 1;
	internal const long ButtonPressMask = 1L << 2;
	internal const long ButtonReleaseMask = 1L << 3;
	internal const long PointerMotionMask = 1L << 6;
	internal const long EnterWindowMask = 1L << 4;
	internal const long LeaveWindowMask = 1L << 5;
	internal const long FocusChangeMask = 1L << 21;

	// XCreateWindow valuemask.
	internal const long CWEventMask = 1L << 11;

	// Window class.
	internal const int InputOutput = 1;

	// Property mode and predefined atoms.
	internal const int PropModeReplace = 0;
	internal const ulong XA_ATOM = 4;
	internal const ulong XA_STRING = 31;

	[DllImport(X11)]
	internal static extern int XInitThreads();

	[DllImport(X11)]
	internal static extern int XkbSetDetectableAutoRepeat(IntPtr display, int detectable, out int supported);

	[DllImport(X11)]
	internal static extern IntPtr XOpenDisplay(IntPtr displayName);

	[DllImport(X11)]
	internal static extern int XCloseDisplay(IntPtr display);

	[DllImport(X11)]
	internal static extern int XDefaultScreen(IntPtr display);

	[DllImport(X11)]
	internal static extern ulong XDefaultRootWindow(IntPtr display);

	[DllImport(X11)]
	internal static extern ulong XCreateWindow(IntPtr display, ulong parent, int x, int y,
		uint width, uint height, uint borderWidth, int depth, uint @class, IntPtr visual,
		ulong valueMask, ref XSetWindowAttributes attributes);

	[DllImport(X11)]
	internal static extern int XDestroyWindow(IntPtr display, ulong window);

	[DllImport(X11)]
	internal static extern int XMapWindow(IntPtr display, ulong window);

	[DllImport(X11)]
	internal static extern int XUnmapWindow(IntPtr display, ulong window);

	[DllImport(X11)]
	internal static extern ulong XInternAtom(IntPtr display, string atomName, int onlyIfExists);

	[DllImport(X11)]
	internal static extern IntPtr XGetAtomName(IntPtr display, ulong atom);

	[DllImport(X11)]
	internal static extern int XSetWMProtocols(IntPtr display, ulong window, ref ulong protocols, int count);

	[DllImport(X11)]
	internal static extern int XStoreName(IntPtr display, ulong window, string windowName);

	[DllImport(X11)]
	internal static extern int XChangeProperty(IntPtr display, ulong window, ulong property, ulong type,
		int format, int mode, byte[] data, int elementCount);

	[DllImport(X11)]
	internal static extern int XFlush(IntPtr display);

	[DllImport(X11)]
	internal static extern int XSync(IntPtr display, int discard);

	[DllImport(X11)]
	internal static extern int XPending(IntPtr display);

	[DllImport(X11)]
	internal static extern int XNextEvent(IntPtr display, out XEvent xevent);

	[DllImport(X11)]
	internal static extern int XGetWindowAttributes(IntPtr display, ulong window, out XWindowAttributes attributes);

	[DllImport(X11)]
	internal static extern IntPtr XFree(IntPtr data);

	[DllImport(XRandR)]
	internal static extern IntPtr XRRGetMonitors(IntPtr display, ulong window, int getActive, out int monitorCount);

	[DllImport(XRandR)]
	internal static extern void XRRFreeMonitors(IntPtr monitors);
}

[StructLayout(LayoutKind.Sequential)]
internal struct XSetWindowAttributes
{
	public IntPtr background_pixmap;
	public ulong background_pixel;
	public IntPtr border_pixmap;
	public ulong border_pixel;
	public int bit_gravity;
	public int win_gravity;
	public int backing_store;
	public ulong backing_planes;
	public ulong backing_pixel;
	public int save_under;
	public long event_mask;
	public long do_not_propagate_mask;
	public int override_redirect;
	public IntPtr colormap;
	public IntPtr cursor;
}

[StructLayout(LayoutKind.Sequential)]
internal struct XWindowAttributes
{
	public int x;
	public int y;
	public int width;
	public int height;
	public int border_width;
	public int depth;
	public IntPtr visual;
	public ulong root;
	public int @class;
	public int bit_gravity;
	public int win_gravity;
	public int backing_store;
	public ulong backing_planes;
	public ulong backing_pixel;
	public int save_under;
	public ulong colormap;
	public int map_installed;
	public int map_state;
	public long all_event_masks;
	public long your_event_mask;
	public long do_not_propagate_mask;
	public int override_redirect;
	public IntPtr screen;
}

[StructLayout(LayoutKind.Sequential)]
internal struct XAnyEvent
{
	public int type;
	public ulong serial;
	public int send_event;
	public IntPtr display;
	public ulong window;
}

[StructLayout(LayoutKind.Sequential)]
internal struct XConfigureEvent
{
	public int type;
	public ulong serial;
	public int send_event;
	public IntPtr display;
	public ulong @event;
	public ulong window;
	public int x;
	public int y;
	public int width;
	public int height;
	public int border_width;
	public ulong above;
	public int override_redirect;
}

[StructLayout(LayoutKind.Sequential)]
internal struct XDestroyWindowEvent
{
	public int type;
	public ulong serial;
	public int send_event;
	public IntPtr display;
	public ulong @event;
	public ulong window;
}

[StructLayout(LayoutKind.Sequential)]
internal struct XKeyEvent
{
	public int type;
	public ulong serial;
	public int send_event;
	public IntPtr display;
	public ulong window;
	public ulong root;
	public ulong subwindow;
	public ulong time;
	public int x;
	public int y;
	public int x_root;
	public int y_root;
	public uint state;
	public uint keycode;
	public int same_screen;
}

[StructLayout(LayoutKind.Sequential)]
internal struct XButtonEvent
{
	public int type;
	public ulong serial;
	public int send_event;
	public IntPtr display;
	public ulong window;
	public ulong root;
	public ulong subwindow;
	public ulong time;
	public int x;
	public int y;
	public int x_root;
	public int y_root;
	public uint state;
	public uint button;
	public int same_screen;
}

[StructLayout(LayoutKind.Sequential)]
internal struct XMotionEvent
{
	public int type;
	public ulong serial;
	public int send_event;
	public IntPtr display;
	public ulong window;
	public ulong root;
	public ulong subwindow;
	public ulong time;
	public int x;
	public int y;
	public int x_root;
	public int y_root;
	public uint state;
	public byte is_hint;
	public int same_screen;
}

[StructLayout(LayoutKind.Sequential)]
internal struct XCrossingEvent
{
	public int type;
	public ulong serial;
	public int send_event;
	public IntPtr display;
	public ulong window;
	public ulong root;
	public ulong subwindow;
	public ulong time;
	public int x;
	public int y;
	public int x_root;
	public int y_root;
	public int mode;
	public int detail;
	public int same_screen;
	public int focus;
	public uint state;
}

[StructLayout(LayoutKind.Sequential)]
internal struct XFocusChangeEvent
{
	public int type;
	public ulong serial;
	public int send_event;
	public IntPtr display;
	public ulong window;
	public int mode;
	public int detail;
}

[StructLayout(LayoutKind.Explicit, Size = 40)]
internal struct XClientMessageData
{
	[FieldOffset(0)] public long l0;
	[FieldOffset(0)] public short s0;
	[FieldOffset(0)] public byte b0;
}

[StructLayout(LayoutKind.Sequential)]
internal struct XClientMessageEvent
{
	public int type;
	public ulong serial;
	public int send_event;
	public IntPtr display;
	public ulong window;
	public ulong message_type;
	public int format;
	public XClientMessageData data;
}

/// <summary>
/// Xlib <c>XEvent</c> is a union whose size is 24 machine words.
/// </summary>
[StructLayout(LayoutKind.Explicit, Size = 192)]
internal struct XEvent
{
	[FieldOffset(0)] public int type;
	[FieldOffset(0)] public XAnyEvent any;
	[FieldOffset(0)] public XKeyEvent key;
	[FieldOffset(0)] public XButtonEvent button;
	[FieldOffset(0)] public XMotionEvent motion;
	[FieldOffset(0)] public XCrossingEvent crossing;
	[FieldOffset(0)] public XFocusChangeEvent focus;
	[FieldOffset(0)] public XConfigureEvent configure;
	[FieldOffset(0)] public XDestroyWindowEvent destroy;
	[FieldOffset(0)] public XClientMessageEvent client;
}

[StructLayout(LayoutKind.Sequential)]
internal struct XRRMonitorInfo
{
	public ulong name;
	public int primary;
	public int automatic;
	public int noutput;
	public int x;
	public int y;
	public int width;
	public int height;
	public int mwidth;
	public int mheight;
	public IntPtr outputs;
}
