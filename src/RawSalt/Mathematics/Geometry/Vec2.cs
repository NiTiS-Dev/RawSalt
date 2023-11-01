/// Generated with src/RawSalt.Generator/templates/vector.cs.liquid; please not edit this file

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RawSalt.Mathematics.Geometry;


[StructLayout(LayoutKind.Sequential)]
public struct Vec2 :
	IAdditionOperators<Vec2, Vec2, Vec2>,
	ISubtractionOperators<Vec2, Vec2, Vec2>,
	IMultiplyOperators<Vec2, Vec2, Vec2>,
	IDivisionOperators<Vec2, Vec2, Vec2>,
	IModulusOperators<Vec2, Vec2, Vec2>,
	IEquatable<Vec2>
{
	/// <summary>
	/// The X component of the vector.
	/// </summary>
	public float x;
	/// <summary>
	/// The Y component of the vector.
	/// </summary>
	public float y;


	public const int Count = 2;

	/// <summary>
	/// Creates a vector with default values.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vec2()
		=> this = default;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vec2(float x, float y)
		=> (this.x, this.y) = (x, y);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vec2(float all)
		=> (this.x, this.y) = (all, all);

	public Vec2(ReadOnlySpan<float> data)
	{
		if (data.Length < Count)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<Vec2>(ref Unsafe.As<float, byte>(ref MemoryMarshal.GetReference(data)));
	}

	public Vec2(ReadOnlySpan<byte> data)
	{
		if (data.Length < sizeof(float) * Count)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<Vec2>(ref MemoryMarshal.GetReference(data));
	}


	public static Vec2 One
		=> new(1f, 1f);

	public static Vec2 Zero
		=> new(0f, 0f);

	/// <inheritdoc/>
	public readonly bool Equals(Vec2 other)
		=> this == other;

	/// <inheritdoc/>
	public override readonly bool Equals(object? other)
		=> other is Vec2 otherVector && this == otherVector;

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
	public static Vec2 Clamp(Vec2 value, Vec2 min, Vec2 max)
		=> Min(Max(value, min), max);

	/// <summary>
	/// Computes distance between two points.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Distance(Vec2 firstPoint, Vec2 secondPoint)
		=> float.Sqrt(DistanceSquared(firstPoint, secondPoint));

	/// <summary>
	/// Computes distance squared between two points.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float DistanceSquared(Vec2 firstPoint, Vec2 secondPoint)
	{
		var diff = secondPoint - firstPoint;
		return Dot(diff, diff);
	}

	public readonly float LengthSquared
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Dot(this, this);
	}

	public readonly float Length
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => float.Sqrt(LengthSquared);
	}


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Dot(Vec2 lhs, Vec2 rhs)
	{
		return
			(lhs.x * rhs.x) +
			(lhs.y * rhs.y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec2 Max(Vec2 lhs, Vec2 rhs)
	{
		return new(
			float.Max(lhs.x, rhs.x),
			float.Max(lhs.y, rhs.y)
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec2 Min(Vec2 lhs, Vec2 rhs)
	{
		return new(
			float.Max(lhs.x, rhs.x),
			float.Max(lhs.y, rhs.y)
			);
	}

	#endregion

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(Vec2 lhs, Vec2 rhs)
	{
		return
			lhs.x == rhs.x &&
			lhs.y == rhs.y
			;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(Vec2 lhs, Vec2 rhs)
	{
		return
			lhs.x != rhs.x ||
			lhs.y != rhs.y
			;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec2 operator +(Vec2 lhs, Vec2 rhs)
	{
		return new(
			lhs.x + rhs.x,
			lhs.y + rhs.y
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec2 operator -(Vec2 lhs, Vec2 rhs)
	{
		return new(
			lhs.x - rhs.x,
			lhs.y - rhs.y
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec2 operator *(Vec2 lhs, Vec2 rhs)
	{
		return new(
			lhs.x * rhs.x,
			lhs.y * rhs.y
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec2 operator *(Vec2 lhs, float rhs)
	{
		return new(
			lhs.x * rhs,
			lhs.y * rhs
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec2 operator /(Vec2 lhs, Vec2 rhs)
	{
		return new(
			lhs.x / rhs.x,
			lhs.y / rhs.y
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec2 operator /(Vec2 lhs, float rhs)
	{
		return new(
			lhs.x / rhs,
			lhs.y / rhs
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec2 operator %(Vec2 lhs, Vec2 rhs)
	{
		return new(
			lhs.x % rhs.x,
			lhs.y % rhs.y
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec2 operator %(Vec2 lhs, float rhs)
	{
		return new(
			lhs.x % rhs,
			lhs.y % rhs
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec2 operator +(Vec2 self)
	{
		return new(
			+self.x,
			+self.y
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec2 operator -(Vec2 self)
	{
		return new(
			-self.x,
			-self.y
			);
	}
}