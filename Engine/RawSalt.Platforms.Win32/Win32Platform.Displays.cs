using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using NiTiS.Mathematics;

namespace RawSalt.Platforms.Win32;

public partial class Win32Platform
{
	private List<Win32Display>? _displays;

	[SupportedOSPlatform("windows10.0")]
	public override Display GetPrimaryDisplay()
	{
		HMONITOR handle = PInvoke.MonitorFromPoint(Point.Empty, MONITOR_FROM_FLAGS.MONITOR_DEFAULTTOPRIMARY);

		Win32Display primary = GetDisplay(handle);
		return primary;
	}

	[SupportedOSPlatform("windows10.0")]
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

	[SupportedOSPlatform("windows10.0")]
	private unsafe BOOL MonitorEnumProc(HMONITOR hMonitor, HDC hdcMonitor, RECT* lprcMonitor, LPARAM dwData)
	{
		Debug.Assert(_displays != null);

		Win32Display display = GetDisplay(hMonitor);
		_displays.Add(display);

		return true;
	}

	[SupportedOSPlatform("windows10.0")]
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

	[SupportedOSPlatform("windows10.0")]
	private static string GetDisplayName(string deviceName)
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
}