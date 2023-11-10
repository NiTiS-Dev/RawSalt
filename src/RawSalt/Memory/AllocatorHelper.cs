using System;
using System.Runtime.InteropServices;
using System.Text;

namespace RawSalt.Memory;

public static unsafe class AllocatorHelper
{
	public static nint AllocString(string str)
	{
		byte[] utf8string = Encoding.UTF8.GetBytes(str);

		byte* mem = (byte*)Marshal.AllocHGlobal(utf8string.Length + 1);
		mem[utf8string.Length] = 0;

		fixed (byte* src = utf8string)
			new Span<byte>(src, utf8string.Length).CopyTo(new Span<byte>(mem, utf8string.Length));

		return (nint)mem;
	}
	public static nint AllocString(ReadOnlySpan<byte> utf8string)
	{
		byte* mem = (byte*)Marshal.AllocHGlobal(utf8string.Length + 1);
		mem[utf8string.Length] = 0;

		fixed (byte* src = utf8string)
			new Span<byte>(src, utf8string.Length).CopyTo(new Span<byte>(mem, utf8string.Length));

		return (nint)mem;
	}
	public static void FreeString(nint str)
	{
		Marshal.FreeHGlobal(str);
	}
}
