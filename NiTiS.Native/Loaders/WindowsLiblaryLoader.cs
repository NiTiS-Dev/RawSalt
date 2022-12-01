using System;
using System.Runtime.InteropServices;

namespace NiTiS.Native.Loaders;

public sealed unsafe class WindowsLiblaryLoader : NativeLibraryLoader
{
	[DllImport("kernel32.dll")]
	private static extern void* LoadLibraryA(string path);
	[DllImport("kernel32.dll")]
	private static extern void* GetProcAddress(void* pModule, string name);
	[DllImport("kernel32.dll")]
	private static extern int FreeLibrary(void* module);
	internal override string AlternatePath
		=> Environment.Is64BitProcess
		? "runtimes/win-x64/native"
		: "runtimes/win-x86/native"
		;

	public override unsafe void* GetMethodAddress(LibraryHandle* pLib, string name)
		=> GetProcAddress(pLib, name);
	public override unsafe LibraryHandle* LoadLibrary(string path)
		=> (LibraryHandle*)LoadLibraryA(path);
	public override void UnloadLibrary(LibraryHandle* handle)
		=> FreeLibrary(handle);
}