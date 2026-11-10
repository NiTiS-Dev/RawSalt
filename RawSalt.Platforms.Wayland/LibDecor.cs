using System.Runtime.InteropServices;
using NWayland.Protocols.Wayland;
using RawSalt.Logging;

namespace RawSalt.Windowing.Wayland;

/// <summary>
/// Minimal P/Invoke bindings for libdecor 0.2.x.
/// </summary>
/// <remarks>
/// libdecor creates the <c>xdg_surface</c>/<c>xdg_toplevel</c> pair for a content surface and draws
/// client-side decorations using subsurfaces (or server-side decorations when the compositor
/// supports them). This mirrors the integration used by GLFW and SDL.
/// </remarks>
internal static class LibDecorNative
{
	private const string Library = "libdecor-0.so.0";

	internal const int WindowStateNone = 0;
	internal const int WindowStateActive = 1 << 0;
	internal const int WindowStateMaximized = 1 << 1;
	internal const int WindowStateFullscreen = 1 << 2;
	internal const int WindowStateTiledLeft = 1 << 3;
	internal const int WindowStateTiledRight = 1 << 4;
	internal const int WindowStateTiledTop = 1 << 5;
	internal const int WindowStateTiledBottom = 1 << 6;
	internal const int WindowStateSuspended = 1 << 7;

	[StructLayout(LayoutKind.Sequential)]
	internal struct Interface
	{
		public IntPtr Error;
		public IntPtr Reserved0;
		public IntPtr Reserved1;
		public IntPtr Reserved2;
		public IntPtr Reserved3;
		public IntPtr Reserved4;
		public IntPtr Reserved5;
		public IntPtr Reserved6;
		public IntPtr Reserved7;
		public IntPtr Reserved8;
		public IntPtr Reserved9;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct FrameInterface
	{
		public IntPtr Configure;
		public IntPtr Close;
		public IntPtr Commit;
		public IntPtr DismissPopup;
		public IntPtr Reserved0;
		public IntPtr Reserved1;
		public IntPtr Reserved2;
		public IntPtr Reserved3;
		public IntPtr Reserved4;
		public IntPtr Reserved5;
		public IntPtr Reserved6;
		public IntPtr Reserved7;
		public IntPtr Reserved8;
		public IntPtr Reserved9;
	}

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void ErrorCallback(IntPtr context, int error, IntPtr message);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void ConfigureCallback(IntPtr frame, IntPtr configuration, IntPtr userData);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void CloseCallback(IntPtr frame, IntPtr userData);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void CommitCallback(IntPtr frame, IntPtr userData);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void DismissPopupCallback(IntPtr frame, IntPtr seatName, IntPtr userData);

	[DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
	internal static extern IntPtr libdecor_new(IntPtr display, IntPtr iface);

	[DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
	internal static extern void libdecor_unref(IntPtr context);

	[DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
	internal static extern int libdecor_dispatch(IntPtr context, int timeout);

	[DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
	internal static extern IntPtr libdecor_decorate(IntPtr context, IntPtr surface, IntPtr iface,
		IntPtr userData);

	[DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
	internal static extern void libdecor_frame_unref(IntPtr frame);

	[DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
	internal static extern void libdecor_frame_set_app_id(IntPtr frame,
		[MarshalAs(UnmanagedType.LPUTF8Str)] string appId);

	[DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
	internal static extern void libdecor_frame_set_title(IntPtr frame,
		[MarshalAs(UnmanagedType.LPUTF8Str)] string title);

	[DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
	internal static extern void libdecor_frame_set_min_content_size(IntPtr frame, int width, int height);

	[DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
	internal static extern void libdecor_frame_set_max_content_size(IntPtr frame, int width, int height);

	[DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
	internal static extern void libdecor_frame_commit(IntPtr frame, IntPtr state, IntPtr configuration);

	[DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
	internal static extern void libdecor_frame_map(IntPtr frame);

	[DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
	internal static extern IntPtr libdecor_state_new(int width, int height);

	[DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
	internal static extern void libdecor_state_free(IntPtr state);

	[DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.I1)]
	internal static extern bool libdecor_configuration_get_content_size(IntPtr configuration,
		IntPtr frame, out int width, out int height);

	internal static bool IsAvailable()
	{
		if (!NativeLibrary.TryLoad(Library, out IntPtr handle))
		{
			return false;
		}

		try
		{
			return NativeLibrary.TryGetExport(handle, "libdecor_new", out _)
				&& NativeLibrary.TryGetExport(handle, "libdecor_decorate", out _)
				&& NativeLibrary.TryGetExport(handle, "libdecor_frame_commit", out _)
				&& NativeLibrary.TryGetExport(handle, "libdecor_frame_map", out _)
				&& NativeLibrary.TryGetExport(handle, "libdecor_frame_unref", out _);
		}
		finally
		{
			NativeLibrary.Free(handle);
		}
	}

	internal static IntPtr AllocInterface(ErrorCallback error)
	{
		Interface value = new()
		{
			Error = Marshal.GetFunctionPointerForDelegate(error)
		};

		return StructToPointer(value);
	}

	internal static IntPtr AllocFrameInterface(ConfigureCallback configure, CloseCallback close,
		CommitCallback commit, DismissPopupCallback dismissPopup)
	{
		FrameInterface value = new()
		{
			Configure = Marshal.GetFunctionPointerForDelegate(configure),
			Close = Marshal.GetFunctionPointerForDelegate(close),
			Commit = Marshal.GetFunctionPointerForDelegate(commit),
			DismissPopup = Marshal.GetFunctionPointerForDelegate(dismissPopup)
		};

		return StructToPointer(value);
	}

	private static IntPtr StructToPointer<T>(T value) where T : struct
	{
		IntPtr pointer = Marshal.AllocHGlobal(Marshal.SizeOf<T>());
		Marshal.StructureToPtr(value, pointer, false);
		return pointer;
	}
}

/// <summary>
/// Owns a libdecor context bound to the platform <see cref="WlDisplay"/>.
/// </summary>
internal sealed class LibDecorContext : IDisposable
{
	private readonly WlDisplay _display;
	private readonly LibDecorNative.ErrorCallback _errorCallback;
	private readonly WlCallback.Listener.Relay _readyListener;
	private readonly IntPtr _interface;
	private WlCallback? _readyCallback;
	private IntPtr _context;
	private bool _ready;

	private LibDecorContext(WlDisplay display)
	{
		_display = display;
		_errorCallback = OnError;
		_interface = LibDecorNative.AllocInterface(_errorCallback);
		_readyListener = new WlCallback.Listener.Relay
		{
			OnDone = OnReady
		};
	}

	public IntPtr Handle => _context;
	public bool IsReady => _ready;

	public static LibDecorContext? TryCreate(WlDisplay display)
	{
		if (!LibDecorNative.IsAvailable())
		{
			return null;
		}

		LibDecorContext context = new(display);

		try
		{
			context._context = LibDecorNative.libdecor_new(display.Handle, context._interface);

			if (context._context == IntPtr.Zero)
			{
				Log.Warning("libdecor: failed to create context.");
				context.Dispose();
				return null;
			}

			// Start libdecor initialization and remember when it finished requesting its globals.
			LibDecorNative.libdecor_dispatch(context._context, 0);
			context._readyCallback = display.Sync(context._readyListener, null!);
			return context;
		}
		catch (Exception ex) when (ex is DllNotFoundException or EntryPointNotFoundException)
		{
			Log.Warning($"libdecor: unavailable ({ex.Message}).");
			context.Dispose();
			return null;
		}
	}

	/// <summary>
	/// Drives libdecor (and the display) until it has finished initialization.
	/// </summary>
	public bool WaitUntilReady(int timeoutMilliseconds = 2000)
	{
		if (_ready)
		{
			return true;
		}

		if (_context == IntPtr.Zero)
		{
			return false;
		}

		long deadline = Environment.TickCount64 + timeoutMilliseconds;

		while (!_ready)
		{
			// libdecor must be dispatched before the display is read, as in GLFW.
			LibDecorNative.libdecor_dispatch(_context, 0);
			_display.Roundtrip();

			if (Environment.TickCount64 >= deadline)
			{
				break;
			}
		}

		if (!_ready)
		{
			Log.Warning("libdecor: initialization did not complete; falling back to xdg-shell.");
		}

		return _ready;
	}

	public int Dispatch(int timeout)
	{
		return _context == IntPtr.Zero ? 0 : LibDecorNative.libdecor_dispatch(_context, timeout);
	}

	public LibDecorFrame? Decorate(WlSurface surface, WaylandWindow window, string title,
		string appId)
	{
		if (!_ready)
		{
			return null;
		}

		return LibDecorFrame.Create(this, surface, window, title, appId);
	}

	private void OnError(IntPtr context, int error, IntPtr message)
	{
		string text = Marshal.PtrToStringUTF8(message) ?? "unknown error";
		Log.Error($"libdecor error {error}: {text}");
	}

	private void OnReady(WlCallback sender, uint callbackData)
	{
		_ready = true;
	}

	public void Dispose()
	{
		if (_context != IntPtr.Zero)
		{
			LibDecorNative.libdecor_unref(_context);
			_context = IntPtr.Zero;
		}

		_readyCallback?.Dispose();
		_readyCallback = null;

		Marshal.FreeHGlobal(_interface);
	}
}

/// <summary>
/// A libdecor frame that decorates one <see cref="WlSurface"/>.
/// </summary>
internal sealed class LibDecorFrame : IDisposable
{
	private readonly WaylandWindow _window;
	private readonly LibDecorNative.ConfigureCallback _configureCallback;
	private readonly LibDecorNative.CloseCallback _closeCallback;
	private readonly LibDecorNative.CommitCallback _commitCallback;
	private readonly LibDecorNative.DismissPopupCallback _dismissPopupCallback;
	private readonly GCHandle _userData;
	private readonly IntPtr _interface;
	private IntPtr _frame;

	private LibDecorFrame(WaylandWindow window)
	{
		_window = window;
		_configureCallback = OnConfigure;
		_closeCallback = OnClose;
		_commitCallback = OnCommit;
		_dismissPopupCallback = OnDismissPopup;
		_userData = GCHandle.Alloc(this);
		_interface = LibDecorNative.AllocFrameInterface(_configureCallback, _closeCallback,
			_commitCallback, _dismissPopupCallback);
	}

	public IntPtr Handle => _frame;

	public static LibDecorFrame? Create(LibDecorContext context, WlSurface surface,
		WaylandWindow window, string title, string appId)
	{
		LibDecorFrame frame = new(window);

		frame._frame = LibDecorNative.libdecor_decorate(context.Handle, surface.Handle,
			frame._interface, GCHandle.ToIntPtr(frame._userData));

		if (frame._frame == IntPtr.Zero)
		{
			Log.Warning("libdecor: failed to decorate the surface.");
			frame.Dispose();
			return null;
		}

		LibDecorNative.libdecor_frame_set_app_id(frame._frame, appId);
		LibDecorNative.libdecor_frame_set_title(frame._frame, title);
		LibDecorNative.libdecor_frame_set_min_content_size(frame._frame, 1, 1);
		return frame;
	}

	public void Map()
	{
		if (_frame != IntPtr.Zero)
		{
			LibDecorNative.libdecor_frame_map(_frame);
		}
	}

	/// <summary>
	/// Applies a configuration received from libdecor.
	/// </summary>
	internal void Commit(int width, int height, IntPtr configuration)
	{
		if (_frame == IntPtr.Zero)
		{
			return;
		}

		IntPtr state = LibDecorNative.libdecor_state_new(width, height);
		LibDecorNative.libdecor_frame_commit(_frame, state, configuration);
		LibDecorNative.libdecor_state_free(state);
	}

	private void OnConfigure(IntPtr frame, IntPtr configuration, IntPtr userData)
	{
		int width = 0;
		int height = 0;

		if (!LibDecorNative.libdecor_configuration_get_content_size(configuration, frame,
				out width, out height))
		{
			width = _window.Size.X;
			height = _window.Size.Y;
		}

		_window.ProcessLibDecorConfigure(width, height, configuration);
	}

	private void OnClose(IntPtr frame, IntPtr userData)
	{
		_window.ProcessCloseRequest();
	}

	private void OnCommit(IntPtr frame, IntPtr userData)
	{
		_window.ProcessLibDecorCommit();
	}

	private void OnDismissPopup(IntPtr frame, IntPtr seatName, IntPtr userData)
	{
		// The engine has no popups yet.
	}

	public void Dispose()
	{
		if (_frame != IntPtr.Zero)
		{
			LibDecorNative.libdecor_frame_unref(_frame);
			_frame = IntPtr.Zero;
		}

		if (_userData.IsAllocated)
		{
			_userData.Free();
		}

		Marshal.FreeHGlobal(_interface);
	}
}
