/// Generated with src/RawSalt.Generator/templates/vector.cs.liquid; please not edit this file

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Numerics;

namespace RawSalt.Mathematics.Geometry;


[StructLayout(LayoutKind.Sequential)]
public struct BVec3 :
	// Looks like for bool operators none interfaces
	IEquatable<BVec3>
{
	/// <summary>
	/// The X component of the vector.
	/// </summary>
	public bool x;
	/// <summary>
	/// The Y component of the vector.
	/// </summary>
	public bool y;
	/// <summary>
	/// The Z component of the vector.
	/// </summary>
	public bool z;


	public const int Count = 3;

	/// <summary>
	/// Creates a vector with default values.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public BVec3()
		=> this = default;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public BVec3(bool x, bool y, bool z)
		=> (this.x, this.y, this.z) = (x, y, z);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public BVec3(bool all)
		=> (this.x, this.y, this.z) = (all, all, all);

	public BVec3(ReadOnlySpan<bool> data)
	{
		if (data.Length < Count)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<BVec3>(ref Unsafe.As<bool, byte>( ref MemoryMarshal.GetReference(data)));
	}
	/// <summary>
	/// Constructs vector by extending the <paramref name="xy"/> vector
	/// </summary>
	public BVec3(BVec2 xy, bool z)
		=> (this.x, this.y, this.z) = (xy.x, xy.y, z);

	
	public BVec3(ReadOnlySpan<byte> data)
	{
		if (data.Length < sizeof(bool) * Count)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<BVec3>(ref MemoryMarshal.GetReference(data));
	}


	public static BVec3 True
		=> new(true,true,true);

	public static BVec3 False
		=> new(false,false,false);

	/// <inheritdoc/>
	public readonly bool Equals(BVec3 other)
		=> this == other;
	
	/// <inheritdoc/>
	public override readonly bool Equals(object? other)
		=> other is BVec3 otherVector && this == otherVector;
	
	/// <inheritdoc/>
	public override readonly int GetHashCode()
		=> HashCode.Combine(this.x, this.y, this.z);
		
	/// <summary>
	/// Returns string representation of vector.
	/// </summary>
	public override readonly string ToString()
		=> $"<{x}, {y}, {z}>";


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(BVec3 lhs, BVec3 rhs)
	{
		return
			lhs.x == rhs.x &&
			lhs.y == rhs.y &&
			lhs.z == rhs.z 
			;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(BVec3 lhs, BVec3 rhs)
	{
		return
			lhs.x != rhs.x ||
			lhs.y != rhs.y ||
			lhs.z != rhs.z
			;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static BVec3 operator &(BVec3 lhs, BVec3 rhs)
	{
		return new(
			lhs.x & rhs.x,
			lhs.y & rhs.y,
			lhs.z & rhs.z
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static BVec3 operator |(BVec3 lhs, BVec3 rhs)
	{
		return new(
			lhs.x | rhs.x,
			lhs.y | rhs.y,
			lhs.z | rhs.z
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static BVec3 operator ^(BVec3 lhs, BVec3 rhs)
	{
		return new(
			lhs.x ^ rhs.x,
			lhs.y ^ rhs.y,
			lhs.z ^ rhs.z
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static BVec3 operator !(BVec3 self)
	{
		return new(
			!self.x,
			!self.y,
			!self.z
			);
	}
}