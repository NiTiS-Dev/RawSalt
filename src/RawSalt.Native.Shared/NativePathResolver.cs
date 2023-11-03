using System;
using System.IO;
using System.Runtime.InteropServices;
using RuntimeInfo = System.Runtime.InteropServices.RuntimeInformation;

namespace RawSalt.Native;

internal static class NativePathResolver
{
	private static readonly string? RuntimeIdentifier;

	public static string GetAdditionalPath(string libname)
	{
		return Path.Combine("runtimes", RuntimeIdentifier!, "native", libname);
	}

	static NativePathResolver()
	{
		if (OperatingSystem.IsWindows())
		{
			RuntimeIdentifier = RuntimeInfo.OSArchitecture switch
			{
				Architecture.X64 => "win-x64",
				Architecture.X86 => "win-x86",
				Architecture.Arm64 => "win-arm64",
				Architecture.Arm => "win-arm",
				_ => throw new Exception($"Architecture ({RuntimeInfo.OSArchitecture}) is not supported!")
			};
		}
		else if (OperatingSystem.IsLinux() || OperatingSystem.IsAndroid())
		{
			RuntimeIdentifier = RuntimeInfo.ProcessArchitecture switch
			{
				Architecture.X64 => "linux-x64",
				Architecture.X86 => "linux-x86",
				Architecture.Arm64 => "linux-arm64",
				_ => throw new System.Exception($"Architecture ({RuntimeInfo.ProcessArchitecture}) is not supported!")
			};
		}
	}
}