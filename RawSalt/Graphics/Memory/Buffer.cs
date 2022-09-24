using Silk.NET.OpenGL;
using System.Runtime.CompilerServices;

namespace RawSalt.Graphics.Memory;

public readonly struct Buffer
{
	public readonly uint Handle;
	public Buffer(uint handle)
	{
		Handle = handle;
	}
	public void Bind(GL gl, BufferTargetARB bufferTarget)
	{
		gl.BindBuffer(bufferTarget, Handle);
	}
}