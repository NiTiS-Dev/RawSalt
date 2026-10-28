using System.Runtime.Versioning;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Dwm;
using Windows.Win32.UI.WindowsAndMessaging;
using RawSalt.Platforms;

namespace RawSalt.Windowing.Win32;

public class Win32Window : Window, IWin32Window
{
	private readonly Win32Platform _platform;

	internal HWND Handle { get; }

	nint IWin32Window.Handle => Handle;
	nint IWin32Window.ModuleHandle => _platform.HInstance;

	internal Win32Window(HWND handle, Win32Platform platform)
	{
		Handle = handle;
		_platform = platform;
	}

	[SupportedOSPlatform("windows5.0")]
	public override void PumpEvents()
	{
		while (PInvoke.PeekMessage(out MSG msg, Handle, 0, 0, PEEK_MESSAGE_REMOVE_TYPE.PM_REMOVE))
		{
			PInvoke.TranslateMessage(msg);
			PInvoke.DispatchMessage(msg);
		}
	}
}