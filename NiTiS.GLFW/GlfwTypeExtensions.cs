using System.Runtime.CompilerServices;

namespace NiTiS.GLFW;

public static class GlfwTypeExtensions
{
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static bool ToSystem(this GlfwBool@bool)
		=> @bool == GlfwBool.True ? true : false;

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static GlfwBool ToGlfw(this bool@bool)
		=> @bool == true ? GlfwBool.True : GlfwBool.False;
}
