using System;
using System.Runtime.InteropServices;

namespace NiTiS.Native;

public static class OSInfo
{
	public static bool IsProccess64Bit
		=> Environment.Is64BitProcess;
	public static bool IsSystem64Bit
		=> Environment.Is64BitProcess;
	public static bool IsWindows
		=> RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
	public static bool IsLinux
		=> RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
	public static bool IsAnroid
		=> RuntimeInformation.IsOSPlatform(OSPlatform.Create("ANDROID"));
	public static bool IsMacos
		=> RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
	public static bool IsIos
		=> RuntimeInformation.IsOSPlatform(OSPlatform.Create("IOS"));
}