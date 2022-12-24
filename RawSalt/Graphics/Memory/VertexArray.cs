//namespace RawSalt.Graphics.Memory;

//public readonly struct VertexArray
//{
//	public readonly uint Handle;

//	public VertexArray(uint handle)
//	{
//		Handle = handle;
//	}
//	public VertexArray()
//	{
//		Handle = GL.CreateVertexArray();
//	} 
//	public void Bind()
//		=> GL.BindVertexArray(Handle);
//	public unsafe void AttributePointer(uint index, int count, VertexAttribPointerType type, uint vertexSize, int offset)
//	{
//		gl.VertexAttribPointer(index, count, type, false, vertexSize, (void*)offset);
//		gl.EnableVertexAttribArray(index);
//	}
//	public unsafe void AttributePointer<T>(uint index, int count, VertexAttribPointerType type, uint vertexSize, int offset)
//		where T : unmanaged
//	{
//		gl.VertexAttribPointer(index, count, type, false, (uint)(sizeof(T) * vertexSize), (void*)(sizeof(T) * offset));
//		gl.EnableVertexAttribArray(index);
//	}
//	public void Dispose(GL gl)
//		=> gl.DeleteVertexArray(Handle);
//}