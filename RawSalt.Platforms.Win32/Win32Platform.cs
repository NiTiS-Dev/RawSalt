using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using NiTiS.Mathematics;
using RawSalt.Logging;
using RawSalt.Platforms;

namespace RawSalt.Windowing.Win32;

public class Win32Platform : Platform
{
	public override WellKnownPlatformApi Api => WellKnownPlatformApi.Win32;

	private List<Win32Display>? _displays;

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
		Debug.Assert(_displays is not null);

		MONITORINFOEXW info;
		info.monitorInfo.cbSize = (uint)Unsafe.SizeOf<MONITORINFOEXW>();
		if (!PInvoke.GetMonitorInfo(hMonitor, (MONITORINFO*)&info))
		{
			Log.Error("GetMonitorInfo failed");
			return true;
		}

		string deviceName = info.szDevice.ToString();
		string displayName = GetDisplayName(deviceName);
		Region2<int> region = info.monitorInfo.rcMonitor.AsRegion();
		Region2<int> workRegion = info.monitorInfo.rcWork.AsRegion();
		_displays.Add(new Win32Display(displayName, region, workRegion));

		return true;
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

	public override Win32Window CreateView(in WindowConfiguration configuration)
	{
		throw new NotImplementedException();
	}

	public override bool ReleaseView(View view)
	{
		if (view is not Win32Window window)
		{
			return false;
		}

		return false;
	}
}