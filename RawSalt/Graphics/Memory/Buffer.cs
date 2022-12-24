//using NiTiS.OpenGL;
//using System;

//namespace RawSalt.Graphics.Memory;

///// <summary>
///// OpenGL memory buffer
///// </summary>
//public readonly unsafe struct Buffer
//{
//	/// <summary>
//	/// Buffer handle
//	/// </summary>
//	public readonly uint Handle;
//	/// <summary>
//	/// Buffer target
//	/// </summary>
//	public readonly BufferType Target;
//	/// <summary>
//	/// Create buffer object
//	/// </summary>
//	/// <remarks>
//	/// Must create OpenGL buffer with same <paramref name="handle"/> aka Buffer Name
//	/// </remarks>
//	/// <param name="handle">Buffer name</param>
//	/// <param name="target">Buffer specification</param>
//	public Buffer(uint handle, BufferType target)
//	{
//		Handle = handle;
//		Target = target;
//	}
//	public Buffer(BufferType target)
//	{
//		Handle = GL.CreateBuffer();
//		Target = target;
//	}
//	/// <summary>
//	/// Bind buffer
//	/// </summary>
//	public void Bind()
//	{
//		GL.BindBuffer(Target, Handle);
//	}
//	/// <summary>
//	/// Initialize data for buffer
//	/// </summary>
//	/// <typeparam name="T">Array type</typeparam>
//	/// <param name="data">Array</param>
//	/// <param name="usage">Buffer usage</param>
//	public void Data<T>(T[] data, BufferUsage usage = BufferUsage.StaticDraw)
//		where T : unmanaged
//	{
//		fixed (void* pData = data)
//		{
//			GL.BufferData(Target, (ptr)(sizeof(T) * data.Length), pData, usage);
//		}
//	}
//	/// <summary>
//	/// Initialize data for buffer
//	/// </summary>
//	/// <typeparam name="T">Pointer type</typeparam>
//	/// <param name="gl">OpenGL</param>
//	/// <param name="pData">Pointer to data</param>
//	/// <param name="dataSize">Data size</param>
//	/// <param name="usage">Buffer usage</param>
//	public void Data<T>(T* pData, ptr dataSize, BufferUsage usage = BufferUsage.StaticDraw)
//		where T : unmanaged
//	{
//		GL.BufferData(Target, dataSize, pData, usage);
//	}
//	/// <summary>
//	/// Initialize data for buffer
//	/// </summary>
//	/// <param name="gl">OpenGL</param>
//	/// <param name="pData">Pointer to data</param>
//	/// <param name="dataSize">Data size</param>
//	/// <param name="usage">Buffer usage</param>
//	public void Data(void* pData, ptr dataSize, BufferUsage usage = BufferUsage.StaticDraw)
//	{
//		GL.BufferData(Target, dataSize, pData, usage);
//	}
//	/// <summary>
//	/// Change buffer data
//	/// </summary>
//	/// <typeparam name="T">Array type</typeparam>
//	/// <param name="gl">OpenGL</param>
//	/// <param name="offset">Buffer data offset</param>
//	/// <param name="size">Data size</param>
//	/// <param name="data">Data</param>
//	public void SubData<T>(ptr offset, ptr size, T[] data)
//		where T : unmanaged
//	{
//		fixed (void* pData = data)
//		{
//			GL.BufferSubData(Target, (sptr)offset, size, pData);
//		}
//	}
//	/// <summary>
//	/// Change buffer data
//	/// </summary>
//	/// <typeparam name="T">Pointer type</typeparam>
//	/// <param name="gl">OpenGL</param>
//	/// <param name="offset">Buffer data offset</param>
//	/// <param name="size">Data size</param>
//	/// <param name="pData">Pointer to data</param>
//	public void SubData<T>(ptr offset, ptr size, T* pData)
//		where T : unmanaged
//	{
//		GL.BufferSubData(Target, (sptr)offset, size, pData);
//	}
//	/// <summary>
//	/// Change buffer data
//	/// </summary>
//	/// <param name="gl">OpenGL</param>
//	/// <param name="offset">Buffer data offset</param>
//	/// <param name="size">Data size</param>
//	/// <param name="pData">Pointer to data</param>
//	public void SubData(ptr offset, ptr size, void* pData)
//	{
//		GL.BufferSubData(Target, (sptr)offset, size, pData);
//	}
//	/// <inheritdoc/>
//	public override string ToString()
//		=> $"Buffer<{Target}>({Handle})";
//	public void Dispose()
//		=> GL.DeleteBuffer(Handle);
//}