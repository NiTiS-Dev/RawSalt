using Windows.Win32.Foundation;
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
}