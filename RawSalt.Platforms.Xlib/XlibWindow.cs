using NiTiS.Mathematics;
using RawSalt.Input;
using RawSalt.Platforms;

namespace RawSalt.Windowing.Xlib;

public sealed class XlibWindow : Window, INativeWindow
{
	public override XlibPlatform Platform { get; }

	internal ulong Handle { get; }

	private Vector2<int> _size;
	private bool _alive = true;
	private bool _destroyed;

	public override Vector2<int> Size => _size;

	/// <summary>
	/// X11 has no per-window scale factor, so the surface size matches the window size.
	/// </summary>
	public override Vector2<int> SurfaceSize => _size;

	public override event Action<SizeChangedEventArgs>? SizeChanged;
	public override event Action<CloseRequest>? CloseRequest;

	public override bool ShouldUpdate => _alive;

	NativeWindowHandle INativeWindow.NativeHandle =>
		new(WellKnownPlatformApi.Xlib, Platform.DisplayHandle, unchecked((nint)Handle));

	internal XlibWindow(ulong handle, XlibPlatform platform)
	{
		Handle = handle;
		Platform = platform;
	}

	public override void PumpEvents() => Platform.PumpEvents();

	public void Close()
	{
		if (_destroyed)
		{
			return;
		}

		_destroyed = true;
		_alive = false;
		Platform.RemoveWindow(this);
		XlibNative.XDestroyWindow(Platform.DisplayHandle, Handle);
		XlibNative.XFlush(Platform.DisplayHandle);
	}

	public override void Hide()
	{
		XlibNative.XUnmapWindow(Platform.DisplayHandle, Handle);
		XlibNative.XFlush(Platform.DisplayHandle);
	}

	public override void Show()
	{
		XlibNative.XMapWindow(Platform.DisplayHandle, Handle);
		XlibNative.XFlush(Platform.DisplayHandle);
	}

	internal void MarkDestroyed()
	{
		_alive = false;
		_destroyed = true;
	}

	internal void ProcessEvent(in XEvent xevent)
	{
		switch (xevent.type)
		{
			case XlibNative.KeyPress:
				ProcessKey(in xevent.key, true);
				break;

			case XlibNative.KeyRelease:
				ProcessKey(in xevent.key, false);
				break;

			case XlibNative.ButtonPress:
				ProcessButton(in xevent.button, true);
				break;

			case XlibNative.ButtonRelease:
				ProcessButton(in xevent.button, false);
				break;

			case XlibNative.MotionNotify:
				OnMouseMove(new MouseMoveEventArgs(new Vector2<int>(xevent.motion.x, xevent.motion.y)));
				break;

			case XlibNative.EnterNotify:
				OnCursorEntered();
				break;

			case XlibNative.LeaveNotify:
				OnCursorLeft();
				break;

			case XlibNative.FocusIn:
				OnFocusChanged(true);
				break;

			case XlibNative.FocusOut:
				OnFocusChanged(false);
				break;

			case XlibNative.ConfigureNotify:
				UpdateSize();
				break;

			case XlibNative.ClientMessage:
				if (xevent.client.message_type == Platform.WmProtocolsAtom &&
				    xevent.client.format == 32 &&
				    unchecked((ulong)xevent.client.data.l0) == Platform.WmDeleteWindowAtom)
				{
					OnCloseRequested();
				}

				break;

			case XlibNative.DestroyNotify:
				_alive = false;
				break;
		}
	}

	private void ProcessKey(in XKeyEvent keyEvent, bool pressed)
	{
		// Linux X11 key codes are evdev codes offset by 8.
		int evdevCode = (int)keyEvent.keycode - 8;
		if (!LinuxKeyMap.TryGetKey(evdevCode, out Key key))
		{
			return;
		}

		KeyModifiers modifiers = GetModifiers(keyEvent.state);
		if (pressed)
		{
			OnKeyDown(new KeyEventArgs(key, modifiers, IsKeyDown(key)));
		}
		else
		{
			OnKeyUp(new KeyEventArgs(key, modifiers));
		}
	}

	private void ProcessButton(in XButtonEvent buttonEvent, bool pressed)
	{
		Vector2<int> position = new(buttonEvent.x, buttonEvent.y);

		if (pressed && TryGetWheelDelta(buttonEvent.button, out Vector2<int> delta))
		{
			OnMouseMove(new MouseMoveEventArgs(position));
			OnMouseWheel(new MouseWheelEventArgs(delta));
			return;
		}

		if (!TryGetMouseButton(buttonEvent.button, out MouseButton button))
		{
			return;
		}

		MouseButtonEventArgs args = new(button, position, GetModifiers(buttonEvent.state));
		if (pressed)
		{
			OnMouseDown(args);
		}
		else
		{
			OnMouseUp(args);
		}
	}

	private static bool TryGetMouseButton(uint x11Button, out MouseButton button)
	{
		switch (x11Button)
		{
			case 1: button = MouseButton.Left; return true;
			case 2: button = MouseButton.Middle; return true;
			case 3: button = MouseButton.Right; return true;
			case 8: button = MouseButton.Back; return true;
			case 9: button = MouseButton.Forward; return true;
			default: button = default; return false;
		}
	}

	private static bool TryGetWheelDelta(uint x11Button, out Vector2<int> delta)
	{
		switch (x11Button)
		{
			case 4: delta = new(0, 1); return true;
			case 5: delta = new(0, -1); return true;
			case 6: delta = new(-1, 0); return true;
			case 7: delta = new(1, 0); return true;
			default: delta = default; return false;
		}
	}

	private static KeyModifiers GetModifiers(uint state)
	{
		KeyModifiers modifiers = KeyModifiers.None;
		long mask = state;

		if ((mask & XlibNative.ShiftMask) != 0) modifiers |= KeyModifiers.Shift;
		if ((mask & XlibNative.ControlMask) != 0) modifiers |= KeyModifiers.Control;
		if ((mask & XlibNative.Mod1Mask) != 0) modifiers |= KeyModifiers.Alt;
		if ((mask & XlibNative.Mod4Mask) != 0) modifiers |= KeyModifiers.Super;
		if ((mask & XlibNative.LockMask) != 0) modifiers |= KeyModifiers.CapsLock;
		if ((mask & XlibNative.Mod2Mask) != 0) modifiers |= KeyModifiers.NumLock;

		return modifiers;
	}

	internal void UpdateSize()
	{
		if (XlibNative.XGetWindowAttributes(Platform.DisplayHandle, Handle, out XWindowAttributes attributes) == 0)
		{
			return;
		}

		Vector2<int> size = new(attributes.width, attributes.height);
		if (_size == size)
		{
			return;
		}

		_size = size;
		SizeChanged?.Invoke(new SizeChangedEventArgs(size, size));
	}

	private void OnCloseRequested()
	{
		if (CloseRequest is null)
		{
			_alive = false;
			return;
		}

		CloseRequest request = new();
		CloseRequest.Invoke(request);

		_alive = !request.ShouldClose;
	}
}
