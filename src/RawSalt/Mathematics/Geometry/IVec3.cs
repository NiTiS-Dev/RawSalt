/// Generated with src/RawSalt.Generator/templates/vector.cs.liquid; please not edit this file

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Numerics;

namespace RawSalt.Mathematics.Geometry;


[StructLayout(LayoutKind.Sequential)]
public struct IVec3 :
	IAdditionOperators<IVec3, IVec3, IVec3>,
	ISubtractionOperators<IVec3, IVec3, IVec3>,
	IMultiplyOperators<IVec3, IVec3, IVec3>,
	IDivisionOperators<IVec3, IVec3, IVec3>,
	IModulusOperators<IVec3, IVec3, IVec3>,
	IEquatable<IVec3>
{
	/// <summary>
	/// The X component of the vector.
	/// </summary>
	public int x;
	/// <summary>
	/// The Y component of the vector.
	/// </summary>
	public int y;
	/// <summary>
	/// The Z component of the vector.
	/// </summary>
	public int z;


	public const int Count = 3;

	/// <summary>
	/// Creates a vector with default values.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public IVec3()
		=> this = default;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public IVec3(int x, int y, int z)
		=> (this.x, this.y, this.z) = (x, y, z);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public IVec3(int all)
		=> (this.x, this.y, this.z) = (all, all, all);

	public IVec3(ReadOnlySpan<int> data)
	{
		if (data.Length < Count)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<IVec3>(ref Unsafe.As<int, byte>( ref MemoryMarshal.GetReference(data)));
	}
	/// <summary>
	/// Constructs vector by extending the <paramref name="xy"/> vector
	/// </summary>
	public IVec3(IVec2 xy, int z)
		=> (this.x, this.y, this.z) = (xy.x, xy.y, z);

	
	public IVec3(ReadOnlySpan<byte> data)
	{
		if (data.Length < sizeof(int) * Count)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<IVec3>(ref MemoryMarshal.GetReference(data));
	}


	public static IVec3 One
		=> new(1,1,1);

	public static IVec3 Zero
		=> new(0,0,0);

	/// <inheritdoc/>
	public readonly bool Equals(IVec3 other)
		=> this == other;
	
	/// <inheritdoc/>
	public override readonly bool Equals(object? other)
		=> other is IVec3 otherVector && this == otherVector;
	
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
	public static IVec3 Clamp(IVec3 value, IVec3 min, IVec3 max)
		=> Min(Max(value, min), max);

	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Dot(IVec3 lhs, IVec3 rhs)
	{
		return
			(lhs.x * rhs.x) +
			(lhs.y * rhs.y) +
			(lhs.z * rhs.z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IVec3 Max(IVec3 lhs, IVec3 rhs)
	{
		return new(
			int.Max(lhs.x, rhs.x),
			int.Max(lhs.y, rhs.y),
			int.Max(lhs.z, rhs.z)
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IVec3 Min(IVec3 lhs, IVec3 rhs)
	{
		return new(
			int.Max(lhs.x, rhs.x),
			int.Max(lhs.y, rhs.y),
			int.Max(lhs.z, rhs.z)
			);
	}

	#endregion

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(IVec3 lhs, IVec3 rhs)
	{
		return
			lhs.x == rhs.x &&
			lhs.y == rhs.y &&
			lhs.z == rhs.z 
			;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(IVec3 lhs, IVec3 rhs)
	{
		return
			lhs.x != rhs.x ||
			lhs.y != rhs.y ||
			lhs.z != rhs.z
			;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IVec3 operator +(IVec3 lhs, IVec3 rhs)
	{
		return new(
			lhs.x + rhs.x,
			lhs.y + rhs.y,
			lhs.z + rhs.z
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IVec3 operator -(IVec3 lhs, IVec3 rhs)
	{
		return new(
			lhs.x - rhs.x,
			lhs.y - rhs.y,
			lhs.z - rhs.z
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IVec3 operator *(IVec3 lhs, IVec3 rhs)
	{
		return new(
			lhs.x * rhs.x,
			lhs.y * rhs.y,
			lhs.z * rhs.z
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IVec3 operator *(IVec3 lhs, int rhs)
	{
		return new(
			lhs.x * rhs,
			lhs.y * rhs,
			lhs.z * rhs
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IVec3 operator /(IVec3 lhs, IVec3 rhs)
	{
		return new(
			lhs.x / rhs.x,
			lhs.y / rhs.y,
			lhs.z / rhs.z
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IVec3 operator /(IVec3 lhs, int rhs)
	{
		return new(
			lhs.x / rhs,
			lhs.y / rhs,
			lhs.z / rhs
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IVec3 operator %(IVec3 lhs, IVec3 rhs)
	{
		return new(
			lhs.x % rhs.x,
			lhs.y % rhs.y,
			lhs.z % rhs.z
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IVec3 operator %(IVec3 lhs, int rhs)
	{
		return new(
			lhs.x % rhs,
			lhs.y % rhs,
			lhs.z % rhs
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IVec3 operator +(IVec3 self)
	{
		return new(
			+self.x,
			+self.y,
			+self.z
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IVec3 operator -(IVec3 self)
	{
		return new(
			-self.x,
			-self.y,
			-self.z
			);
	}
}