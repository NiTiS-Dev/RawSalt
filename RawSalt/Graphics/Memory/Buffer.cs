using Silk.NET.OpenGL;

namespace RawSalt.Graphics.Memory;

/// <summary>
/// OpenGL memory buffer
/// </summary>
public readonly unsafe struct Buffer
{
	/// <summary>
	/// Buffer handle
	/// </summary>
	public readonly uint Handle;
	/// <summary>
	/// Buffer target
	/// </summary>
	public readonly BufferTargetARB Target;
	/// <summary>
	/// Create buffer object
	/// </summary>
	/// <remarks>
	/// Must create OpenGL buffer with same <paramref name="handle"/> aka Buffer Name
	/// </remarks>
	/// <param name="handle">Buffer name</param>
	/// <param name="target">Buffer specification</param>
	public Buffer(uint handle, BufferTargetARB target)
	{
		Handle = handle;
		Target = target;
	}
	/// <summary>
	/// Bind buffer
	/// </summary>
	/// <param name="gl">OpenGL</param>
	public void Bind(GL gl)
	{
		gl.BindBuffer(Target, Handle);
	}
	/// <summary>
	/// Initialize data for buffer
	/// </summary>
	/// <typeparam name="T">Array type</typeparam>
	/// <param name="gl">OpenGL</param>
	/// <param name="data">Array</param>
	/// <param name="bufferTarget">Buffer target</param>
	/// <param name="usage">Buffer usage</param>
	public void Data<T>(GL gl, T[] data, BufferUsageARB usage = BufferUsageARB.StaticDraw)
		where T : unmanaged
	{
		fixed (void* pData = data)
		{
			gl.BufferData(Target, (ptr)(sizeof(T) * data.Length), pData, usage);
		}
	}
	/// <summary>
	/// Initialize data for buffer
	/// </summary>
	/// <typeparam name="T">Pointer type</typeparam>
	/// <param name="gl">OpenGL</param>
	/// <param name="pData">Pointer to data</param>
	/// <param name="dataSize">Data size</param>
	/// <param name="usage">Buffer usage</param>
	public void Data<T>(GL gl, T* pData, ptr dataSize, BufferUsageARB usage = BufferUsageARB.StaticDraw)
		where T : unmanaged
	{
		gl.BufferData(Target, dataSize, pData, usage);
	}
	/// <summary>
	/// Initialize data for buffer
	/// </summary>
	/// <param name="gl">OpenGL</param>
	/// <param name="pData">Pointer to data</param>
	/// <param name="dataSize">Data size</param>
	/// <param name="usage">Buffer usage</param>
	public void Data(GL gl, void* pData, ptr dataSize, BufferUsageARB usage = BufferUsageARB.StaticDraw)
	{
		gl.BufferData(Target, dataSize, pData, usage);
	}
	/// <summary>
	/// Change buffer data
	/// </summary>
	/// <typeparam name="T">Array type</typeparam>
	/// <param name="gl">OpenGL</param>
	/// <param name="offset">Buffer data offset</param>
	/// <param name="size">Data size</param>
	/// <param name="data">Data</param>
	public void SubData<T>(GL gl, ptr offset, ptr size, T[] data)
		where T : unmanaged
	{
		fixed (void* pData = data)
		{
			gl.BufferSubData(Target, (sptr)offset, size, pData);
		}
	}
	/// <summary>
	/// Change buffer data
	/// </summary>
	/// <typeparam name="T">Pointer type</typeparam>
	/// <param name="gl">OpenGL</param>
	/// <param name="offset">Buffer data offset</param>
	/// <param name="size">Data size</param>
	/// <param name="pData">Pointer to data</param>
	public void SubData<T>(GL gl, ptr offset, ptr size, T* pData)
		where T : unmanaged
	{
		gl.BufferSubData(Target, (sptr)offset, size, pData);
	}
	/// <summary>
	/// Change buffer data
	/// </summary>
	/// <param name="gl">OpenGL</param>
	/// <param name="offset">Buffer data offset</param>
	/// <param name="size">Data size</param>
	/// <param name="pData">Pointer to data</param>
	public void SubData(GL gl, ptr offset, ptr size, void* pData)
	{
		gl.BufferSubData(Target, (sptr)offset, size, pData);
	}
	/// <inheritdoc/>
	public override string ToString()
		=> $"Buffer<{Target}>({Handle})";
}