/// Generated with src/RawSalt.Generator/templates/vector.cs.liquid; please not edit this file

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RawSalt.Mathematics.Geometry;


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

		this = Unsafe.ReadUnaligned<DVec4>(ref Unsafe.As<double, byte>(ref MemoryMarshal.GetReference(data)));
	}
	/// <summary>
	/// Constructs vector by extending the <paramref name="xy"/> vector
	/// </summary>
	public DVec4(DVec2 xy, double z, double w)
		=> (this.x, this.y, this.z, this.w) = (xy.x, xy.y, z, w);

	/// <summary>
	/// Constructs vector by extending the <paramref name="xyz"/> vector
	/// </summary>
	public DVec4(DVec3 xyz, double w)
		=> (this.x, this.y, this.z, this.w) = (xyz.x, xyz.y, xyz.z, w);


	public DVec4(ReadOnlySpan<byte> data)
	{
		if (data.Length < sizeof(double) * Count)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<DVec4>(ref MemoryMarshal.GetReference(data));
	}


	public static DVec4 One
		=> new(1, 1, 1, 1);

	public static DVec4 Zero
		=> new(0, 0, 0, 0);

	/// <inheritdoc/>
	public readonly bool Equals(DVec4 other)
		=> this == other;

	/// <inheritdoc/>
	public override readonly bool Equals(object? other)
		=> other is DVec4 otherVector && this == otherVector;

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
	public static DVec4 Clamp(DVec4 value, DVec4 min, DVec4 max)
		=> Min(Max(value, min), max);

	/// <summary>
	/// Computes distance between two points.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Distance(DVec4 firstPoint, DVec4 secondPoint)
		=> double.Sqrt(DistanceSquared(firstPoint, secondPoint));

	/// <summary>
	/// Computes distance squared between two points.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double DistanceSquared(DVec4 firstPoint, DVec4 secondPoint)
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
	public static double Dot(DVec4 lhs, DVec4 rhs)
	{
		return
			(lhs.x * rhs.x) +
			(lhs.y * rhs.y) +
			(lhs.z * rhs.z) +
			(lhs.w * rhs.w);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DVec4 Max(DVec4 lhs, DVec4 rhs)
	{
		return new(
			double.Max(lhs.x, rhs.x),
			double.Max(lhs.y, rhs.y),
			double.Max(lhs.z, rhs.z),
			double.Max(lhs.w, rhs.w)
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DVec4 Min(DVec4 lhs, DVec4 rhs)
	{
		return new(
			double.Max(lhs.x, rhs.x),
			double.Max(lhs.y, rhs.y),
			double.Max(lhs.z, rhs.z),
			double.Max(lhs.w, rhs.w)
			);
	}

	#endregion

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
	public static DVec4 operator *(DVec4 lhs, double rhs)
	{
		return new(
			lhs.x * rhs,
			lhs.y * rhs,
			lhs.z * rhs,
			lhs.w * rhs
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
	public static DVec4 operator /(DVec4 lhs, double rhs)
	{
		return new(
			lhs.x / rhs,
			lhs.y / rhs,
			lhs.z / rhs,
			lhs.w / rhs
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
	public static DVec4 operator %(DVec4 lhs, double rhs)
	{
		return new(
			lhs.x % rhs,
			lhs.y % rhs,
			lhs.z % rhs,
			lhs.w % rhs
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