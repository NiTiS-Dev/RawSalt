using RawSalt.Platforms;

namespace RawSalt.Windowing.Win32;

public class Win32Window : Window, IWin32Window
{
	public nint Handle { get; }
	public nint ModuleHandle { get; }
}