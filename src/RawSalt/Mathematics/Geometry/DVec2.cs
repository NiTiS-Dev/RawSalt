/// Generated with src/RawSalt.Generator/templates/vector.cs.liquid; please not edit this file

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Numerics;

namespace RawSalt.Mathematics.Geometry;


[StructLayout(LayoutKind.Sequential)]
public struct DVec2 :
	IAdditionOperators<DVec2, DVec2, DVec2>,
	ISubtractionOperators<DVec2, DVec2, DVec2>,
	IMultiplyOperators<DVec2, DVec2, DVec2>,
	IDivisionOperators<DVec2, DVec2, DVec2>,
	IModulusOperators<DVec2, DVec2, DVec2>,
	IEquatable<DVec2>
{
	/// <summary>
	/// The X component of the vector.
	/// </summary>
	public double x;
	/// <summary>
	/// The Y component of the vector.
	/// </summary>
	public double y;


	public const int Count = 2;

	/// <summary>
	/// Creates a vector with default values.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DVec2()
		=> this = default;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DVec2(double x, double y)
		=> (this.x, this.y) = (x, y);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DVec2(double all)
		=> (this.x, this.y) = (all, all);

	public DVec2(ReadOnlySpan<double> data)
	{
		if (data.Length < Count)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<DVec2>(ref Unsafe.As<double, byte>( ref MemoryMarshal.GetReference(data)));
	}
	
	public DVec2(ReadOnlySpan<byte> data)
	{
		if (data.Length < sizeof(double) * Count)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<DVec2>(ref MemoryMarshal.GetReference(data));
	}


	public static DVec2 One
		=> new(1,1);

	public static DVec2 Zero
		=> new(0,0);

	/// <inheritdoc/>
	public readonly bool Equals(DVec2 other)
		=> this == other;
	
	/// <inheritdoc/>
	public override readonly bool Equals(object? other)
		=> other is DVec2 otherVector && this == otherVector;
	
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
	public static DVec2 Clamp(DVec2 value, DVec2 min, DVec2 max)
		=> Min(Max(value, min), max);

	/// <summary>
	/// Computes distance between two points.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Distance(DVec2 firstPoint, DVec2 secondPoint)
		=> double.Sqrt(DistanceSquared(firstPoint, secondPoint));

	/// <summary>
	/// Computes distance squared between two points.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double DistanceSquared(DVec2 firstPoint, DVec2 secondPoint)
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
	public static double Dot(DVec2 lhs, DVec2 rhs)
	{
		return
			(lhs.x * rhs.x) +
			(lhs.y * rhs.y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DVec2 Max(DVec2 lhs, DVec2 rhs)
	{
		return new(
			double.Max(lhs.x, rhs.x),
			double.Max(lhs.y, rhs.y)
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DVec2 Min(DVec2 lhs, DVec2 rhs)
	{
		return new(
			double.Max(lhs.x, rhs.x),
			double.Max(lhs.y, rhs.y)
			);
	}

	#endregion

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(DVec2 lhs, DVec2 rhs)
	{
		return
			lhs.x == rhs.x &&
			lhs.y == rhs.y 
			;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(DVec2 lhs, DVec2 rhs)
	{
		return
			lhs.x != rhs.x ||
			lhs.y != rhs.y
			;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DVec2 operator +(DVec2 lhs, DVec2 rhs)
	{
		return new(
			lhs.x + rhs.x,
			lhs.y + rhs.y
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DVec2 operator -(DVec2 lhs, DVec2 rhs)
	{
		return new(
			lhs.x - rhs.x,
			lhs.y - rhs.y
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DVec2 operator *(DVec2 lhs, DVec2 rhs)
	{
		return new(
			lhs.x * rhs.x,
			lhs.y * rhs.y
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DVec2 operator *(DVec2 lhs, double rhs)
	{
		return new(
			lhs.x * rhs,
			lhs.y * rhs
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DVec2 operator /(DVec2 lhs, DVec2 rhs)
	{
		return new(
			lhs.x / rhs.x,
			lhs.y / rhs.y
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DVec2 operator /(DVec2 lhs, double rhs)
	{
		return new(
			lhs.x / rhs,
			lhs.y / rhs
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DVec2 operator %(DVec2 lhs, DVec2 rhs)
	{
		return new(
			lhs.x % rhs.x,
			lhs.y % rhs.y
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DVec2 operator %(DVec2 lhs, double rhs)
	{
		return new(
			lhs.x % rhs,
			lhs.y % rhs
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DVec2 operator +(DVec2 self)
	{
		return new(
			+self.x,
			+self.y
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DVec2 operator -(DVec2 self)
	{
		return new(
			-self.x,
			-self.y
			);
	}
}