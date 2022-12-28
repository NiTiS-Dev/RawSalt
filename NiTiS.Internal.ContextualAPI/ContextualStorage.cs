
using NiTiS.Native;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("NiTiS.OpenGL")]
[assembly:InternalsVisibleTo("NiTiS.GLFW")]

namespace NiTiS.Internal.ContextualAPI;

/// <summary>
/// Storage for contextual API's context
/// </summary>
internal static unsafe class ContextualStorage
{
	public static delegate* unmanaged[Stdcall]<CString, void*> openGL;
	public static delegate* unmanaged[Stdcall]<CString, void*> vulkan;
	public static delegate* unmanaged[Stdcall]<CString, void*> directX;
}
