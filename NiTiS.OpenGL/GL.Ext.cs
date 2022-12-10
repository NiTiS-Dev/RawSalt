using NiTiS.Native;
using System;

namespace NiTiS.OpenGL;

public static unsafe partial class GL
{
	static GL()
	{
#if GLFW
		NativeAPI.Initialize(typeof(GL), GLFW.Glfw.GetProcAddress);
#endif
	}
	public static uint CreateBuffer()
	{
		uint i = default;
		CreateBuffers(1, &i);
		return i;
	}
	public static void DeleteBuffer(uint buffer)
	{
		DeleteBuffers(0, &buffer);
	}
}
