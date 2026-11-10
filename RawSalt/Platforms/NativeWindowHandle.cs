namespace RawSalt.Platforms;

/// <summary>
/// Platform-neutral description of a native window that a graphics API can create a surface from.
/// </summary>
/// <param name="Api">Windowing API that owns the handles.</param>
/// <param name="Connection">
/// Connection the window belongs to: <c>HINSTANCE</c> (Win32), <c>Display*</c> (Xlib)
/// or <c>wl_display*</c> (Wayland).
/// </param>
/// <param name="Window">
/// Native window handle: <c>HWND</c> (Win32), <c>Window</c> XID (Xlib) or <c>wl_surface*</c> (Wayland).
/// </param>
public readonly record struct NativeWindowHandle(WellKnownPlatformApi Api, nint Connection, nint Window);
