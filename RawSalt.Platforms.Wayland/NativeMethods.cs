using System.Runtime.InteropServices;

namespace RawSalt.Windowing.Wayland;

/// <summary>
/// Minimal libc bindings required to poll the Wayland connection.
/// </summary>
internal static class LibC
{
	internal const short PollIn = 1;

	[DllImport("libc", SetLastError = true)]
	internal static extern int poll([In, Out] PollFd[] fds, nuint nfds, int timeout);
}

[StructLayout(LayoutKind.Sequential)]
internal struct PollFd
{
	public int fd;
	public short events;
	public short revents;
}
