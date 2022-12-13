using NiTiS.Native;
using System;

namespace NiTiS.OpenGL;

public static unsafe partial class GL
{
	static GL()
	{
		if (NiTiS.Internal.ContextualAPI.ContextualStorage.openGL is null)
			throw new Exception("OpenGL has no context");

		NativeAPI.Initialize(typeof(GL), NiTiS.Internal.ContextualAPI.ContextualStorage.openGL);
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
