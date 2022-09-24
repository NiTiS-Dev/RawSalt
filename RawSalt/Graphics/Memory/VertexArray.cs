using Silk.NET.OpenGL;

namespace RawSalt.Graphics.Memory;

public readonly struct VertexArray
{
	public readonly uint Handle;

	public VertexArray(uint handle)
	{
		Handle = handle;
	}
	public void Bind(GL gl)
		=> gl.BindVertexArray(Handle);
	public unsafe void AttributePointer(GL gl, uint index, int count, VertexAttribPointerType type, uint vertexSize, int offset)
	{
		gl.VertexAttribPointer(index, count, type, false, vertexSize, (void*)offset);
		gl.EnableVertexAttribArray(index);
	}
}