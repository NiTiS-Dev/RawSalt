using NWayland;
using NWayland.Interop;
using NWayland.Protocols.CursorShapeV1;
using NWayland.Protocols.FractionalScaleV1;
using NWayland.Protocols.TextInputUnstableV3;
using NWayland.Protocols.Viewporter;
using NWayland.Protocols.Wayland;
using NWayland.Protocols.XdgDecorationUnstableV1;
using NWayland.Protocols.XdgShell;
using NiTiS.Mathematics;
using RawSalt.Input;
using RawSalt.Logging;
using RawSalt.Platforms;

namespace RawSalt.Windowing.Wayland;

public sealed class WaylandPlatform : Platform
{
	public override WellKnownPlatformApi Api => WellKnownPlatformApi.Wayland;

	internal IntPtr DisplayHandle => _display.Handle;
	internal WlCompositor Compositor { get; private set; } = null!;
	internal XdgWmBase WmBase { get; private set; } = null!;
	internal ZxdgDecorationManagerV1? DecorationManager { get; private set; }
	internal WpViewporter? Viewporter { get; private set; }
	internal WpFractionalScaleManagerV1? FractionalScaleManager { get; private set; }
	internal ZwpTextInputManagerV3? TextInputManager { get; private set; }
	internal WpCursorShapeManagerV1? CursorShapeManager { get; private set; }
	internal LibDecorContext? LibDecor { get; private set; }

	private readonly WlDisplay _display;
	private readonly WlRegistry _registry;
	private readonly WlRegistry.Listener.Relay _registryListener;
	private readonly WlCompositor.Listener.Relay _compositorListener = new();
	private readonly XdgWmBase.Listener.Relay _wmBaseListener;
	private readonly ZxdgDecorationManagerV1.Listener.Relay _decorationListener = new();
	private readonly WpViewporter.Listener.Relay _viewporterListener = new();
	private readonly WpFractionalScaleManagerV1.Listener.Relay _fractionalScaleManagerListener = new();
	private readonly ZwpTextInputManagerV3.Listener.Relay _textInputManagerListener = new();
	private readonly ZwpTextInputV3.Listener.Relay _textInputListener;
	private readonly WpCursorShapeManagerV1.Listener.Relay _cursorShapeManagerListener = new();
	private readonly WpCursorShapeDeviceV1.Listener.Relay _cursorShapeDeviceListener = new();
	private readonly Dictionary<WlOutput, WaylandDisplay> _outputs = [];
	private readonly Dictionary<nint, WaylandWindow> _windows = [];
	private readonly int _displayFd;
	private readonly WlSeat.Listener.Relay _seatListener;
	private readonly WlKeyboard.Listener.Relay _keyboardListener;
	private readonly WlPointer.Listener.Relay _pointerListener;

	private WlSeat? _seat;
	private WlKeyboard? _keyboard;
	private WlPointer? _pointer;
	private ZwpTextInputV3? _textInput;
	private WpCursorShapeDeviceV1? _cursorShapeDevice;
	private WlSurface? _keyboardFocus;
	private WlSurface? _pointerFocus;
	private WlSurface? _textInputSurface;
	private KeyModifiers _modifiers;
	private Vector2<int> _pointerPosition;
	private CursorShape _cursorShape = CursorShape.Default;
	private uint _pointerSerial;
	private bool _textInputEnabled;
	private string _pendingText = string.Empty;
	private string _pendingPreedit = string.Empty;
	private int _pendingPreeditBegin;
	private int _pendingPreeditEnd;
	private bool _hasPendingPreedit;
	private double _scrollX;
	private double _scrollY;

	public WaylandPlatform(Application application) : base(application)
	{
		_display = WlDisplay.Connect(null!, new WlDisplayOptions());
		_displayFd = _display.GetFd();

		_wmBaseListener = new XdgWmBase.Listener.Relay
		{
			OnPing = (wmBase, serial) => wmBase.Pong(serial)
		};

		_seatListener = new WlSeat.Listener.Relay
		{
			OnCapabilities = OnSeatCapabilities
		};

		_keyboardListener = new WlKeyboard.Listener.Relay
		{
			OnEnter = OnKeyboardEnter,
			OnLeave = OnKeyboardLeave,
			OnKey = OnKeyboardKey,
			OnModifiers = OnKeyboardModifiers
		};

		_pointerListener = new WlPointer.Listener.Relay
		{
			OnEnter = OnPointerEnter,
			OnLeave = OnPointerLeave,
			OnMotion = OnPointerMotion,
			OnButton = OnPointerButton,
			OnAxis = OnPointerAxis,
			OnFrame = OnPointerFrame
		};

		_textInputListener = new ZwpTextInputV3.Listener.Relay
		{
			OnEnter = OnTextInputEnter,
			OnLeave = OnTextInputLeave,
			OnPreeditString = OnTextInputPreeditString,
			OnCommitString = OnTextInputCommitString,
			OnDone = OnTextInputDone
		};

		_registryListener = new WlRegistry.Listener.Relay
		{
			OnGlobal = OnGlobal,
			OnGlobalRemove = OnGlobalRemove
		};

		_registry = _display.GetRegistry(_registryListener, null!);
		_display.Roundtrip();

		if (Compositor == null || WmBase == null)
		{
			throw new NotSupportedException("Wayland compositor does not expose wl_compositor and xdg_wm_base.");
		}

		// Collect the initial state of the bound outputs.
		_display.Roundtrip();

		if (Environment.GetEnvironmentVariable("RAWSALT_WAYLAND_LIBDECOR") != "0")
		{
			LibDecor = LibDecorContext.TryCreate(_display);
			LibDecor?.WaitUntilReady();
		}
	}

	private void OnGlobal(WlRegistry registry, uint name, string @interface, uint version)
	{
		switch (@interface)
		{
			case "wl_compositor":
				Compositor = WlCompositor.Bind(registry, name, Math.Min(version, 4), _compositorListener, null!);
				break;

			case "xdg_wm_base":
				WmBase = XdgWmBase.Bind(registry, name, Math.Min(version, 1), _wmBaseListener, null!);
				break;

			case "wl_output":
				WaylandDisplay display = new(name);
				display.Output = WlOutput.Bind(registry, name, Math.Min(version, 4), display.Listener, null!);
				_outputs.Add(display.Output, display);
				break;

			case "wl_seat":
				_seat = WlSeat.Bind(registry, name, Math.Min(version, 5), _seatListener, null!);
				EnsureTextInput();
				break;

			case "zxdg_decoration_manager_v1":
				DecorationManager = ZxdgDecorationManagerV1.Bind(registry, name, Math.Min(version, 1),
					_decorationListener, null!);
				break;

			case "wp_viewporter":
				Viewporter = WpViewporter.Bind(registry, name, Math.Min(version, 1),
					_viewporterListener, null!);
				break;

			case "wp_fractional_scale_manager_v1":
				FractionalScaleManager = WpFractionalScaleManagerV1.Bind(registry, name,
					Math.Min(version, 1), _fractionalScaleManagerListener, null!);
				break;

			case "zwp_text_input_manager_v3":
				TextInputManager = ZwpTextInputManagerV3.Bind(registry, name, Math.Min(version, 1),
					_textInputManagerListener, null!);
				EnsureTextInput();
				break;

			case "wp_cursor_shape_manager_v1":
				CursorShapeManager = WpCursorShapeManagerV1.Bind(registry, name, Math.Min(version, 1),
					_cursorShapeManagerListener, null!);
				EnsureCursorShapeDevice();
				break;
		}
	}

	private void OnGlobalRemove(WlRegistry registry, uint name)
	{
		foreach (WaylandDisplay display in _outputs.Values.ToArray())
		{
			if (display.RegistryName != name || display.Output is not WlOutput output)
			{
				continue;
			}

			_outputs.Remove(output);
			display.Output = null;
			output.Dispose();
			break;
		}
	}

	private void OnSeatCapabilities(WlSeat seat, WlSeat.CapabilityEnum capabilities)
	{
		bool hasKeyboard = capabilities.HasFlag(WlSeat.CapabilityEnum.Keyboard);
		if (hasKeyboard && _keyboard == null)
		{
			_keyboard = seat.GetKeyboard(_keyboardListener, null!);
			EnsureTextInput();
		}
		else if (!hasKeyboard)
		{
			SetKeyboardFocus(null);
			_modifiers = KeyModifiers.None;
			_keyboard?.Dispose();
			_keyboard = null;
		}

		bool hasPointer = capabilities.HasFlag(WlSeat.CapabilityEnum.Pointer);
		if (hasPointer && _pointer == null)
		{
			_pointer = seat.GetPointer(_pointerListener, null!);
			EnsureCursorShapeDevice();
		}
		else if (!hasPointer)
		{
			if (_pointerFocus != null && FindWindow(_pointerFocus) is WaylandWindow pointerWindow)
			{
				pointerWindow.ProcessCursorLeft();
			}

			_pointerFocus = null;
			_cursorShapeDevice?.Dispose();
			_cursorShapeDevice = null;
			_pointer?.Dispose();
			_pointer = null;
		}
	}

	private void OnKeyboardEnter(WlKeyboard keyboard, uint serial, WlSurface? surface, ReadOnlySpan<byte> keys)
	{
		SetKeyboardFocus(surface);
	}

	private void OnKeyboardLeave(WlKeyboard keyboard, uint serial, WlSurface? surface)
	{
		if (ReferenceEquals(_keyboardFocus, surface))
		{
			SetKeyboardFocus(null);
		}
	}

	private void OnKeyboardKey(WlKeyboard keyboard, uint serial, uint time, uint key,
		WlKeyboard.KeyStateEnum state)
	{
		if (LinuxKeyMap.TryGetKey((int)key, out Key mapped) is false)
		{
			return;
		}

		if (FindWindow(_keyboardFocus) is not WaylandWindow window)
		{
			return;
		}

		bool pressed = state != WlKeyboard.KeyStateEnum.Released;
		bool repeat = state == WlKeyboard.KeyStateEnum.Repeated || (pressed && window.IsKeyDown(mapped));
		window.ProcessKey(mapped, _modifiers, pressed, repeat);
	}

	private void OnKeyboardModifiers(WlKeyboard keyboard, uint serial, uint depressed, uint latched,
		uint locked, uint group)
	{
		uint mask = depressed | latched | locked;
		KeyModifiers modifiers = KeyModifiers.None;

		if ((mask & (1u << 0)) != 0) modifiers |= KeyModifiers.Shift;
		if ((mask & (1u << 1)) != 0) modifiers |= KeyModifiers.CapsLock;
		if ((mask & (1u << 2)) != 0) modifiers |= KeyModifiers.Control;
		if ((mask & (1u << 3)) != 0) modifiers |= KeyModifiers.Alt;
		if ((mask & (1u << 4)) != 0) modifiers |= KeyModifiers.NumLock;
		if ((mask & (1u << 6)) != 0) modifiers |= KeyModifiers.Super;

		_modifiers = modifiers;
	}

	private void SetKeyboardFocus(WlSurface? surface)
	{
		if (ReferenceEquals(_keyboardFocus, surface))
		{
			return;
		}

		if (FindWindow(_keyboardFocus) is WaylandWindow previous)
		{
			previous.ProcessKeyboardFocus(false);
		}

		_keyboardFocus = surface;

		if (FindWindow(surface) is WaylandWindow current)
		{
			current.ProcessKeyboardFocus(true);
		}

		SyncTextInput();
	}

	private void OnPointerEnter(WlPointer pointer, uint serial, WlSurface? surface, WlFixed surfaceX,
		WlFixed surfaceY)
	{
		_pointerSerial = serial;
		_pointerFocus = surface;
		_pointerPosition = new Vector2<int>(ToPixel(surfaceX), ToPixel(surfaceY));

		if (FindWindow(surface) is WaylandWindow window)
		{
			window.ProcessMouseMove(_pointerPosition);
			window.ProcessCursorEntered();
			ApplyCursorShape(window.CursorShape);
		}
		else
		{
			ApplyCursorShape(CursorShape.Default);
		}
	}

	private void OnPointerLeave(WlPointer pointer, uint serial, WlSurface? surface)
	{
		if (ReferenceEquals(_pointerFocus, surface) is false)
		{
			return;
		}

		if (FindWindow(surface) is WaylandWindow window)
		{
			window.ProcessCursorLeft();
		}

		_pointerFocus = null;
	}

	private void OnPointerMotion(WlPointer pointer, uint time, WlFixed surfaceX, WlFixed surfaceY)
	{
		_pointerPosition = new Vector2<int>(ToPixel(surfaceX), ToPixel(surfaceY));

		if (FindWindow(_pointerFocus) is WaylandWindow window)
		{
			window.ProcessMouseMove(_pointerPosition);
		}
	}

	private void OnPointerButton(WlPointer pointer, uint serial, uint time, uint button,
		WlPointer.ButtonStateEnum state)
	{
		_pointerSerial = serial;

		if (LinuxButtonMap.TryGetButton((int)button, out MouseButton mapped) is false)
		{
			return;
		}

		if (FindWindow(_pointerFocus) is not WaylandWindow window)
		{
			return;
		}

		bool pressed = state == WlPointer.ButtonStateEnum.Pressed;
		window.ProcessMouseButton(mapped, pressed, _pointerPosition, _modifiers);
	}

	private void OnPointerAxis(WlPointer pointer, uint time, WlPointer.AxisEnum axis, WlFixed value)
	{
		// Wayland reports surface-local scroll units; one wheel notch is 10 units.
		double delta = (double)value / 10.0;

		switch (axis)
		{
			case WlPointer.AxisEnum.VerticalScroll:
				// Wayland positive is down, while the engine follows the GLFW convention (positive up).
				_scrollY -= delta;
				break;

			case WlPointer.AxisEnum.HorizontalScroll:
				_scrollX += delta;
				break;
		}
	}

	private void OnPointerFrame(WlPointer pointer)
	{
		int x = (int)Math.Round(_scrollX);
		int y = (int)Math.Round(_scrollY);
		if (x == 0 && y == 0)
		{
			return;
		}

		if (FindWindow(_pointerFocus) is not WaylandWindow window)
		{
			return;
		}

		_scrollX -= x;
		_scrollY -= y;
		window.ProcessMouseWheel(new Vector2<int>(x, y));
	}

	private WaylandWindow? FindWindow(WlSurface? surface)
	{
		if (surface == null)
		{
			return null;
		}

		return _windows.TryGetValue(surface.Handle, out WaylandWindow? window) ? window : null;
	}

	private static int ToPixel(WlFixed value)
	{
		return (int)MathF.Round((float)(double)value);
	}

	private void EnsureTextInput()
	{
		if (_textInput != null || TextInputManager == null || _seat == null)
		{
			return;
		}

		_textInput = TextInputManager.GetTextInput(_seat, _textInputListener, null!);
		SyncTextInput();
	}

	private void SyncTextInput()
	{
		if (_textInput == null)
		{
			return;
		}

		bool enable = FindWindow(_keyboardFocus) is WaylandWindow window && window.IsTextInputEnabled;
		if (enable == _textInputEnabled)
		{
			return;
		}

		_textInputEnabled = enable;

		if (enable)
		{
			_textInput.SetContentType(ZwpTextInputV3.ContentHintEnum.None,
				ZwpTextInputV3.ContentPurposeEnum.Normal);
			_textInput.Enable();
		}
		else
		{
			_textInput.Disable();
		}

		_textInput.Commit();

		if (!enable)
		{
			ResetPendingTextInput();
		}
	}

	private void OnTextInputEnter(ZwpTextInputV3 textInput, WlSurface? surface)
	{
		_textInputSurface = surface;
	}

	private void OnTextInputLeave(ZwpTextInputV3 textInput, WlSurface? surface)
	{
		if (!ReferenceEquals(_textInputSurface, surface))
		{
			return;
		}

		if (_hasPendingPreedit && FindWindow(surface) is WaylandWindow window)
		{
			window.ProcessPreedit(string.Empty, 0, 0);
		}

		ResetPendingTextInput();
		_textInputSurface = null;
	}

	private void OnTextInputPreeditString(ZwpTextInputV3 textInput, string? text, int cursorBegin,
		int cursorEnd)
	{
		_pendingPreedit = text ?? string.Empty;
		_pendingPreeditBegin = cursorBegin;
		_pendingPreeditEnd = cursorEnd;
		_hasPendingPreedit = true;
	}

	private void OnTextInputCommitString(ZwpTextInputV3 textInput, string? text)
	{
		_pendingText += text ?? string.Empty;
	}

	private void OnTextInputDone(ZwpTextInputV3 textInput, uint serial)
	{
		WaylandWindow? window = FindWindow(_textInputSurface) ?? FindWindow(_keyboardFocus);

		if (_hasPendingPreedit)
		{
			window?.ProcessPreedit(_pendingPreedit, _pendingPreeditBegin, _pendingPreeditEnd);
			_hasPendingPreedit = false;
		}

		if (_pendingText.Length != 0)
		{
			window?.ProcessTextInput(_pendingText);
			_pendingText = string.Empty;
		}
	}

	private void ResetPendingTextInput()
	{
		_pendingText = string.Empty;
		_pendingPreedit = string.Empty;
		_pendingPreeditBegin = 0;
		_pendingPreeditEnd = 0;
		_hasPendingPreedit = false;
	}

	private void EnsureCursorShapeDevice()
	{
		if (_cursorShapeDevice != null || CursorShapeManager == null || _pointer == null)
		{
			return;
		}

		_cursorShapeDevice = CursorShapeManager.GetPointer(_pointer, _cursorShapeDeviceListener, null!);
		ApplyCursorShape(_cursorShape);
	}

	private void ApplyWindowCursorShape(WaylandWindow window)
	{
		if (FindWindow(_pointerFocus) == window)
		{
			ApplyCursorShape(window.CursorShape);
		}
	}

	private void ApplyCursorShape(CursorShape shape)
	{
		_cursorShape = shape;
		_cursorShapeDevice?.SetShape(_pointerSerial, ToCursorShape(shape));
	}

	private static WpCursorShapeDeviceV1.ShapeEnum ToCursorShape(CursorShape shape)
	{
		return shape switch
		{
			CursorShape.ContextMenu => WpCursorShapeDeviceV1.ShapeEnum.ContextMenu,
			CursorShape.Help => WpCursorShapeDeviceV1.ShapeEnum.Help,
			CursorShape.Pointer => WpCursorShapeDeviceV1.ShapeEnum.Pointer,
			CursorShape.Progress => WpCursorShapeDeviceV1.ShapeEnum.Progress,
			CursorShape.Wait => WpCursorShapeDeviceV1.ShapeEnum.Wait,
			CursorShape.Cell => WpCursorShapeDeviceV1.ShapeEnum.Cell,
			CursorShape.Crosshair => WpCursorShapeDeviceV1.ShapeEnum.Crosshair,
			CursorShape.Text => WpCursorShapeDeviceV1.ShapeEnum.Text,
			CursorShape.VerticalText => WpCursorShapeDeviceV1.ShapeEnum.VerticalText,
			CursorShape.Alias => WpCursorShapeDeviceV1.ShapeEnum.Alias,
			CursorShape.Copy => WpCursorShapeDeviceV1.ShapeEnum.Copy,
			CursorShape.Move => WpCursorShapeDeviceV1.ShapeEnum.Move,
			CursorShape.NoDrop => WpCursorShapeDeviceV1.ShapeEnum.NoDrop,
			CursorShape.NotAllowed => WpCursorShapeDeviceV1.ShapeEnum.NotAllowed,
			CursorShape.Grab => WpCursorShapeDeviceV1.ShapeEnum.Grab,
			CursorShape.Grabbing => WpCursorShapeDeviceV1.ShapeEnum.Grabbing,
			CursorShape.ResizeEast => WpCursorShapeDeviceV1.ShapeEnum.EResize,
			CursorShape.ResizeNorth => WpCursorShapeDeviceV1.ShapeEnum.NResize,
			CursorShape.ResizeNorthEast => WpCursorShapeDeviceV1.ShapeEnum.NeResize,
			CursorShape.ResizeNorthWest => WpCursorShapeDeviceV1.ShapeEnum.NwResize,
			CursorShape.ResizeSouth => WpCursorShapeDeviceV1.ShapeEnum.SResize,
			CursorShape.ResizeSouthEast => WpCursorShapeDeviceV1.ShapeEnum.SeResize,
			CursorShape.ResizeSouthWest => WpCursorShapeDeviceV1.ShapeEnum.SwResize,
			CursorShape.ResizeWest => WpCursorShapeDeviceV1.ShapeEnum.WResize,
			CursorShape.ResizeEastWest => WpCursorShapeDeviceV1.ShapeEnum.EwResize,
			CursorShape.ResizeNorthSouth => WpCursorShapeDeviceV1.ShapeEnum.NsResize,
			CursorShape.ResizeNorthEastSouthWest => WpCursorShapeDeviceV1.ShapeEnum.NeswResize,
			CursorShape.ResizeNorthWestSouthEast => WpCursorShapeDeviceV1.ShapeEnum.NwseResize,
			CursorShape.ResizeColumn => WpCursorShapeDeviceV1.ShapeEnum.ColResize,
			CursorShape.ResizeRow => WpCursorShapeDeviceV1.ShapeEnum.RowResize,
			CursorShape.AllScroll => WpCursorShapeDeviceV1.ShapeEnum.AllScroll,
			CursorShape.ZoomIn => WpCursorShapeDeviceV1.ShapeEnum.ZoomIn,
			CursorShape.ZoomOut => WpCursorShapeDeviceV1.ShapeEnum.ZoomOut,
			CursorShape.DragAndDropAsk => WpCursorShapeDeviceV1.ShapeEnum.DndAsk,
			CursorShape.AllResize => WpCursorShapeDeviceV1.ShapeEnum.AllResize,
			_ => WpCursorShapeDeviceV1.ShapeEnum.Default,
		};
	}

	public static bool IsAvailable()
	{
		if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WAYLAND_DISPLAY")))
		{
			return false;
		}

		try
		{
			using WlDisplay display = WlDisplay.Connect(null!, new WlDisplayOptions());
			return true;
		}
		catch (Exception ex) when (ex is DllNotFoundException or EntryPointNotFoundException or NWaylandException)
		{
			return false;
		}
	}

	internal int GetOutputScale(WlOutput output)
	{
		return _outputs.TryGetValue(output, out WaylandDisplay? display) ? display.Scale : 1;
	}

	internal int GetMaximumOutputScale()
	{
		int scale = 1;
		foreach (WaylandDisplay display in _outputs.Values)
		{
			scale = Math.Max(scale, display.Scale);
		}

		return scale;
	}

	protected override void Dispose(bool disposing)
	{
		foreach (WaylandWindow window in _windows.Values.ToArray())
		{
			window.Close();
		}

		_windows.Clear();

		LibDecor?.Dispose();
		LibDecor = null;

		_textInput?.Dispose();
		_textInput = null;
		_cursorShapeDevice?.Dispose();
		_cursorShapeDevice = null;

		_keyboard?.Dispose();
		_keyboard = null;
		_pointer?.Dispose();
		_pointer = null;
		_seat?.Dispose();
		_seat = null;

		foreach (WaylandDisplay display in _outputs.Values)
		{
			display.Output?.Dispose();
		}

		_outputs.Clear();

		DecorationManager?.Dispose();
		DecorationManager = null;
		FractionalScaleManager?.Dispose();
		FractionalScaleManager = null;
		Viewporter?.Dispose();
		Viewporter = null;
		TextInputManager?.Dispose();
		TextInputManager = null;
		CursorShapeManager?.Dispose();
		CursorShapeManager = null;
		WmBase?.Dispose();
		WmBase = null!;
		Compositor?.Dispose();
		Compositor = null!;
		_registry.Dispose();
		_display.Dispose();
	}

	public override IReadOnlyList<Display> GetDisplays()
	{
		return [.. _outputs.Values];
	}

	public override WaylandWindow CreateView(in WindowConfiguration configuration)
	{
		LibDecor?.WaitUntilReady();

		WaylandWindow window = new(this, configuration.Size, configuration.Title);
		_windows.Add(window.Surface.Handle, window);
		window.CursorShapeChanged += _ => ApplyWindowCursorShape(window);
		window.TextInputEnabledChanged += _ => SyncTextInput();
		return window;
	}

	public override bool ReleaseView(View view)
	{
		if (view is not WaylandWindow window)
		{
			return false;
		}

		window.Close();
		return true;
	}

	public override void PumpEvents()
	{
		try
		{
			// libdecor also processes non-Wayland events (GTK plugin), so dispatch it first.
			LibDecor?.Dispatch(0);

			// Non-blocking read cycle: prepare, poll, read (or cancel), then dispatch.
			if (_display.PrepareRead() == 0)
			{
				PollFd descriptor = new()
				{
					fd = _displayFd,
					events = LibC.PollIn
				};

				if (LibC.poll([descriptor], 1, 0) > 0)
				{
					_display.ReadEvents();
				}
				else
				{
					_display.CancelRead();
				}
			}

			_display.DispatchPending();
			_display.Flush();
		}
		catch (Exception ex) when (ex is NWaylandException or ObjectDisposedException)
		{
			Log.Error(ex, "Wayland event pump failed.");
		}
	}

	public override bool TryCreateErrorMessage(string message)
	{
		Console.Error.WriteLine(message);
		return true;
	}

	internal void RemoveWindow(WaylandWindow window)
	{
		nint handle = window.Surface.Handle;

		if (_keyboardFocus != null && _keyboardFocus.Handle == handle)
		{
			_keyboardFocus = null;
			window.ProcessKeyboardFocus(false);
		}

		if (_pointerFocus != null && _pointerFocus.Handle == handle)
		{
			_pointerFocus = null;
		}

		if (_textInputSurface != null && _textInputSurface.Handle == handle)
		{
			_textInputSurface = null;
			ResetPendingTextInput();
		}

		SyncTextInput();
		_windows.Remove(handle);
	}
}
