using System;
using System.Runtime.InteropServices;

namespace NiTiS.Native;

public interface INativeLibraryContainer
{
	abstract LibFileSearchPath SearchType { get; }
	abstract string WindowsX86 { get; }
	abstract string WindowsX64 { get; }
	abstract string Linux { get; }
	abstract string Android { get; }
	abstract string MacOS { get; }
	abstract string IOS { get; }
	/// <summary>
	/// Return library name based on system type
	/// </summary>
	/// <returns>Library name with extension</returns>
	string Machine()
	{
		if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Create("ANDROID")))
			{
				return this.Android;
			}
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			{
				return this.Linux;
			}
			if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				return null;
			}
			if (!RuntimeInformation.IsOSPlatform(OSPlatform.Create("IOS")))
			{
				return this.MacOS;
			}
			return this.IOS;
		}
		else
		{
			if (!Environment.Is64BitProcess)
			{
				return this.WindowsX86;
			}
			return this.WindowsX64;
		}
	}
}
