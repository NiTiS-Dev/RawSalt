/// Generated with src/RawSalt.Generator/templates/vector.cs.liquid; please not edit this file

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RawSalt.Mathematics.Geometry;


[StructLayout(LayoutKind.Sequential)]
public struct DVec3 :
	IAdditionOperators<DVec3, DVec3, DVec3>,
	ISubtractionOperators<DVec3, DVec3, DVec3>,
	IMultiplyOperators<DVec3, DVec3, DVec3>,
	IDivisionOperators<DVec3, DVec3, DVec3>,
	IModulusOperators<DVec3, DVec3, DVec3>,
	IEquatable<DVec3>
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


	public const int Count = 3;

	/// <summary>
	/// Creates a vector with default values.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DVec3()
		=> this = default;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DVec3(double x, double y, double z)
		=> (this.x, this.y, this.z) = (x, y, z);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DVec3(double all)
		=> (this.x, this.y, this.z) = (all, all, all);

	public DVec3(ReadOnlySpan<double> data)
	{
		if (data.Length < Count)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<DVec3>(ref Unsafe.As<double, byte>(ref MemoryMarshal.GetReference(data)));
	}
	/// <summary>
	/// Constructs vector by extending the <paramref name="xy"/> vector
	/// </summary>
	public DVec3(DVec2 xy, double z)
		=> (this.x, this.y, this.z) = (xy.x, xy.y, z);


	public DVec3(ReadOnlySpan<byte> data)
	{
		if (data.Length < sizeof(double) * Count)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<DVec3>(ref MemoryMarshal.GetReference(data));
	}


	public static DVec3 One
		=> new(1, 1, 1);

	public static DVec3 Zero
		=> new(0, 0, 0);

	/// <inheritdoc/>
	public readonly bool Equals(DVec3 other)
		=> this == other;

	/// <inheritdoc/>
	public override readonly bool Equals(object? other)
		=> other is DVec3 otherVector && this == otherVector;

	/// <inheritdoc/>
	public override readonly int GetHashCode()
		=> HashCode.Combine(this.x, this.y, this.z);

	/// <summary>
	/// Returns string representation of vector.
	/// </summary>
	public override readonly string ToString()
		=> $"<{x}, {y}, {z}>";

	#region Vector operations

	/// <summary>
	/// Restricts vector by <paramref name="max"/> and <paramref name="max"/> values.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DVec3 Clamp(DVec3 value, DVec3 min, DVec3 max)
		=> Min(Max(value, min), max);

	/// <summary>
	/// Computes distance between two points.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Distance(DVec3 firstPoint, DVec3 secondPoint)
		=> double.Sqrt(DistanceSquared(firstPoint, secondPoint));

	/// <summary>
	/// Computes distance squared between two points.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double DistanceSquared(DVec3 firstPoint, DVec3 secondPoint)
	{
		var diff = secondPoint - firstPoint;
		return Dot(diff, diff);
	}

	public readonly double LengthSquared
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Dot(this, this);
	}

	public readonly double Length
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => double.Sqrt(LengthSquared);
	}


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Dot(DVec3 lhs, DVec3 rhs)
	{
		return
			(lhs.x * rhs.x) +
			(lhs.y * rhs.y) +
			(lhs.z * rhs.z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DVec3 Max(DVec3 lhs, DVec3 rhs)
	{
		return new(
			double.Max(lhs.x, rhs.x),
			double.Max(lhs.y, rhs.y),
			double.Max(lhs.z, rhs.z)
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DVec3 Min(DVec3 lhs, DVec3 rhs)
	{
		return new(
			double.Max(lhs.x, rhs.x),
			double.Max(lhs.y, rhs.y),
			double.Max(lhs.z, rhs.z)
			);
	}

	#endregion

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(DVec3 lhs, DVec3 rhs)
	{
		return
			lhs.x == rhs.x &&
			lhs.y == rhs.y &&
			lhs.z == rhs.z
			;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(DVec3 lhs, DVec3 rhs)
	{
		return
			lhs.x != rhs.x ||
			lhs.y != rhs.y ||
			lhs.z != rhs.z
			;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DVec3 operator +(DVec3 lhs, DVec3 rhs)
	{
		return new(
			lhs.x + rhs.x,
			lhs.y + rhs.y,
			lhs.z + rhs.z
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DVec3 operator -(DVec3 lhs, DVec3 rhs)
	{
		return new(
			lhs.x - rhs.x,
			lhs.y - rhs.y,
			lhs.z - rhs.z
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DVec3 operator *(DVec3 lhs, DVec3 rhs)
	{
		return new(
			lhs.x * rhs.x,
			lhs.y * rhs.y,
			lhs.z * rhs.z
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DVec3 operator *(DVec3 lhs, double rhs)
	{
		return new(
			lhs.x * rhs,
			lhs.y * rhs,
			lhs.z * rhs
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DVec3 operator /(DVec3 lhs, DVec3 rhs)
	{
		return new(
			lhs.x / rhs.x,
			lhs.y / rhs.y,
			lhs.z / rhs.z
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DVec3 operator /(DVec3 lhs, double rhs)
	{
		return new(
			lhs.x / rhs,
			lhs.y / rhs,
			lhs.z / rhs
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DVec3 operator %(DVec3 lhs, DVec3 rhs)
	{
		return new(
			lhs.x % rhs.x,
			lhs.y % rhs.y,
			lhs.z % rhs.z
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DVec3 operator %(DVec3 lhs, double rhs)
	{
		return new(
			lhs.x % rhs,
			lhs.y % rhs,
			lhs.z % rhs
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DVec3 operator +(DVec3 self)
	{
		return new(
			+self.x,
			+self.y,
			+self.z
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DVec3 operator -(DVec3 self)
	{
		return new(
			-self.x,
			-self.y,
			-self.z
			);
	}
}