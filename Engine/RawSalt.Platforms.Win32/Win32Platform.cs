using System.Collections.Concurrent;
using System.ComponentModel;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Dwm;
using Windows.Win32.UI.HiDpi;
using Windows.Win32.UI.WindowsAndMessaging;
using RawSalt.Collections;
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
			return PInvoke.DefWindowProc(hWnd, msg, wParam, lParam);
		}

		if (_windows.TryGetValue(hWnd, out Win32Window? window))
		{
			if (msg == PInvoke.WM_SIZE)
			{
				window.UpdateSize();
				return PInvoke.DefWindowProc(hWnd, msg, wParam, lParam);
			}

			if (msg == PInvoke.WM_DPICHANGED)
			{
				window.UpdateSize();
				return PInvoke.DefWindowProc(hWnd, msg, wParam, lParam);
			}

			if (msg == PInvoke.WM_CLOSE)
			{
				// TODO: Instead send application stop request, and then if application is accept close request it will destroy the window
				window.RequestClose();
				return new LRESULT(0);
			}
		}

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