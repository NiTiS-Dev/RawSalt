
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
	internal static delegate* unmanaged[Cdecl]<CString, void*> openGL;
	internal static delegate* unmanaged[Cdecl]<CString, void*> vulkan;
	internal static delegate* unmanaged[Cdecl]<CString, void*> directX;
}
