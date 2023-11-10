/// Generated with src/RawSalt.Generator/templates/vector.cs.liquid; please not edit this file

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Numerics;

namespace RawSalt.Mathematics.Geometry;


[StructLayout(LayoutKind.Sequential)]
public struct BVec2 :
	// Looks like for bool operators none interfaces
	IEquatable<BVec2>
{
	/// <summary>
	/// The X component of the vector.
	/// </summary>
	public bool x;
	/// <summary>
	/// The Y component of the vector.
	/// </summary>
	public bool y;


	public const int Count = 2;

	/// <summary>
	/// Creates a vector with default values.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public BVec2()
		=> this = default;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public BVec2(bool x, bool y)
		=> (this.x, this.y) = (x, y);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public BVec2(bool all)
		=> (this.x, this.y) = (all, all);

	public BVec2(ReadOnlySpan<bool> data)
	{
		if (data.Length < Count)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<BVec2>(ref Unsafe.As<bool, byte>( ref MemoryMarshal.GetReference(data)));
	}
	
	public BVec2(ReadOnlySpan<byte> data)
	{
		if (data.Length < sizeof(bool) * Count)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<BVec2>(ref MemoryMarshal.GetReference(data));
	}


	public static BVec2 True
		=> new(true,true);

	public static BVec2 False
		=> new(false,false);

	/// <inheritdoc/>
	public readonly bool Equals(BVec2 other)
		=> this == other;
	
	/// <inheritdoc/>
	public override readonly bool Equals(object? other)
		=> other is BVec2 otherVector && this == otherVector;
	
	/// <inheritdoc/>
	public override readonly int GetHashCode()
		=> HashCode.Combine(this.x, this.y);
		
	/// <summary>
	/// Returns string representation of vector.
	/// </summary>
	public override readonly string ToString()
		=> $"<{x}, {y}>";


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(BVec2 lhs, BVec2 rhs)
	{
		return
			lhs.x == rhs.x &&
			lhs.y == rhs.y 
			;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(BVec2 lhs, BVec2 rhs)
	{
		return
			lhs.x != rhs.x ||
			lhs.y != rhs.y
			;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static BVec2 operator &(BVec2 lhs, BVec2 rhs)
	{
		return new(
			lhs.x & rhs.x,
			lhs.y & rhs.y
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static BVec2 operator |(BVec2 lhs, BVec2 rhs)
	{
		return new(
			lhs.x | rhs.x,
			lhs.y | rhs.y
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static BVec2 operator ^(BVec2 lhs, BVec2 rhs)
	{
		return new(
			lhs.x ^ rhs.x,
			lhs.y ^ rhs.y
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static BVec2 operator !(BVec2 self)
	{
		return new(
			!self.x,
			!self.y
			);
	}
}