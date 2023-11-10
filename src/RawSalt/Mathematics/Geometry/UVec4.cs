/// Generated with src/RawSalt.Generator/templates/vector.cs.liquid; please not edit this file

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Numerics;

namespace RawSalt.Mathematics.Geometry;


[StructLayout(LayoutKind.Sequential)]
public struct UVec4 :
	IAdditionOperators<UVec4, UVec4, UVec4>,
	ISubtractionOperators<UVec4, UVec4, UVec4>,
	IMultiplyOperators<UVec4, UVec4, UVec4>,
	IDivisionOperators<UVec4, UVec4, UVec4>,
	IModulusOperators<UVec4, UVec4, UVec4>,
	IEquatable<UVec4>
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
	/// <summary>
	/// The W component of the vector.
	/// </summary>
	public uint w;


	public const int Count = 4;

	/// <summary>
	/// Creates a vector with default values.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public UVec4()
		=> this = default;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public UVec4(uint x, uint y, uint z, uint w)
		=> (this.x, this.y, this.z, this.w) = (x, y, z, w);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public UVec4(uint all)
		=> (this.x, this.y, this.z, this.w) = (all, all, all, all);

	public UVec4(ReadOnlySpan<uint> data)
	{
		if (data.Length < Count)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<UVec4>(ref Unsafe.As<uint, byte>( ref MemoryMarshal.GetReference(data)));
	}
	/// <summary>
	/// Constructs vector by extending the <paramref name="xy"/> vector
	/// </summary>
	public UVec4(UVec2 xy, uint z, uint w)
		=> (this.x, this.y, this.z, this.w) = (xy.x, xy.y, z, w);

	/// <summary>
	/// Constructs vector by extending the <paramref name="xyz"/> vector
	/// </summary>
	public UVec4(UVec3 xyz, uint w)
		=> (this.x, this.y, this.z, this.w) = (xyz.x, xyz.y, xyz.z, w);

	
	public UVec4(ReadOnlySpan<byte> data)
	{
		if (data.Length < sizeof(uint) * Count)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<UVec4>(ref MemoryMarshal.GetReference(data));
	}


	public static UVec4 One
		=> new(1,1,1,1);

	public static UVec4 Zero
		=> new(0,0,0,0);

	/// <inheritdoc/>
	public readonly bool Equals(UVec4 other)
		=> this == other;
	
	/// <inheritdoc/>
	public override readonly bool Equals(object? other)
		=> other is UVec4 otherVector && this == otherVector;
	
	/// <inheritdoc/>
	public override readonly int GetHashCode()
		=> HashCode.Combine(this.x, this.y, this.z, this.w);
		
	/// <summary>
	/// Returns string representation of vector.
	/// </summary>
	public override readonly string ToString()
		=> $"<{x}, {y}, {z}, {w}>";

	#region Vector operations

	/// <summary>
	/// Restricts vector by <paramref name="max"/> and <paramref name="max"/> values.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static UVec4 Clamp(UVec4 value, UVec4 min, UVec4 max)
		=> Min(Max(value, min), max);

	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint Dot(UVec4 lhs, UVec4 rhs)
	{
		return
			(lhs.x * rhs.x) +
			(lhs.y * rhs.y) +
			(lhs.z * rhs.z) +
			(lhs.w * rhs.w);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static UVec4 Max(UVec4 lhs, UVec4 rhs)
	{
		return new(
			uint.Max(lhs.x, rhs.x),
			uint.Max(lhs.y, rhs.y),
			uint.Max(lhs.z, rhs.z),
			uint.Max(lhs.w, rhs.w)
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static UVec4 Min(UVec4 lhs, UVec4 rhs)
	{
		return new(
			uint.Max(lhs.x, rhs.x),
			uint.Max(lhs.y, rhs.y),
			uint.Max(lhs.z, rhs.z),
			uint.Max(lhs.w, rhs.w)
			);
	}

	#endregion

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(UVec4 lhs, UVec4 rhs)
	{
		return
			lhs.x == rhs.x &&
			lhs.y == rhs.y &&
			lhs.z == rhs.z &&
			lhs.w == rhs.w 
			;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(UVec4 lhs, UVec4 rhs)
	{
		return
			lhs.x != rhs.x ||
			lhs.y != rhs.y ||
			lhs.z != rhs.z ||
			lhs.w != rhs.w
			;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static UVec4 operator +(UVec4 lhs, UVec4 rhs)
	{
		return new(
			lhs.x + rhs.x,
			lhs.y + rhs.y,
			lhs.z + rhs.z,
			lhs.w + rhs.w
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static UVec4 operator -(UVec4 lhs, UVec4 rhs)
	{
		return new(
			lhs.x - rhs.x,
			lhs.y - rhs.y,
			lhs.z - rhs.z,
			lhs.w - rhs.w
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static UVec4 operator *(UVec4 lhs, UVec4 rhs)
	{
		return new(
			lhs.x * rhs.x,
			lhs.y * rhs.y,
			lhs.z * rhs.z,
			lhs.w * rhs.w
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static UVec4 operator *(UVec4 lhs, uint rhs)
	{
		return new(
			lhs.x * rhs,
			lhs.y * rhs,
			lhs.z * rhs,
			lhs.w * rhs
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static UVec4 operator /(UVec4 lhs, UVec4 rhs)
	{
		return new(
			lhs.x / rhs.x,
			lhs.y / rhs.y,
			lhs.z / rhs.z,
			lhs.w / rhs.w
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static UVec4 operator /(UVec4 lhs, uint rhs)
	{
		return new(
			lhs.x / rhs,
			lhs.y / rhs,
			lhs.z / rhs,
			lhs.w / rhs
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static UVec4 operator %(UVec4 lhs, UVec4 rhs)
	{
		return new(
			lhs.x % rhs.x,
			lhs.y % rhs.y,
			lhs.z % rhs.z,
			lhs.w % rhs.w
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static UVec4 operator %(UVec4 lhs, uint rhs)
	{
		return new(
			lhs.x % rhs,
			lhs.y % rhs,
			lhs.z % rhs,
			lhs.w % rhs
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static UVec4 operator +(UVec4 self)
	{
		return new(
			+self.x,
			+self.y,
			+self.z,
			+self.w
			);
	}

}