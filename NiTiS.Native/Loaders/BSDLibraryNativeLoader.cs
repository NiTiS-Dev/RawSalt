using System.Runtime.InteropServices;

namespace NiTiS.Native.Loaders;

public sealed unsafe class BSDLibraryNativeLoader : NativeLibraryLoader
{
	private const string LibAPI = "libdl";
	private const int RtldNow = 0x002;

	[DllImport(LibAPI)]
	private static extern void* dlopen(string path, int flags);
	[DllImport(LibAPI)]
	private static extern void* dlsym(void* pModule, string name);
	[DllImport(LibAPI)]
	private static extern int dlclose(void* module);

	public override unsafe void* GetMethodAddress(LibraryHandle* pLib, string name)
		=> dlsym(pLib, name);
	public override unsafe LibraryHandle* LoadLibrary(string path)
		=> (LibraryHandle*)dlopen(path, RtldNow);
	public override void UnloadLibrary(LibraryHandle* handle)
		=> dlclose(handle);
}
