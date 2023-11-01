using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RawSalt;

public static class MachineInfo
{
	public static Architecture Architecture
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return RuntimeInformation.ProcessArchitecture;
		}
	}

	public static bool IsSystem64Bit
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return Environment.Is64BitOperatingSystem;
		}
	}

	public static bool IsWindows
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return IsOSPlatform(OSPlatform.Windows);
		}
	}

	public static bool IsLinux
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return IsOSPlatform(OSPlatform.Linux);
		}
	}

	public static bool IsAndroid
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return IsOSPlatform(OSPlatform.Create("ANDROID"));
		}
	}

	public static bool IsMacos
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return IsOSPlatform(OSPlatform.OSX);
		}
	}

	public static bool IsIos
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return IsOSPlatform(OSPlatform.Create("IOS"));
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsOSPlatform(OSPlatform platform)
	{
		return RuntimeInformation.IsOSPlatform(platform);
	}
}
