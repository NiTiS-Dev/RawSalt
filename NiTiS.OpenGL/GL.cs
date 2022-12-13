using NiTiS.Native;

namespace NiTiS.OpenGL;

[NativeAPI(MethodPrefix = "gl", APIType = APIType.Contextual)]
public static unsafe partial class GL
{
	#region Clear
	public static delegate* unmanaged[Cdecl]<float, float, float, float, void> ClearColor;
	public static delegate* unmanaged[Cdecl]<ClearBufferMask, void> Clear;
	#endregion
	#region Buffers
	public static delegate* unmanaged[Cdecl]<uint, uint*, void> CreateBuffers;
	public static delegate* unmanaged[Cdecl]<BufferType, uint, void> BindBuffer;
	public static delegate* unmanaged[Cdecl]<BufferType, nuint, void*, BufferUsage, void> BufferData;
	public static delegate* unmanaged[Cdecl]<BufferType, nint, nuint, void*, void> BufferSubData;
	public static delegate* unmanaged[Cdecl]<uint, uint*, void> DeleteBuffers;
	#endregion


	private class __import : INativeLibraryContainer
	{
		public LibFileSearchPath SearchType
			=> OSInfo.IsWindows
			? LibFileSearchPath.ByPathVariable
			: LibFileSearchPath.NearByExeFile;

		public virtual string WindowsX86
			=> "opengl32.dll";
		public string WindowsX64
			=> WindowsX86;

		public string Linux
			=> "libGL.so.1";

		public string Android
			=> Linux;

		public string MacOS
			=> "/System/Library/Frameworks/OpenGL.framework/OpenGL";

		public string IOS
			=> MacOS;
	}
}