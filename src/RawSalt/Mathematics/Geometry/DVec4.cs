/// Generated with src/RawSalt.Generator/templates/vector.cs.liquid; please not edit this file

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Numerics;

namespace RawSalt.Maths;


[StructLayout(LayoutKind.Sequential)]
public struct DVec4 :
	IAdditionOperators<DVec4, DVec4, DVec4>,
	ISubtractionOperators<DVec4, DVec4, DVec4>,
	IMultiplyOperators<DVec4, DVec4, DVec4>,
	IDivisionOperators<DVec4, DVec4, DVec4>,
	IModulusOperators<DVec4, DVec4, DVec4>,
	IEquatable<DVec4>
{
	/// <summary>
	/// The X component of the vector.
	/// </summary>
	public double x;
	/// <summary>
	/// The Y component of the vector.
	/// </summary>
	public double y;
	/// <summary>
	/// The Z component of the vector.
	/// </summary>
	public double z;
	/// <summary>
	/// The W component of the vector.
	/// </summary>
	public double w;


	public const int Count = 4;

	/// <summary>
	/// Creates a vector with default values.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DVec4()
		=> this = default;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DVec4(double x, double y, double z, double w)
		=> (this.x, this.y, this.z, this.w) = (x, y, z, w);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DVec4(double all)
		=> (this.x, this.y, this.z, this.w) = (all, all, all, all);

	public DVec4(ReadOnlySpan<double> data)
	{
		if (data.Length < Count)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<DVec4>(ref Unsafe.As<double, byte>( ref MemoryMarshal.GetReference(data)));
	}
	
	public DVec4(ReadOnlySpan<byte> data)
	{
		if (data.Length < sizeof(double) * Count)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<DVec4>(ref MemoryMarshal.GetReference(data));
	}


	public static DVec4 One
		=> new(
			1,
			1,
			1,
			1
			);

	public static DVec4 Zero
		=> new(
			0,
			0,
			0,
			0
			);

	/// <inheritdoc/>
	public readonly bool Equals(DVec4 other)
		=> this == other;
	
	/// <inheritdoc/>
	public override readonly bool Equals(object? other)
		=> other is DVec4 otherVector && this == otherVector;
	
	/// <inheritdoc/>
	public override readonly int GetHashCode()
		=> HashCode.Combine(this.x, this.y, this.z, this.w);
		
	/// <inheritdoc/>
	public override readonly string ToString()
		=> $"<{x}, {y}, {z}, {w}>";


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(DVec4 lhs, DVec4 rhs)
	{
		return
			lhs.x == rhs.x &&
			lhs.y == rhs.y &&
			lhs.z == rhs.z &&
			lhs.w == rhs.w 
			;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(DVec4 lhs, DVec4 rhs)
	{
		return
			lhs.x != rhs.x ||
			lhs.y != rhs.y ||
			lhs.z != rhs.z ||
			lhs.w != rhs.w
			;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DVec4 operator +(DVec4 lhs, DVec4 rhs)
	{
		return new(
			lhs.x + rhs.x,
			lhs.y + rhs.y,
			lhs.z + rhs.z,
			lhs.w + rhs.w
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DVec4 operator -(DVec4 lhs, DVec4 rhs)
	{
		return new(
			lhs.x - rhs.x,
			lhs.y - rhs.y,
			lhs.z - rhs.z,
			lhs.w - rhs.w
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DVec4 operator *(DVec4 lhs, DVec4 rhs)
	{
		return new(
			lhs.x * rhs.x,
			lhs.y * rhs.y,
			lhs.z * rhs.z,
			lhs.w * rhs.w
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DVec4 operator /(DVec4 lhs, DVec4 rhs)
	{
		return new(
			lhs.x / rhs.x,
			lhs.y / rhs.y,
			lhs.z / rhs.z,
			lhs.w / rhs.w
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DVec4 operator %(DVec4 lhs, DVec4 rhs)
	{
		return new(
			lhs.x % rhs.x,
			lhs.y % rhs.y,
			lhs.z % rhs.z,
			lhs.w % rhs.w
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DVec4 operator +(DVec4 self)
	{
		return new(
			+self.x,
			+self.y,
			+self.z,
			+self.w
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DVec4 operator -(DVec4 self)
	{
		return new(
			-self.x,
			-self.y,
			-self.z,
			-self.w
			);
	}
}