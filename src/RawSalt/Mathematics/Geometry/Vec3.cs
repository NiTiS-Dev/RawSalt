/// Generated with src/RawSalt.Generator/templates/vector.cs.liquid; please not edit this file

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Numerics;

namespace RawSalt.Mathematics.Geometry;


[StructLayout(LayoutKind.Sequential)]
public struct Vec3 :
	IAdditionOperators<Vec3, Vec3, Vec3>,
	ISubtractionOperators<Vec3, Vec3, Vec3>,
	IMultiplyOperators<Vec3, Vec3, Vec3>,
	IDivisionOperators<Vec3, Vec3, Vec3>,
	IModulusOperators<Vec3, Vec3, Vec3>,
	IEquatable<Vec3>
{
	/// <summary>
	/// The X component of the vector.
	/// </summary>
	public float x;
	/// <summary>
	/// The Y component of the vector.
	/// </summary>
	public float y;
	/// <summary>
	/// The Z component of the vector.
	/// </summary>
	public float z;


	public const int Count = 3;

	/// <summary>
	/// Creates a vector with default values.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vec3()
		=> this = default;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vec3(float x, float y, float z)
		=> (this.x, this.y, this.z) = (x, y, z);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vec3(float all)
		=> (this.x, this.y, this.z) = (all, all, all);

	public Vec3(ReadOnlySpan<float> data)
	{
		if (data.Length < Count)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<Vec3>(ref Unsafe.As<float, byte>( ref MemoryMarshal.GetReference(data)));
	}
	/// <summary>
	/// Constructs vector by extending the <paramref name="xy"/> vector
	/// </summary>
	public Vec3(Vec2 xy, float z)
		=> (this.x, this.y, this.z) = (xy.x, xy.y, z);

	
	public Vec3(ReadOnlySpan<byte> data)
	{
		if (data.Length < sizeof(float) * Count)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<Vec3>(ref MemoryMarshal.GetReference(data));
	}


	public static Vec3 One
		=> new(1f,1f,1f);

	public static Vec3 Zero
		=> new(0f,0f,0f);

	/// <inheritdoc/>
	public readonly bool Equals(Vec3 other)
		=> this == other;
	
	/// <inheritdoc/>
	public override readonly bool Equals(object? other)
		=> other is Vec3 otherVector && this == otherVector;
	
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
	public static Vec3 Clamp(Vec3 value, Vec3 min, Vec3 max)
		=> Min(Max(value, min), max);

	/// <summary>
	/// Computes distance between two points.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Distance(Vec3 firstPoint, Vec3 secondPoint)
		=> float.Sqrt(DistanceSquared(firstPoint, secondPoint));

	/// <summary>
	/// Computes distance squared between two points.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float DistanceSquared(Vec3 firstPoint, Vec3 secondPoint)
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
	public static float Dot(Vec3 lhs, Vec3 rhs)
	{
		return
			(lhs.x * rhs.x) +
			(lhs.y * rhs.y) +
			(lhs.z * rhs.z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec3 Max(Vec3 lhs, Vec3 rhs)
	{
		return new(
			float.Max(lhs.x, rhs.x),
			float.Max(lhs.y, rhs.y),
			float.Max(lhs.z, rhs.z)
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec3 Min(Vec3 lhs, Vec3 rhs)
	{
		return new(
			float.Max(lhs.x, rhs.x),
			float.Max(lhs.y, rhs.y),
			float.Max(lhs.z, rhs.z)
			);
	}

	#endregion

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(Vec3 lhs, Vec3 rhs)
	{
		return
			lhs.x == rhs.x &&
			lhs.y == rhs.y &&
			lhs.z == rhs.z 
			;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(Vec3 lhs, Vec3 rhs)
	{
		return
			lhs.x != rhs.x ||
			lhs.y != rhs.y ||
			lhs.z != rhs.z
			;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec3 operator +(Vec3 lhs, Vec3 rhs)
	{
		return new(
			lhs.x + rhs.x,
			lhs.y + rhs.y,
			lhs.z + rhs.z
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec3 operator -(Vec3 lhs, Vec3 rhs)
	{
		return new(
			lhs.x - rhs.x,
			lhs.y - rhs.y,
			lhs.z - rhs.z
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec3 operator *(Vec3 lhs, Vec3 rhs)
	{
		return new(
			lhs.x * rhs.x,
			lhs.y * rhs.y,
			lhs.z * rhs.z
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec3 operator *(Vec3 lhs, float rhs)
	{
		return new(
			lhs.x * rhs,
			lhs.y * rhs,
			lhs.z * rhs
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec3 operator /(Vec3 lhs, Vec3 rhs)
	{
		return new(
			lhs.x / rhs.x,
			lhs.y / rhs.y,
			lhs.z / rhs.z
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec3 operator /(Vec3 lhs, float rhs)
	{
		return new(
			lhs.x / rhs,
			lhs.y / rhs,
			lhs.z / rhs
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec3 operator %(Vec3 lhs, Vec3 rhs)
	{
		return new(
			lhs.x % rhs.x,
			lhs.y % rhs.y,
			lhs.z % rhs.z
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec3 operator %(Vec3 lhs, float rhs)
	{
		return new(
			lhs.x % rhs,
			lhs.y % rhs,
			lhs.z % rhs
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec3 operator +(Vec3 self)
	{
		return new(
			+self.x,
			+self.y,
			+self.z
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec3 operator -(Vec3 self)
	{
		return new(
			-self.x,
			-self.y,
			-self.z
			);
	}
}