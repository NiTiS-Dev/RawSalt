using System;
using System.Runtime.InteropServices;

namespace RawSalt.Structs;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct BitMap8
{
	private const byte BitDelta = 0b00000001;
	private byte map;
	public BitMap8(byte map)
	{
		this.map = map;
	}
	public bool this[int index]
	{
		get
		{
			if (index > 7 || index < 0)
				throw new IndexOutOfRangeException();

			return 0 != (map & (BitDelta << index));
		}
		set
		{
			if (index > 7 || index < 0)
				throw new IndexOutOfRangeException();

			if (value)
				map = (byte)(map | (BitDelta << index));
			else
				map = (byte)(map & ~(BitDelta << index));
		}
	}
	public static BitMap8 operator ~(BitMap8 value)
		=> new((byte)~value.map);
}