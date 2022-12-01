using System;
using System.Runtime.InteropServices;

namespace NiTiS.Native.Loaders;

public abstract unsafe class NativeLibraryLoader
{
	public abstract LibraryHandle* LoadLibrary(string path);
	public abstract void UnloadLibrary(LibraryHandle* handle);
	public abstract void* GetMethodAddress(LibraryHandle* handle, string methodName);
	internal virtual string AlternatePath => null;
	public static NativeLibraryLoader GetPlatformDefaultLoader()
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			return new WindowsLiblaryLoader();
		}
		else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
		{
			return new UnixLibraryLoader();
		}
		else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.OSDescription.ToUpper().Contains("BSD"))
		{
			return new BSDLibraryNativeLoader();
		}

		return default;
	}
}
