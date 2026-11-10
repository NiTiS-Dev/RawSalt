using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using RawSalt.Logging;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.WindowsAndMessaging;
using Microsoft.Win32;

namespace RawSalt.Platforms.Win32;

public partial class Win32Platform
{
	private static volatile int _counter = 0;

	internal HINSTANCE HInstance { get; }
	private ushort _classAtom;
	private readonly WNDPROC _wndproc;

	[SupportedOSPlatform("windows5.0")]
	private unsafe void RegisterClass()
	{
		if (_classAtom != 0) return;

		ushort newClass = RegisterClassSlow();
		if (Interlocked.CompareExchange(ref _classAtom, newClass, 0) != 0)
		{
			PInvoke.UnregisterClass((PCWSTR)(void*)_classAtom, HInstance);
		}

		ushort RegisterClassSlow()
		{
			int index = Interlocked.Increment(ref _counter);
			fixed (char* pClassName = $"RawSalt.Platforms.Win32.Class{index}")
			{
				WNDCLASSEXW param = default;
				param.cbSize = (uint)sizeof(WNDCLASSEXW);
				param.hInstance = HInstance;
				param.hbrBackground = (HBRUSH)(nint)PInvoke.GetStockObject(GET_STOCK_OBJECT_FLAGS.BLACK_BRUSH);
				param.hCursor = PInvoke.LoadCursor(default, PInvoke.IDC_ARROW);
				param.lpszClassName = pClassName;
				param.lpfnWndProc = _wndproc;

				ushort result = PInvoke.RegisterClassEx(in param);
				if (result == 0)
				{
					Log.Error("RegisterClassEx failed: {0}", Marshal.GetLastPInvokeError());
				}

				return result;
			}
		}
	}

	public static bool IsDarkModeEnabled()
	{
		if (!OperatingSystem.IsWindows()) return false;

		try
		{
			using var key = Registry.CurrentUser.OpenSubKey(
				@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
			object? value = key?.GetValue("AppsUseLightTheme");
			if (value is int i)
			{
				return i == 0; // 0 = dark, 1 = light
			}
		}
		catch
		{
			// Ignore failures and default to light mode
		}

		return false;
	}
}