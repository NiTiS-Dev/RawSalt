using System;
using System.Runtime.InteropServices;

namespace NiTiS.Native;

public interface INativeLibraryContainer
{
	abstract string WindowsX86 { get; }
	abstract string WindowsX64 { get; }
	abstract string Linux { get; }
	abstract string Android { get; }
	abstract string MacOS { get; }
	abstract string IOS { get; }
	string Machine()
		=> RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
		? Environment.Is64BitProcess
			? WindowsX64
			: WindowsX86
		: RuntimeInformation.IsOSPlatform(OSPlatform.Create("ANDROID"))
			? Android
		: RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
			? Linux
		: RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
			? RuntimeInformation.IsOSPlatform(OSPlatform.Create("IOS"))
				? IOS
			: MacOS
		: default
		;
}