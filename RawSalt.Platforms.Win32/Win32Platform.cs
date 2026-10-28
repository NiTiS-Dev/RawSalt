using System.ComponentModel;
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
using RawSalt.Platforms;

namespace RawSalt.Windowing.Win32;

public partial class Win32Platform : Platform
{
	public override WellKnownPlatformApi Api => WellKnownPlatformApi.Win32;

	private List<Win32Display>? _displays;
	private readonly SmallDictionary<nint /*HWND*/, Win32Window> _windows = [];

	[SupportedOSPlatform("windows10.0.15063")]
	public Win32Platform()
	{
		PInvoke.SetProcessDpiAwarenessContext(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);
		HInstance = PInvoke.GetModuleHandle((PCWSTR)null);
		_wndproc = WndProc;
	}

	[SupportedOSPlatform("windows5.0")]
	public override Display GetPrimaryDisplay()
	{
		HMONITOR handle = PInvoke.MonitorFromPoint(Point.Empty, MONITOR_FROM_FLAGS.MONITOR_DEFAULTTOPRIMARY);

		Win32Display primary = GetDisplay(handle);
		return primary;
	}

	[SupportedOSPlatform("windows5.0")]
	public override unsafe IReadOnlyList<Display> GetDisplays()
	{
		if (_displays == null)
		{
			_displays = [];
		}
		else
		{
			_displays.Clear();
		}

		PInvoke.EnumDisplayMonitors(HDC.Null, null, MonitorEnumProc, default);
		return _displays.AsReadOnly();
	}

	[SupportedOSPlatform("windows5.0")]
	private unsafe BOOL MonitorEnumProc(HMONITOR hMonitor, HDC hdcMonitor, RECT* lprcMonitor, LPARAM dwData)
	{
		if (_displays is null)
		{
			Log.Error("_displays is null: {0}", Marshal.GetLastPInvokeError());
			return true;
		}

		Win32Display display = GetDisplay(hMonitor);
		_displays.Add(display);

		return true;
	}

	[SupportedOSPlatform("windows5.0")]
	private static unsafe Win32Display GetDisplay(HMONITOR handle)
	{
		MONITORINFOEXW info;
		info.monitorInfo.cbSize = (uint)Unsafe.SizeOf<MONITORINFOEXW>();
		if (!PInvoke.GetMonitorInfo(handle, (MONITORINFO*)&info))
		{
			throw new Win32Exception(Marshal.GetLastPInvokeError(), Marshal.GetLastPInvokeErrorMessage());
		}

		string deviceName = info.szDevice.ToString();
		string displayName = GetDisplayName(deviceName);
		Region2<int> region = info.monitorInfo.rcMonitor.AsRegion();
		Region2<int> workRegion = info.monitorInfo.rcWork.AsRegion();
		return new Win32Display(displayName, region, workRegion);
	}

	[SupportedOSPlatform("windows5.0")]
	private static unsafe string GetDisplayName(string deviceName)
	{
		DISPLAY_DEVICEW displayDevice = new()
		{
			cb = (uint)sizeof(DISPLAY_DEVICEW)
		};

		for (uint i = 0;
		     PInvoke.EnumDisplayDevices(deviceName, i, ref displayDevice, 0);
		     i++)
		{
			if ((displayDevice.StateFlags & DISPLAY_DEVICE_STATE_FLAGS.DISPLAY_DEVICE_ACTIVE) != 0)
			{
				return displayDevice.DeviceString.ToString();
			}

			displayDevice = new DISPLAY_DEVICEW
			{
				cb = (uint)sizeof(DISPLAY_DEVICEW)
			};
		}

		return deviceName;
	}

	[SupportedOSPlatform("windows5.0")]
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

		UpdateTheme(handle);
		PInvoke.UpdateWindow(handle);
		PInvoke.ShowWindow(handle, SHOW_WINDOW_CMD.SW_SHOW);

		return window;
	}

	[SupportedOSPlatform("windows5.0")]
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
				if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 14393))
				{
					window.UpdateSize();
				}
				return PInvoke.DefWindowProc(hWnd, msg, wParam, lParam);
			}

			if (msg == PInvoke.WM_DPICHANGED)
			{
				if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 14393))
				{
					window.UpdateSize();
				}
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