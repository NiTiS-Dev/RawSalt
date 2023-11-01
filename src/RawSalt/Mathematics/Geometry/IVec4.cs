/// Generated with src/RawSalt.Generator/templates/vector.cs.liquid; please not edit this file

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RawSalt.Mathematics.Geometry;


[StructLayout(LayoutKind.Sequential)]
public struct IVec4 :
	IAdditionOperators<IVec4, IVec4, IVec4>,
	ISubtractionOperators<IVec4, IVec4, IVec4>,
	IMultiplyOperators<IVec4, IVec4, IVec4>,
	IDivisionOperators<IVec4, IVec4, IVec4>,
	IModulusOperators<IVec4, IVec4, IVec4>,
	IEquatable<IVec4>
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
	/// <summary>
	/// The W component of the vector.
	/// </summary>
	public int w;


	public const int Count = 4;

	/// <summary>
	/// Creates a vector with default values.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public IVec4()
		=> this = default;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public IVec4(int x, int y, int z, int w)
		=> (this.x, this.y, this.z, this.w) = (x, y, z, w);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public IVec4(int all)
		=> (this.x, this.y, this.z, this.w) = (all, all, all, all);

	public IVec4(ReadOnlySpan<int> data)
	{
		if (data.Length < Count)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<IVec4>(ref Unsafe.As<int, byte>(ref MemoryMarshal.GetReference(data)));
	}
	/// <summary>
	/// Constructs vector by extending the <paramref name="xy"/> vector
	/// </summary>
	public IVec4(IVec2 xy, int z, int w)
		=> (this.x, this.y, this.z, this.w) = (xy.x, xy.y, z, w);

	/// <summary>
	/// Constructs vector by extending the <paramref name="xyz"/> vector
	/// </summary>
	public IVec4(IVec3 xyz, int w)
		=> (this.x, this.y, this.z, this.w) = (xyz.x, xyz.y, xyz.z, w);


	public IVec4(ReadOnlySpan<byte> data)
	{
		if (data.Length < sizeof(int) * Count)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<IVec4>(ref MemoryMarshal.GetReference(data));
	}


	public static IVec4 One
		=> new(1, 1, 1, 1);

	public static IVec4 Zero
		=> new(0, 0, 0, 0);

	/// <inheritdoc/>
	public readonly bool Equals(IVec4 other)
		=> this == other;

	/// <inheritdoc/>
	public override readonly bool Equals(object? other)
		=> other is IVec4 otherVector && this == otherVector;

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
	public static IVec4 Clamp(IVec4 value, IVec4 min, IVec4 max)
		=> Min(Max(value, min), max);


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Dot(IVec4 lhs, IVec4 rhs)
	{
		return
			(lhs.x * rhs.x) +
			(lhs.y * rhs.y) +
			(lhs.z * rhs.z) +
			(lhs.w * rhs.w);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IVec4 Max(IVec4 lhs, IVec4 rhs)
	{
		return new(
			int.Max(lhs.x, rhs.x),
			int.Max(lhs.y, rhs.y),
			int.Max(lhs.z, rhs.z),
			int.Max(lhs.w, rhs.w)
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IVec4 Min(IVec4 lhs, IVec4 rhs)
	{
		return new(
			int.Max(lhs.x, rhs.x),
			int.Max(lhs.y, rhs.y),
			int.Max(lhs.z, rhs.z),
			int.Max(lhs.w, rhs.w)
			);
	}

	#endregion

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(IVec4 lhs, IVec4 rhs)
	{
		return
			lhs.x == rhs.x &&
			lhs.y == rhs.y &&
			lhs.z == rhs.z &&
			lhs.w == rhs.w
			;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(IVec4 lhs, IVec4 rhs)
	{
		return
			lhs.x != rhs.x ||
			lhs.y != rhs.y ||
			lhs.z != rhs.z ||
			lhs.w != rhs.w
			;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IVec4 operator +(IVec4 lhs, IVec4 rhs)
	{
		return new(
			lhs.x + rhs.x,
			lhs.y + rhs.y,
			lhs.z + rhs.z,
			lhs.w + rhs.w
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IVec4 operator -(IVec4 lhs, IVec4 rhs)
	{
		return new(
			lhs.x - rhs.x,
			lhs.y - rhs.y,
			lhs.z - rhs.z,
			lhs.w - rhs.w
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IVec4 operator *(IVec4 lhs, IVec4 rhs)
	{
		return new(
			lhs.x * rhs.x,
			lhs.y * rhs.y,
			lhs.z * rhs.z,
			lhs.w * rhs.w
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IVec4 operator *(IVec4 lhs, int rhs)
	{
		return new(
			lhs.x * rhs,
			lhs.y * rhs,
			lhs.z * rhs,
			lhs.w * rhs
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IVec4 operator /(IVec4 lhs, IVec4 rhs)
	{
		return new(
			lhs.x / rhs.x,
			lhs.y / rhs.y,
			lhs.z / rhs.z,
			lhs.w / rhs.w
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IVec4 operator /(IVec4 lhs, int rhs)
	{
		return new(
			lhs.x / rhs,
			lhs.y / rhs,
			lhs.z / rhs,
			lhs.w / rhs
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IVec4 operator %(IVec4 lhs, IVec4 rhs)
	{
		return new(
			lhs.x % rhs.x,
			lhs.y % rhs.y,
			lhs.z % rhs.z,
			lhs.w % rhs.w
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IVec4 operator %(IVec4 lhs, int rhs)
	{
		return new(
			lhs.x % rhs,
			lhs.y % rhs,
			lhs.z % rhs,
			lhs.w % rhs
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IVec4 operator +(IVec4 self)
	{
		return new(
			+self.x,
			+self.y,
			+self.z,
			+self.w
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IVec4 operator -(IVec4 self)
	{
		return new(
			-self.x,
			-self.y,
			-self.z,
			-self.w
			);
	}
}