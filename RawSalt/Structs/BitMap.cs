using System;
using System.Runtime.InteropServices;

namespace RawSalt.Structs;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct BitMap
{
	private const byte BitDelta = 0b00000001;
	private byte map;
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

			map = (byte)(map | (BitDelta << index));
		}
	}
}