using System.Runtime.InteropServices;

namespace RawSalt.Logging;

internal static class WindowsConsole
{
	public static void EnableVirtualTerminalProcessing()
	{
        if (!OperatingSystem.IsWindows()) return;

		nint stdout = GetStdHandle(StandardOutputHandleId);
		if (stdout != (nint)InvalidHandleValue && GetConsoleMode(stdout, out uint mode))
		{
			SetConsoleMode(stdout, mode | EnableVirtualTerminalProcessingMode);
		}
	}

	const int StandardOutputHandleId = -11;
	const uint EnableVirtualTerminalProcessingMode = 4;
	const long InvalidHandleValue = -1;

	[DllImport("kernel32.dll", SetLastError = true)]
	static extern IntPtr GetStdHandle(int handleId);

	[DllImport("kernel32.dll", SetLastError = true)]
	static extern bool GetConsoleMode(IntPtr handle, out uint mode);

	[DllImport("kernel32.dll", SetLastError = true)]
	static extern bool SetConsoleMode(IntPtr handle, uint mode);
}