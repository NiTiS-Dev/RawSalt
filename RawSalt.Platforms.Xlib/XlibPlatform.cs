using System.Runtime.InteropServices;
using System.Text;
using NiTiS.Mathematics;
using RawSalt.Logging;
using RawSalt.Platforms;

namespace RawSalt.Windowing.Xlib;

public sealed class XlibPlatform : Platform
{
	public override WellKnownPlatformApi Api => WellKnownPlatformApi.Xlib;

	internal IntPtr DisplayHandle { get; private set; }
	internal ulong RootWindow { get; }
	internal ulong WmProtocolsAtom { get; }
	internal ulong WmDeleteWindowAtom { get; }

	private readonly ulong _netWmNameAtom;
	private readonly ulong _utf8StringAtom;
	private readonly Dictionary<ulong, XlibWindow> _windows = [];
	private List<XlibDisplay>? _displays;

	public XlibPlatform(Application application) : base(application)
	{
		EnsureThreadsInitialized();

		DisplayHandle = XlibNative.XOpenDisplay(IntPtr.Zero);
		if (DisplayHandle == IntPtr.Zero)
		{
			throw new InvalidOperationException("Failed to open X display. Make sure the DISPLAY environment variable is set.");
		}

		RootWindow = XlibNative.XDefaultRootWindow(DisplayHandle);
		WmProtocolsAtom = InternAtom("WM_PROTOCOLS");
		WmDeleteWindowAtom = InternAtom("WM_DELETE_WINDOW");
		_netWmNameAtom = InternAtom("_NET_WM_NAME");
		_utf8StringAtom = InternAtom("UTF8_STRING");
	}

	public static bool IsAvailable()
	{
		try
		{
			EnsureThreadsInitialized();
			IntPtr display = XlibNative.XOpenDisplay(IntPtr.Zero);
			if (display == IntPtr.Zero)
			{
				return false;
			}

			XlibNative.XCloseDisplay(display);
			return true;
		}
		catch (DllNotFoundException)
		{
			return false;
		}
		catch (EntryPointNotFoundException)
		{
			return false;
		}
	}

	private static bool _threadsInitialized;
	private static readonly Lock ThreadsLock = new();

	private static void EnsureThreadsInitialized()
	{
		if (_threadsInitialized)
		{
			return;
		}

		lock (ThreadsLock)
		{
			if (_threadsInitialized)
			{
				return;
			}

			XlibNative.XInitThreads();
			_threadsInitialized = true;
		}
	}

	protected override void Dispose(bool disposing)
	{
		foreach (XlibWindow window in _windows.Values.ToArray())
		{
			window.Close();
		}

		_windows.Clear();

		if (DisplayHandle != IntPtr.Zero)
		{
			XlibNative.XCloseDisplay(DisplayHandle);
			DisplayHandle = IntPtr.Zero;
		}
	}

	public override IReadOnlyList<Display> GetDisplays()
	{
		if (_displays == null)
		{
			_displays = [];
		}
		else
		{
			_displays.Clear();
		}

		ReadMonitors();

		if (_displays.Count == 0)
		{
			// XRandR is unavailable or reported nothing; fall back to the root window.
			if (XlibNative.XGetWindowAttributes(DisplayHandle, RootWindow, out XWindowAttributes attributes) != 0)
			{
				_displays.Add(new XlibDisplay("Default Display",
					new Region2<int>(0, 0, attributes.width, attributes.height)));
			}
		}

		return _displays;
	}

	private void ReadMonitors()
	{
		IntPtr monitors = IntPtr.Zero;
		try
		{
			monitors = XlibNative.XRRGetMonitors(DisplayHandle, RootWindow, 1, out int count);
			if (monitors == IntPtr.Zero || count <= 0)
			{
				return;
			}

			int stride = Marshal.SizeOf<XRRMonitorInfo>();
			for (int i = 0; i < count; i++)
			{
				XRRMonitorInfo info = Marshal.PtrToStructure<XRRMonitorInfo>(IntPtr.Add(monitors, i * stride));
				string name = ResolveMonitorName(info.name, i);
				_displays!.Add(new XlibDisplay(name, new Region2<int>(info.x, info.y, info.width, info.height)));
			}
		}
		catch (DllNotFoundException)
		{
			Log.Warning("Failed to enumerate X11 monitors: libXrandr is not available.");
		}
		catch (EntryPointNotFoundException)
		{
			Log.Warning("Failed to enumerate X11 monitors: XRRGetMonitors is not available.");
		}
		finally
		{
			if (monitors != IntPtr.Zero)
			{
				XlibNative.XRRFreeMonitors(monitors);
			}
		}
	}

	private string ResolveMonitorName(ulong atom, int index)
	{
		if (atom != 0)
		{
			IntPtr name = XlibNative.XGetAtomName(DisplayHandle, atom);
			if (name != IntPtr.Zero)
			{
				try
				{
					string? resolved = Marshal.PtrToStringAnsi(name);
					if (!string.IsNullOrEmpty(resolved))
					{
						return resolved;
					}
				}
				finally
				{
					XlibNative.XFree(name);
				}
			}
		}

		return $"Display {index + 1}";
	}

	public override XlibWindow CreateView(in WindowConfiguration configuration)
	{
		int width = Math.Max(1, configuration.Size.X);
		int height = Math.Max(1, configuration.Size.Y);

		int x;
		int y;
		if (configuration.Position is Vector2<int> position)
		{
			x = position.X;
			y = position.Y;
		}
		else
		{
			XlibNative.XGetWindowAttributes(DisplayHandle, RootWindow, out XWindowAttributes screen);
			x = Math.Max(0, (screen.width - width) / 2);
			y = Math.Max(0, (screen.height - height) / 2);
		}

		XSetWindowAttributes attributes = new()
		{
			event_mask = XlibNative.StructureNotifyMask |
			             XlibNative.ExposureMask |
			             XlibNative.KeyPressMask |
			             XlibNative.KeyReleaseMask |
			             XlibNative.ButtonPressMask |
			             XlibNative.ButtonReleaseMask |
			             XlibNative.PointerMotionMask |
			             XlibNative.EnterWindowMask |
			             XlibNative.LeaveWindowMask |
			             XlibNative.FocusChangeMask
		};

		ulong handle = XlibNative.XCreateWindow(DisplayHandle, RootWindow, x, y, (uint)width, (uint)height,
			0, 0, XlibNative.InputOutput, IntPtr.Zero, XlibNative.CWEventMask, ref attributes);

		if (handle == 0)
		{
			throw new InvalidOperationException("Failed to create X11 window.");
		}

		ulong deleteWindow = WmDeleteWindowAtom;
		XlibNative.XSetWMProtocols(DisplayHandle, handle, ref deleteWindow, 1);
		SetTitle(handle, configuration.Title);

		XlibWindow window = new(handle, this);
		_windows.Add(handle, window);
		window.UpdateSize();
		XlibNative.XFlush(DisplayHandle);

		return window;
	}

	private void SetTitle(ulong handle, string title)
	{
		XlibNative.XStoreName(DisplayHandle, handle, title);

		byte[] utf8 = Encoding.UTF8.GetBytes(title);
		XlibNative.XChangeProperty(DisplayHandle, handle, _netWmNameAtom, _utf8StringAtom,
			8, XlibNative.PropModeReplace, utf8, utf8.Length);
	}

	public override bool ReleaseView(View view)
	{
		if (view is not XlibWindow window)
		{
			return false;
		}

		window.Close();
		return true;
	}

	public override void PumpEvents()
	{
		if (DisplayHandle == IntPtr.Zero)
		{
			return;
		}

		while (XlibNative.XPending(DisplayHandle) > 0)
		{
			if (XlibNative.XNextEvent(DisplayHandle, out XEvent xevent) != 0)
			{
				break;
			}

			if (xevent.type == XlibNative.DestroyNotify)
			{
				if (_windows.Remove(xevent.destroy.window, out XlibWindow? destroyed))
				{
					destroyed.MarkDestroyed();
				}

				continue;
			}

			if (_windows.TryGetValue(xevent.any.window, out XlibWindow? window))
			{
				window.ProcessEvent(in xevent);
			}
		}
	}

	public override bool TryCreateErrorMessage(string message)
	{
		Console.Error.WriteLine(message);
		return true;
	}

	internal void RemoveWindow(XlibWindow window)
	{
		_windows.Remove(window.Handle);
	}

	private ulong InternAtom(string name)
	{
		return XlibNative.XInternAtom(DisplayHandle, name, 0);
	}
}
