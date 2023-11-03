using System.Runtime.InteropServices;

namespace RawSalt.Native.Windows;

public static partial class Kernel32
{
	public const string LibName = "kernel32.dll";

	[DllImport(LibName, CallingConvention = CallingConvention.Winapi)]
	public static extern HWND GetConsoleWindow();
}
