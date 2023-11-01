using System;

namespace RawSalt.Native.Windows;

public readonly struct HDC : IEquatable<HDC>
{
	public readonly nint Handle;

	public HDC(nint handle)
	{
		Handle = handle;
	}

	public static HDC Nil => new(0);

	public readonly bool Equals(HDC other)
	{
		return Handle == other.Handle;
	}

	public override bool Equals(object? obj)
	{
		return obj is HDC hDC && Equals(hDC);
	}

	public static bool operator ==(HDC left, HDC right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(HDC left, HDC right)
	{
		return !left.Equals(right);
	}

	public override int GetHashCode()
	{
		return (int)Handle;
	}
}
