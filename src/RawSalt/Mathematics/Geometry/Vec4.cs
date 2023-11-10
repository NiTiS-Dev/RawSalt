/// Generated with src/RawSalt.Generator/templates/vector.cs.liquid; please not edit this file

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Numerics;

namespace RawSalt.Mathematics.Geometry;


[StructLayout(LayoutKind.Sequential)]
public struct Vec4 :
	IAdditionOperators<Vec4, Vec4, Vec4>,
	ISubtractionOperators<Vec4, Vec4, Vec4>,
	IMultiplyOperators<Vec4, Vec4, Vec4>,
	IDivisionOperators<Vec4, Vec4, Vec4>,
	IModulusOperators<Vec4, Vec4, Vec4>,
	IEquatable<Vec4>
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
	/// <summary>
	/// The W component of the vector.
	/// </summary>
	public float w;


	public const int Count = 4;

	/// <summary>
	/// Creates a vector with default values.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vec4()
		=> this = default;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vec4(float x, float y, float z, float w)
		=> (this.x, this.y, this.z, this.w) = (x, y, z, w);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vec4(float all)
		=> (this.x, this.y, this.z, this.w) = (all, all, all, all);

	public Vec4(ReadOnlySpan<float> data)
	{
		if (data.Length < Count)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<Vec4>(ref Unsafe.As<float, byte>( ref MemoryMarshal.GetReference(data)));
	}
	/// <summary>
	/// Constructs vector by extending the <paramref name="xy"/> vector
	/// </summary>
	public Vec4(Vec2 xy, float z, float w)
		=> (this.x, this.y, this.z, this.w) = (xy.x, xy.y, z, w);

	/// <summary>
	/// Constructs vector by extending the <paramref name="xyz"/> vector
	/// </summary>
	public Vec4(Vec3 xyz, float w)
		=> (this.x, this.y, this.z, this.w) = (xyz.x, xyz.y, xyz.z, w);

	
	public Vec4(ReadOnlySpan<byte> data)
	{
		if (data.Length < sizeof(float) * Count)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<Vec4>(ref MemoryMarshal.GetReference(data));
	}


	public static Vec4 One
		=> new(1f,1f,1f,1f);

	public static Vec4 Zero
		=> new(0f,0f,0f,0f);

	/// <inheritdoc/>
	public readonly bool Equals(Vec4 other)
		=> this == other;
	
	/// <inheritdoc/>
	public override readonly bool Equals(object? other)
		=> other is Vec4 otherVector && this == otherVector;
	
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
	public static Vec4 Clamp(Vec4 value, Vec4 min, Vec4 max)
		=> Min(Max(value, min), max);

	/// <summary>
	/// Computes distance between two points.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Distance(Vec4 firstPoint, Vec4 secondPoint)
		=> float.Sqrt(DistanceSquared(firstPoint, secondPoint));

	/// <summary>
	/// Computes distance squared between two points.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float DistanceSquared(Vec4 firstPoint, Vec4 secondPoint)
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
	public static float Dot(Vec4 lhs, Vec4 rhs)
	{
		return
			(lhs.x * rhs.x) +
			(lhs.y * rhs.y) +
			(lhs.z * rhs.z) +
			(lhs.w * rhs.w);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec4 Max(Vec4 lhs, Vec4 rhs)
	{
		return new(
			float.Max(lhs.x, rhs.x),
			float.Max(lhs.y, rhs.y),
			float.Max(lhs.z, rhs.z),
			float.Max(lhs.w, rhs.w)
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec4 Min(Vec4 lhs, Vec4 rhs)
	{
		return new(
			float.Max(lhs.x, rhs.x),
			float.Max(lhs.y, rhs.y),
			float.Max(lhs.z, rhs.z),
			float.Max(lhs.w, rhs.w)
			);
	}

	#endregion

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(Vec4 lhs, Vec4 rhs)
	{
		return
			lhs.x == rhs.x &&
			lhs.y == rhs.y &&
			lhs.z == rhs.z &&
			lhs.w == rhs.w 
			;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(Vec4 lhs, Vec4 rhs)
	{
		return
			lhs.x != rhs.x ||
			lhs.y != rhs.y ||
			lhs.z != rhs.z ||
			lhs.w != rhs.w
			;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec4 operator +(Vec4 lhs, Vec4 rhs)
	{
		return new(
			lhs.x + rhs.x,
			lhs.y + rhs.y,
			lhs.z + rhs.z,
			lhs.w + rhs.w
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec4 operator -(Vec4 lhs, Vec4 rhs)
	{
		return new(
			lhs.x - rhs.x,
			lhs.y - rhs.y,
			lhs.z - rhs.z,
			lhs.w - rhs.w
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec4 operator *(Vec4 lhs, Vec4 rhs)
	{
		return new(
			lhs.x * rhs.x,
			lhs.y * rhs.y,
			lhs.z * rhs.z,
			lhs.w * rhs.w
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec4 operator *(Vec4 lhs, float rhs)
	{
		return new(
			lhs.x * rhs,
			lhs.y * rhs,
			lhs.z * rhs,
			lhs.w * rhs
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec4 operator /(Vec4 lhs, Vec4 rhs)
	{
		return new(
			lhs.x / rhs.x,
			lhs.y / rhs.y,
			lhs.z / rhs.z,
			lhs.w / rhs.w
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec4 operator /(Vec4 lhs, float rhs)
	{
		return new(
			lhs.x / rhs,
			lhs.y / rhs,
			lhs.z / rhs,
			lhs.w / rhs
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec4 operator %(Vec4 lhs, Vec4 rhs)
	{
		return new(
			lhs.x % rhs.x,
			lhs.y % rhs.y,
			lhs.z % rhs.z,
			lhs.w % rhs.w
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec4 operator %(Vec4 lhs, float rhs)
	{
		return new(
			lhs.x % rhs,
			lhs.y % rhs,
			lhs.z % rhs,
			lhs.w % rhs
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec4 operator +(Vec4 self)
	{
		return new(
			+self.x,
			+self.y,
			+self.z,
			+self.w
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec4 operator -(Vec4 self)
	{
		return new(
			-self.x,
			-self.y,
			-self.z,
			-self.w
			);
	}
}