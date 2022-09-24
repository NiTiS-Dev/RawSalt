using System;
using System.Runtime.InteropServices;

namespace RawSalt.Core;

[StructLayout(LayoutKind.Explicit, Size = 1)]
public readonly struct PlatformType
{
	[FieldOffset(0)] private readonly Side side;
	[FieldOffset(0)] private readonly Platform platform;
	public PlatformType(Side side, Platform platform)
	{
		this.side |= side;
		this.platform |= platform;
	}
	public Side Side
	{
		get
		{
			Side side = this.side;

			byte result = (byte)((byte)side & 0b_0011_0000);

			return (Side)result;
		}
	}
	public Platform Platform
	{
		get
		{
			Platform platform = this.platform;

			byte result = (byte)((byte)platform & 0b_0000_1111);

			return (Platform)result;
		}
	}
}