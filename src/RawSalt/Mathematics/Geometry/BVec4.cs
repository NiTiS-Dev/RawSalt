/// Generated with src/RawSalt.Generator/templates/vector.cs.liquid; please not edit this file

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RawSalt.Mathematics.Geometry;


[StructLayout(LayoutKind.Sequential)]
public struct BVec4 :
	// Looks like for bool operators none interfaces
	IEquatable<BVec4>
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
	/// <summary>
	/// The W component of the vector.
	/// </summary>
	public bool w;


	public const int Count = 4;

	/// <summary>
	/// Creates a vector with default values.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public BVec4()
		=> this = default;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public BVec4(bool x, bool y, bool z, bool w)
		=> (this.x, this.y, this.z, this.w) = (x, y, z, w);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public BVec4(bool all)
		=> (this.x, this.y, this.z, this.w) = (all, all, all, all);

	public BVec4(ReadOnlySpan<bool> data)
	{
		if (data.Length < Count)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<BVec4>(ref Unsafe.As<bool, byte>(ref MemoryMarshal.GetReference(data)));
	}
	/// <summary>
	/// Constructs vector by extending the <paramref name="xy"/> vector
	/// </summary>
	public BVec4(BVec2 xy, bool z, bool w)
		=> (this.x, this.y, this.z, this.w) = (xy.x, xy.y, z, w);

	/// <summary>
	/// Constructs vector by extending the <paramref name="xyz"/> vector
	/// </summary>
	public BVec4(BVec3 xyz, bool w)
		=> (this.x, this.y, this.z, this.w) = (xyz.x, xyz.y, xyz.z, w);


	public BVec4(ReadOnlySpan<byte> data)
	{
		if (data.Length < sizeof(bool) * Count)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<BVec4>(ref MemoryMarshal.GetReference(data));
	}


	public static BVec4 True
		=> new(true, true, true, true);

	public static BVec4 False
		=> new(false, false, false, false);

	/// <inheritdoc/>
	public readonly bool Equals(BVec4 other)
		=> this == other;

	/// <inheritdoc/>
	public override readonly bool Equals(object? other)
		=> other is BVec4 otherVector && this == otherVector;

	/// <inheritdoc/>
	public override readonly int GetHashCode()
		=> HashCode.Combine(this.x, this.y, this.z, this.w);

	/// <summary>
	/// Returns string representation of vector.
	/// </summary>
	public override readonly string ToString()
		=> $"<{x}, {y}, {z}, {w}>";


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(BVec4 lhs, BVec4 rhs)
	{
		return
			lhs.x == rhs.x &&
			lhs.y == rhs.y &&
			lhs.z == rhs.z &&
			lhs.w == rhs.w
			;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(BVec4 lhs, BVec4 rhs)
	{
		return
			lhs.x != rhs.x ||
			lhs.y != rhs.y ||
			lhs.z != rhs.z ||
			lhs.w != rhs.w
			;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static BVec4 operator &(BVec4 lhs, BVec4 rhs)
	{
		return new(
			lhs.x & rhs.x,
			lhs.y & rhs.y,
			lhs.z & rhs.z,
			lhs.w & rhs.w
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static BVec4 operator |(BVec4 lhs, BVec4 rhs)
	{
		return new(
			lhs.x | rhs.x,
			lhs.y | rhs.y,
			lhs.z | rhs.z,
			lhs.w | rhs.w
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static BVec4 operator ^(BVec4 lhs, BVec4 rhs)
	{
		return new(
			lhs.x ^ rhs.x,
			lhs.y ^ rhs.y,
			lhs.z ^ rhs.z,
			lhs.w ^ rhs.w
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static BVec4 operator !(BVec4 self)
	{
		return new(
			!self.x,
			!self.y,
			!self.z,
			!self.w
			);
	}
}