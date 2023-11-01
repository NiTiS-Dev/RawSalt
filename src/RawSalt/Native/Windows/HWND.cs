using System;

namespace RawSalt.Native.Windows;

public readonly struct HWND : IEquatable<HWND>
{
	public readonly nint Handle;

	public HWND(nint handle)
	{
		Handle = handle;
	}

	public static HWND Nil => new(0);

	public readonly bool Equals(HWND other)
	{
		return Handle == other.Handle;
	}

	public override bool Equals(object? obj)
	{
		return obj is HWND hWND && Equals(hWND);
	}

	public static bool operator ==(HWND left, HWND right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(HWND left, HWND right)
	{
		return !left.Equals(right);
	}

	public override int GetHashCode()
	{
		return (int)Handle;
	}
}
