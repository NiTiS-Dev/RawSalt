using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Dwm;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.HiDpi;
using Windows.Win32.UI.WindowsAndMessaging;
using NiTiS.Mathematics;
using RawSalt.Collections;
using RawSalt.Logging;

namespace RawSalt.Platforms.Win32;

public partial class Win32Platform : Platform
{
	public override WellKnownPlatformApi Api => WellKnownPlatformApi.Win32;

	private readonly SmallDictionary<nint /*HWND*/, Win32Window> _windows = [];

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
	}

	protected override void Dispose(bool disposing)
	{
		foreach (var x in _windows)
		{
			PInvoke.DestroyWindow((HWND)x.Key);
		}

		if (disposing)
		{

		}
	}

	[SupportedOSPlatform("windows10.0")]
	public override unsafe Win32Window CreateView(in WindowConfiguration configuration)
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
			Log.Error("CreateWindowEx failed: {0}", Marshal.GetLastPInvokeError());
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
				window.OnCloseRequested();
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