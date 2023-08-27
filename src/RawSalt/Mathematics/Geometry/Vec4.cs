/// Generated with src/RawSalt.Generator/templates/vector.cs.liquid; please not edit this file

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Numerics;

namespace RawSalt.Maths;


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
	
	public Vec4(ReadOnlySpan<byte> data)
	{
		if (data.Length < sizeof(float) * Count)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<Vec4>(ref MemoryMarshal.GetReference(data));
	}


	public static Vec4 One
		=> new(
			1f,
			1f,
			1f,
			1f
			);

	public static Vec4 Zero
		=> new(
			0f,
			0f,
			0f,
			0f
			);

	/// <inheritdoc/>
	public readonly bool Equals(Vec4 other)
		=> this == other;
	
	/// <inheritdoc/>
	public override readonly bool Equals(object? other)
		=> other is Vec4 otherVector && this == otherVector;
	
	/// <inheritdoc/>
	public override readonly int GetHashCode()
		=> HashCode.Combine(this.x, this.y, this.z, this.w);
		
	/// <inheritdoc/>
	public override readonly string ToString()
		=> $"<{x}, {y}, {z}, {w}>";


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