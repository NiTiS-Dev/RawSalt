/// Generated with src/RawSalt.Generator/templates/vector.cs.liquid; please not edit this file

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Numerics;

namespace RawSalt.Maths;


[StructLayout(LayoutKind.Sequential)]
public struct UVec3 :
	IAdditionOperators<UVec3, UVec3, UVec3>,
	ISubtractionOperators<UVec3, UVec3, UVec3>,
	IMultiplyOperators<UVec3, UVec3, UVec3>,
	IDivisionOperators<UVec3, UVec3, UVec3>,
	IModulusOperators<UVec3, UVec3, UVec3>,
	IEquatable<UVec3>
{
	/// <summary>
	/// The X component of the vector.
	/// </summary>
	public uint x;
	/// <summary>
	/// The Y component of the vector.
	/// </summary>
	public uint y;
	/// <summary>
	/// The Z component of the vector.
	/// </summary>
	public uint z;


	public const int Count = 3;

	/// <summary>
	/// Creates a vector with default values.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public UVec3()
		=> this = default;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public UVec3(uint x, uint y, uint z)
		=> (this.x, this.y, this.z) = (x, y, z);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public UVec3(uint all)
		=> (this.x, this.y, this.z) = (all, all, all);

	public UVec3(ReadOnlySpan<uint> data)
	{
		if (data.Length < Count)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<UVec3>(ref Unsafe.As<uint, byte>( ref MemoryMarshal.GetReference(data)));
	}
	
	public UVec3(ReadOnlySpan<byte> data)
	{
		if (data.Length < sizeof(uint) * Count)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<UVec3>(ref MemoryMarshal.GetReference(data));
	}


	public static UVec3 One
		=> new(
			1,
			1,
			1
			);

	public static UVec3 Zero
		=> new(
			0,
			0,
			0
			);

	/// <inheritdoc/>
	public readonly bool Equals(UVec3 other)
		=> this == other;
	
	/// <inheritdoc/>
	public override readonly bool Equals(object? other)
		=> other is UVec3 otherVector && this == otherVector;
	
	/// <inheritdoc/>
	public override readonly int GetHashCode()
		=> HashCode.Combine(this.x, this.y, this.z);
		
	/// <inheritdoc/>
	public override readonly string ToString()
		=> $"<{x}, {y}, {z}>";


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(UVec3 lhs, UVec3 rhs)
	{
		return
			lhs.x == rhs.x &&
			lhs.y == rhs.y &&
			lhs.z == rhs.z 
			;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(UVec3 lhs, UVec3 rhs)
	{
		return
			lhs.x != rhs.x ||
			lhs.y != rhs.y ||
			lhs.z != rhs.z
			;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static UVec3 operator +(UVec3 lhs, UVec3 rhs)
	{
		return new(
			lhs.x + rhs.x,
			lhs.y + rhs.y,
			lhs.z + rhs.z
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static UVec3 operator -(UVec3 lhs, UVec3 rhs)
	{
		return new(
			lhs.x - rhs.x,
			lhs.y - rhs.y,
			lhs.z - rhs.z
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static UVec3 operator *(UVec3 lhs, UVec3 rhs)
	{
		return new(
			lhs.x * rhs.x,
			lhs.y * rhs.y,
			lhs.z * rhs.z
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static UVec3 operator /(UVec3 lhs, UVec3 rhs)
	{
		return new(
			lhs.x / rhs.x,
			lhs.y / rhs.y,
			lhs.z / rhs.z
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static UVec3 operator %(UVec3 lhs, UVec3 rhs)
	{
		return new(
			lhs.x % rhs.x,
			lhs.y % rhs.y,
			lhs.z % rhs.z
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static UVec3 operator +(UVec3 self)
	{
		return new(
			+self.x,
			+self.y,
			+self.z
			);
	}

}