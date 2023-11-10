/// Generated with src/RawSalt.Generator/templates/vector.cs.liquid; please not edit this file

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Numerics;

namespace RawSalt.Mathematics.Geometry;


[StructLayout(LayoutKind.Sequential)]
public struct UVec2 :
	IAdditionOperators<UVec2, UVec2, UVec2>,
	ISubtractionOperators<UVec2, UVec2, UVec2>,
	IMultiplyOperators<UVec2, UVec2, UVec2>,
	IDivisionOperators<UVec2, UVec2, UVec2>,
	IModulusOperators<UVec2, UVec2, UVec2>,
	IEquatable<UVec2>
{
	/// <summary>
	/// The X component of the vector.
	/// </summary>
	public uint x;
	/// <summary>
	/// The Y component of the vector.
	/// </summary>
	public uint y;


	public const int Count = 2;

	/// <summary>
	/// Creates a vector with default values.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public UVec2()
		=> this = default;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public UVec2(uint x, uint y)
		=> (this.x, this.y) = (x, y);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public UVec2(uint all)
		=> (this.x, this.y) = (all, all);

	public UVec2(ReadOnlySpan<uint> data)
	{
		if (data.Length < Count)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<UVec2>(ref Unsafe.As<uint, byte>( ref MemoryMarshal.GetReference(data)));
	}
	
	public UVec2(ReadOnlySpan<byte> data)
	{
		if (data.Length < sizeof(uint) * Count)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<UVec2>(ref MemoryMarshal.GetReference(data));
	}


	public static UVec2 One
		=> new(1,1);

	public static UVec2 Zero
		=> new(0,0);

	/// <inheritdoc/>
	public readonly bool Equals(UVec2 other)
		=> this == other;
	
	/// <inheritdoc/>
	public override readonly bool Equals(object? other)
		=> other is UVec2 otherVector && this == otherVector;
	
	/// <inheritdoc/>
	public override readonly int GetHashCode()
		=> HashCode.Combine(this.x, this.y);
		
	/// <summary>
	/// Returns string representation of vector.
	/// </summary>
	public override readonly string ToString()
		=> $"<{x}, {y}>";

	#region Vector operations

	/// <summary>
	/// Restricts vector by <paramref name="max"/> and <paramref name="max"/> values.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static UVec2 Clamp(UVec2 value, UVec2 min, UVec2 max)
		=> Min(Max(value, min), max);

	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint Dot(UVec2 lhs, UVec2 rhs)
	{
		return
			(lhs.x * rhs.x) +
			(lhs.y * rhs.y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static UVec2 Max(UVec2 lhs, UVec2 rhs)
	{
		return new(
			uint.Max(lhs.x, rhs.x),
			uint.Max(lhs.y, rhs.y)
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static UVec2 Min(UVec2 lhs, UVec2 rhs)
	{
		return new(
			uint.Max(lhs.x, rhs.x),
			uint.Max(lhs.y, rhs.y)
			);
	}

	#endregion

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(UVec2 lhs, UVec2 rhs)
	{
		return
			lhs.x == rhs.x &&
			lhs.y == rhs.y 
			;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(UVec2 lhs, UVec2 rhs)
	{
		return
			lhs.x != rhs.x ||
			lhs.y != rhs.y
			;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static UVec2 operator +(UVec2 lhs, UVec2 rhs)
	{
		return new(
			lhs.x + rhs.x,
			lhs.y + rhs.y
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static UVec2 operator -(UVec2 lhs, UVec2 rhs)
	{
		return new(
			lhs.x - rhs.x,
			lhs.y - rhs.y
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static UVec2 operator *(UVec2 lhs, UVec2 rhs)
	{
		return new(
			lhs.x * rhs.x,
			lhs.y * rhs.y
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static UVec2 operator *(UVec2 lhs, uint rhs)
	{
		return new(
			lhs.x * rhs,
			lhs.y * rhs
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static UVec2 operator /(UVec2 lhs, UVec2 rhs)
	{
		return new(
			lhs.x / rhs.x,
			lhs.y / rhs.y
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static UVec2 operator /(UVec2 lhs, uint rhs)
	{
		return new(
			lhs.x / rhs,
			lhs.y / rhs
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static UVec2 operator %(UVec2 lhs, UVec2 rhs)
	{
		return new(
			lhs.x % rhs.x,
			lhs.y % rhs.y
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static UVec2 operator %(UVec2 lhs, uint rhs)
	{
		return new(
			lhs.x % rhs,
			lhs.y % rhs
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static UVec2 operator +(UVec2 self)
	{
		return new(
			+self.x,
			+self.y
			);
	}

}