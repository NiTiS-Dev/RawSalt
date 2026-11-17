using System.Collections.Concurrent;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Dwm;
using Windows.Win32.UI.HiDpi;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using Windows.Win32.UI.WindowsAndMessaging;
using NiTiS.Mathematics;
using RawSalt.Collections;
using RawSalt.Input;
using RawSalt.Logging;

namespace RawSalt.Platforms.Win32;

public partial class Win32Platform : Platform
{
	/// <inheritdoc/>
	public sealed override WellKnownPlatformApi Api => WellKnownPlatformApi.Win32;

	// Posted to the window thread to make it drain the dispatcher queue.
	private const uint WM_WAKE = PInvoke.WM_APP + 1;

	private readonly SmallDictionary<nint /*HWND*/, Win32Window> _windows = [];
	private readonly ConcurrentQueue<Action> _dispatcher = new();

	// Written by the window thread, drained by View.PumpEvents on the application thread.
	private readonly ConcurrentQueue<Win32KeyEvent> _keyboardQueue = new();

	private volatile bool _mouseWasMoved;
	private volatile bool _mouseWasScrolled;
	private readonly Lock _mouseStateLock = new();
	private readonly bool[] _pressedMouseButtons = new bool[5];
	private readonly ConcurrentQueue<Win32MouseEvent> _mouseQueue = new();
	private Vector2 _mousePosition = default;
	private Vector2 _mouseScroll = default;

	private volatile ThreeState _focus = ThreeState.Unknown;

	private readonly ManualResetEventSlim _threadReady = new(false);
	private readonly Thread _windowThread;
	private volatile bool _stopping;
	private volatile uint _windowThreadId;

	[SupportedOSPlatform("windows10.0")]
	public Win32Platform(Application application) : base(application)
	{
		if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 15063))
		{
			PInvoke.SetProcessDpiAwarenessContext(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);
		}
		else
		{
			Log.Error("Failed to enable DPI Awareness Context.");
		}
		HInstance = PInvoke.GetModuleHandle((PCWSTR)null);
		_wndproc = WndProc;

		_windowThread = new Thread(RunWindowThread)
		{
			Name = "RawSalt/Win32WindowThread",
			IsBackground = true
		};
		_windowThread.Start();
		_threadReady.Wait();
	}

	[SupportedOSPlatform("windows5.0")]
	protected override void Dispose(bool disposing)
	{
		if (Thread.CurrentThread == _windowThread)
		{
			_stopping = true;
			DestroyAllWindows();
		}
		else if (_windowThread.IsAlive)
		{
			InvokeAndWait(DestroyAllWindows);
			_stopping = true;
			Signal();
			_windowThread.Join();
		}
		else
		{
			DestroyAllWindows();
		}

		_threadReady.Dispose();

		if (disposing)
		{

		}
	}

	[SupportedOSPlatform("windows5.0")]
	internal void Post(Action action)
	{
		if (Thread.CurrentThread == _windowThread)
		{
			action();
			return;
		}

		if (_stopping)
		{
			return;
		}

		_dispatcher.Enqueue(action);
		Signal();
	}

	[SupportedOSPlatform("windows5.0")]
	internal void InvokeAndWait(Action action)
	{
		if (Thread.CurrentThread == _windowThread)
		{
			action();
			return;
		}

		if (_stopping)
		{
			throw new ObjectDisposedException(nameof(Win32Platform));
		}

		using ManualResetEventSlim done = new(false);
		Exception? failure = null;
		_dispatcher.Enqueue(() =>
		{
			try
			{
				action();
			}
			catch (Exception ex)
			{
				failure = ex;
			}
			finally
			{
				done.Set();
			}
		});
		Signal();
		done.Wait();

		if (failure != null)
		{
			ExceptionDispatchInfo.Capture(failure).Throw();
		}
	}

	[SupportedOSPlatform("windows10.0")]
	private void RunWindowThread()
	{
		_windowThreadId = PInvoke.GetCurrentThreadId();
		_threadReady.Set();

		try
		{
			while (!_stopping)
			{
				DrainDispatcher();

				if (!PInvoke.GetMessage(out MSG msg, HWND.Null, 0, 0))
				{
					break;
				}

				if (msg.message == WM_WAKE)
				{
					continue;
				}

				PInvoke.TranslateMessage(msg);
				PInvoke.DispatchMessage(msg);
			}

			DrainDispatcher();
		}
		catch (Exception ex)
		{
			Log.Error(ex, "Win32 window thread crashed.");
		}
		finally
		{
			// Stop posting to this thread id: it may be reused after the thread exits.
			_windowThreadId = 0;
			_stopping = true;

			foreach (Win32Window window in _windows.Values)
			{
				window.RequestClose();
			}
		}
	}

	private void DrainDispatcher()
	{
		while (_dispatcher.TryDequeue(out Action? action))
		{
			action();
		}
	}

	[SupportedOSPlatform("windows5.0")]
	private void Signal()
	{
		if (_windowThreadId != 0)
		{
			PInvoke.PostThreadMessage(_windowThreadId, WM_WAKE, default, default);
		}
	}

	internal bool TryDrainKeyboardQueue(out Win32KeyEvent keyEvent)
	{
		return _keyboardQueue.TryDequeue(out keyEvent);
	}

	internal bool TryDrainMouseQueue(out Win32MouseEvent mouseEvent)
	{
		return _mouseQueue.TryDequeue(out mouseEvent);
	}

	// Returns true, when mouse moved
	internal bool GetMousePosition(out Vector2 mousePosition)
	{
		lock (_mouseStateLock)
		{
			if (_mouseWasMoved)
			{
				mousePosition = _mousePosition;

				_mouseWasMoved = false;
				return true;
			}
		}

		mousePosition = default;
		return false;
	}

	internal bool GetMouseScroll(out Vector2 mouseScroll)
	{
		lock (_mouseStateLock)
		{
			if (_mouseWasScrolled)
			{
				mouseScroll = _mouseScroll;

				_mouseWasScrolled = false;
				return true;
			}
		}

		mouseScroll = default;
		return false;
	}

	internal ThreeState GetFocusState()
	{
		ThreeState state = _focus;
		_focus = ThreeState.Unknown;
		return state;
	}

	[SupportedOSPlatform("windows5.0")]
	private void DestroyAllWindows()
	{
		foreach (nint handle in _windows.Keys)
		{
			PInvoke.DestroyWindow((HWND)handle);
		}

		_windows.Clear();
	}

	[SupportedOSPlatform("windows10.0")]
	public override Win32Window CreateView(in WindowConfiguration configuration)
	{
		WindowConfiguration config = configuration;
		Win32Window? window = null;
		InvokeAndWait(() => window = CreateViewCore(in config));
		return window!;
	}

	[SupportedOSPlatform("windows10.0")]
	private unsafe Win32Window CreateViewCore(in WindowConfiguration configuration)
	{
		RegisterClass();
		WINDOW_STYLE style = WINDOW_STYLE.WS_OVERLAPPEDWINDOW;
		WINDOW_EX_STYLE exStyle = WINDOW_EX_STYLE.WS_EX_APPWINDOW;

		RECT rect = new()
		{
			left = 0,
			top = 0,
			right = configuration.Size.X,
			bottom = configuration.Size.Y
		};
		PInvoke.AdjustWindowRectEx(&rect, style, false, exStyle);

		HWND handle;
		Win32Window window;
		fixed (char* pTitle = configuration.Title)
		{
			handle = PInvoke.CreateWindowEx(exStyle, (PCWSTR)(void*)_classAtom, (PCWSTR)pTitle, style,
				configuration.Position?.X ?? PInvoke.CW_USEDEFAULT,
				configuration.Position?.Y ?? PInvoke.CW_USEDEFAULT,
				rect.Width,
				rect.Height,
				HWND.Null,
				HMENU.Null,
				HInstance,
				null);
		}

		if (handle == default)
		{
			throw new Win32Exception(Marshal.GetLastPInvokeError());
		}

		window = new(handle, this);
		_windows.Add(handle, window);
		// First WM_SIZE event sent before Win32Window is instantiated, need to manually update window size.
		window.UpdateSize();
		UpdateTheme(handle);
		PInvoke.UpdateWindow(handle);

		return window;
	}

	[SupportedOSPlatform("windows10.0")]
	public override bool ReleaseView(View view)
	{
		if (view is not Win32Window window)
		{
			return false;
		}

		window.Close();
		return true;
	}

	[SupportedOSPlatform("windows5.0")]
	public override bool TryCreateErrorMessage(string message)
	{
		_ = PInvoke.MessageBox(HWND.Null, message, null, MESSAGEBOX_STYLE.MB_OK | MESSAGEBOX_STYLE.MB_ICONWARNING);
		return true;
	}

	private static Key MapKey(VIRTUAL_KEY virtualKey, LPARAM lParam)
	{
		uint flags = unchecked((uint)lParam.Value);

		uint scanCode = (flags >> 16) & 0xFF;
		bool extended = (flags & (1u << 24)) != 0;

		switch (virtualKey)
		{
			case VIRTUAL_KEY.VK_SHIFT:
				return scanCode == 0x36 ? Key.ShiftRight : Key.ShiftLeft;

			case VIRTUAL_KEY.VK_CONTROL: // VK_CONTROL
				return extended ? Key.ControlRight : Key.ControlLeft;

			case VIRTUAL_KEY.VK_MENU: // VK_MENU
				return extended ? Key.AltRight : Key.AltLeft;
		}

		// Letters, digits and space use the same values as Win32 virtual-key codes.
		if ((ushort)virtualKey is >= 'A' and <= 'Z' or >= '0' and <= '9' or ' ')
		{
			return (Key)virtualKey;
		}

		if (virtualKey is >= VIRTUAL_KEY.VK_NUMPAD0 and <= VIRTUAL_KEY.VK_NUMPAD9)
		{
			return Key.Keypad0 + (virtualKey - VIRTUAL_KEY.VK_NUMPAD0);
		}

		if (virtualKey is >= VIRTUAL_KEY.VK_F1 and <= VIRTUAL_KEY.VK_F24)
		{
			return Key.F1 + (virtualKey - VIRTUAL_KEY.VK_F1);
		}

		return virtualKey switch
		{
			VIRTUAL_KEY.VK_BACK => Key.Backspace,
			VIRTUAL_KEY.VK_TAB => Key.Tab,
			VIRTUAL_KEY.VK_RETURN => Key.Enter,
			VIRTUAL_KEY.VK_PAUSE => Key.Pause,
			VIRTUAL_KEY.VK_CAPITAL => Key.CapsLock,
			VIRTUAL_KEY.VK_ESCAPE => Key.Escape,
			VIRTUAL_KEY.VK_PRIOR => Key.PageUp,
			VIRTUAL_KEY.VK_NEXT => Key.PageDown,
			VIRTUAL_KEY.VK_END => Key.End,
			VIRTUAL_KEY.VK_HOME => Key.Home,
			VIRTUAL_KEY.VK_LEFT => Key.Left,
			VIRTUAL_KEY.VK_UP => Key.Up,
			VIRTUAL_KEY.VK_RIGHT => Key.Right,
			VIRTUAL_KEY.VK_DOWN => Key.Down,
			VIRTUAL_KEY.VK_SNAPSHOT => Key.PrintScreen,
			VIRTUAL_KEY.VK_INSERT => Key.Insert,
			VIRTUAL_KEY.VK_DELETE => Key.Delete,
			VIRTUAL_KEY.VK_LWIN => Key.SuperLeft,
			VIRTUAL_KEY.VK_RWIN => Key.SuperRight,
			VIRTUAL_KEY.VK_APPS => Key.Menu,
			VIRTUAL_KEY.VK_MULTIPLY => Key.KeypadMultiply,
			VIRTUAL_KEY.VK_ADD => Key.KeypadAdd,
			VIRTUAL_KEY.VK_SUBTRACT => Key.KeypadSubtract,
			VIRTUAL_KEY.VK_DECIMAL => Key.KeypadPeriod,
			VIRTUAL_KEY.VK_DIVIDE => Key.KeypadDivide,
			VIRTUAL_KEY.VK_NUMLOCK => Key.NumLock,
			VIRTUAL_KEY.VK_SCROLL => Key.ScrollLock,
			VIRTUAL_KEY.VK_LSHIFT => Key.ShiftLeft,
			VIRTUAL_KEY.VK_RSHIFT => Key.ShiftRight,
			VIRTUAL_KEY.VK_LCONTROL => Key.ControlLeft,
			VIRTUAL_KEY.VK_RCONTROL => Key.ControlRight,
			VIRTUAL_KEY.VK_LMENU => Key.AltLeft,
			VIRTUAL_KEY.VK_RMENU => Key.AltRight,
			VIRTUAL_KEY.VK_OEM_1 => Key.Semicolon,
			VIRTUAL_KEY.VK_OEM_PLUS => Key.Equal,
			VIRTUAL_KEY.VK_OEM_COMMA => Key.Comma,
			VIRTUAL_KEY.VK_OEM_MINUS => Key.Minus,
			VIRTUAL_KEY.VK_OEM_PERIOD => Key.Period,
			VIRTUAL_KEY.VK_OEM_2 => Key.Slash,
			VIRTUAL_KEY.VK_OEM_3 => Key.GraveAccent,
			VIRTUAL_KEY.VK_OEM_4 => Key.LeftBracket,
			VIRTUAL_KEY.VK_OEM_5 => Key.BackSlash,
			VIRTUAL_KEY.VK_OEM_6 => Key.RightBracket,
			VIRTUAL_KEY.VK_OEM_7 => Key.Apostrophe,
			_ => Key.Unknown
		};
	}

	private static MouseButton MapButton(uint code, WPARAM wParam, out bool pressed)
	{
		uint button = ((uint)wParam >> 16) & 0xFFFF;

		switch (code)
		{
			case PInvoke.WM_LBUTTONUP:
				pressed = false;
				return MouseButton.Left;
			case PInvoke.WM_LBUTTONDOWN:
				pressed = true;
				return MouseButton.Left;

			case PInvoke.WM_RBUTTONUP:
				pressed = false;
				return MouseButton.Right;
			case PInvoke.WM_RBUTTONDOWN:
				pressed = true;
				return MouseButton.Right;

			case PInvoke.WM_MBUTTONUP:
				pressed = false;
				return MouseButton.Middle;
			case PInvoke.WM_MBUTTONDOWN:
				pressed = true;
				return MouseButton.Middle;

			case PInvoke.WM_XBUTTONUP:
				pressed = false;
				return FromX();
			case PInvoke.WM_XBUTTONDOWN:
				pressed = true;
				return FromX();

			default:
				pressed = false;
				return MouseButton.Back;
		}

		MouseButton FromX()
		{
			return button switch
			{
				PInvoke.XBUTTON1 => MouseButton.Back,
				_ => MouseButton.Forward
			};
		}
	}

	private static Vector2 WheelToVector(WPARAM wParam, bool horizontal)
	{
		short value = unchecked((short)((uint)wParam >> 16));

		float delta = PInvoke.WHEEL_DELTA;
		Vector2 scroll = horizontal ? new Vector2(value / delta, 0) : new Vector2(0, value / delta);

		return scroll;
	}

	[SupportedOSPlatform("windows10.0")]
	private LRESULT WndProc(HWND hWnd, uint msg, WPARAM wParam, LPARAM lParam)
	{
		if (msg == PInvoke.WM_DESTROY)
		{
			_windows.Remove(hWnd);
			return new LRESULT(0);
		}

		if (msg == PInvoke.WM_SETTINGCHANGE)
		{
			UpdateTheme(hWnd);
			goto DEF_WIND_PROC;
		}

		if (_windows.TryGetValue(hWnd, out Win32Window? window))
		{
			if (msg is PInvoke.WM_KEYDOWN or PInvoke.WM_SYSKEYDOWN or PInvoke.WM_KEYUP or PInvoke.WM_SYSKEYUP)
			{
				Key key = MapKey((VIRTUAL_KEY)(ushort)wParam, lParam);
				bool isDown = msg is PInvoke.WM_KEYDOWN or PInvoke.WM_SYSKEYDOWN;
				bool wasDown = (lParam & (1 << 30)) != 0;

				// TODO: Since we do not remove stale buttons yet, we should not push repeat press events into queue.
				if (isDown ^ wasDown)
				{
					_keyboardQueue.Enqueue(new Win32KeyEvent()
					{
						Key = key,
						Pressed = isDown
					});
				}

				goto DEF_WIND_PROC;
			}

			if (msg is PInvoke.WM_MOUSEMOVE)
			{
				// Relative to view
				int x = unchecked((short)(lParam & 0xFFFF));
				int y = unchecked((short)((lParam >> 16) & 0xFFFF));

				lock (_mouseStateLock)
				{
					_mouseWasMoved = true;
					_mousePosition = new(x, y);
				}

				goto DEF_WIND_PROC;
			}

			if (msg is PInvoke.WM_LBUTTONDOWN or PInvoke.WM_RBUTTONDOWN or PInvoke.WM_MBUTTONDOWN or PInvoke.WM_XBUTTONDOWN
			    or PInvoke.WM_LBUTTONUP or PInvoke.WM_RBUTTONUP or PInvoke.WM_MBUTTONUP or PInvoke.WM_XBUTTONUP)
			{
				MouseButton button = MapButton(msg, wParam, out bool pressed);

				_pressedMouseButtons[(int)button] = pressed;

				if (pressed && _pressedMouseButtons.Count(e => e) == 1)
				{
					PInvoke.SetCapture(hWnd);
				}
				else if (!pressed && !_pressedMouseButtons.Any(e => e))
				{
					PInvoke.ReleaseCapture();
				}

				_mouseQueue.Enqueue(new Win32MouseEvent()
				{
					Button = button,
					Pressed = pressed
				});
			}

			if (msg is PInvoke.WM_MOUSEWHEEL or PInvoke.WM_MOUSEHWHEEL)
			{
				lock (_mouseStateLock)
				{
					_mouseWasScrolled = true;
					Vector2 scroll = WheelToVector(wParam, horizontal: msg is PInvoke.WM_MOUSEHWHEEL);
				}
			}

			if (msg == PInvoke.WM_SETFOCUS)
			{
				_focus = ThreeState.True;
			}

			if (msg == PInvoke.WM_KILLFOCUS)
			{
				_focus = ThreeState.False;
				PInvoke.ReleaseCapture();
			}

			if (msg == PInvoke.WM_SIZE)
			{
				window.UpdateSize();
				goto DEF_WIND_PROC;
			}

			if (msg == PInvoke.WM_DPICHANGED)
			{
				window.UpdateSize();
				goto DEF_WIND_PROC;
			}

			if (msg == PInvoke.WM_CLOSE)
			{
				// TODO: Instead send application stop request, and then if application is accept close request it will destroy the window
				window.RequestClose();
				return new LRESULT(0);
			}
		}

		DEF_WIND_PROC:
		return PInvoke.DefWindowProc(hWnd, msg, wParam, lParam);
	}

	[SupportedOSPlatform("windows10.0")]
	private static unsafe void UpdateTheme(HWND hWnd)
	{
    	BOOL useDark = IsDarkModeEnabled();

    	if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 17763))
    	{
    		HRESULT result = PInvoke.DwmSetWindowAttribute(hWnd,
    			DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE,
    			&useDark, (uint)sizeof(BOOL));

		    if (result != HRESULT.S_OK)
		    {
			    Log.Error("Failed to change window theme; hresult: {0}", result);
		    }
    	}
	}
}