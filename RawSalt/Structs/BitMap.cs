using System;
using System.Runtime.InteropServices;

namespace RawSalt.Structs;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct BitMap
{
	private const byte BitDelta = 0b00000001;
	private byte map;
	public BitMap(byte map)
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
	public static BitMap operator ~(BitMap value)
		=> new((byte)~value.map);
}