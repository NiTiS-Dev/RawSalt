using System.Diagnostics;
using System.Runtime.Versioning;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace RawSalt.Windowing.Win32;

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
			fixed (char* pClassName = $"RawSalt.Platforms.Win32.Class{_counter++}")
			{
				WNDCLASSEXW param = default;
				param.cbSize = (uint)sizeof(WNDCLASSEXW);
				param.hInstance = HInstance;
				param.lpszClassName = pClassName;
				param.lpfnWndProc = _wndproc;

				ushort result = PInvoke.RegisterClassEx(in param);
				Debug.Assert(result != 0);
				return result;
			}
		}
	}
}