using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using RawSalt.Platforms;
using Monitor = RawSalt.Platforms.Monitor;

namespace RawSalt.Windowing.Win32;

public class Win32Platform : Platform
{
	[SupportedOSPlatform("windows5.0")]
	public override unsafe IReadOnlyList<Monitor> GetMonitors()
	{
		PInvoke.EnumDisplayMonitors(HDC.Null, null, MonitorEnumProc, default);

		return [];
	}

	[SupportedOSPlatform("windows5.0")]
	private unsafe BOOL MonitorEnumProc(HMONITOR hMonitor, HDC hdcMonitor, RECT* lprcMonitor, LPARAM dwData)
	{
		MONITORINFOEXW info;
		info.monitorInfo.cbSize = (uint)Unsafe.SizeOf<MONITORINFOEXW>();
		if (!PInvoke.GetMonitorInfo(hMonitor, (MONITORINFO*)&info))
		{
			throw new Win32Exception();
		}

		return true;
	}

	public override View CreateView(in WindowConfiguration configuration)
	{
		throw new NotImplementedException();
	}

	public override bool ReleaseView(View view)
	{
		throw new NotImplementedException();
	}
}