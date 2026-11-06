using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace RawSalt.Graphics.Rendering;

public sealed unsafe class UploadMapArena
{
	private readonly uint _alignment;

	private readonly byte* _mapped;
	private readonly uint _baseOffset;
	private readonly uint _capacity;
	private uint _head;

	public uint Used => _head;
	public uint Capacity => _capacity;

	public UploadMapArena(GraphicsDevice device, byte* mappedRegion, uint baseOffset, uint capacity)
	{
		Debug.Assert(device != null);
		_alignment = device.Adapter.Limits.MinUniformBufferOffsetAlignment;

		Debug.Assert(mappedRegion != null);
		Debug.Assert(baseOffset % _alignment == 0);
		_mapped = mappedRegion;
		_baseOffset = baseOffset;
		_capacity = capacity;
	}

	public UniformRef Allocate<T>(in T data)
		where T : unmanaged
	{
		uint size = (uint)Unsafe.SizeOf<T>();
		uint offset = (_head + _alignment - 1) & ~(_alignment - 1);
		if (offset + size > _capacity)
		{
			throw new InvalidOperationException($"Upload arena overflow ({_capacity} bytes).");
		}

		Unsafe.WriteUnaligned(_mapped + offset, data);
		_head = offset + size;
		return new UniformRef(_baseOffset + offset, size);
	}

	public void Reset() => _head = 0;
}