/// Generated with src/RawSalt.Generator/templates/vector.cs.liquid; please not edit this file

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Numerics;

namespace RawSalt.Maths;


[StructLayout(LayoutKind.Sequential)]
public struct IVec2 :
	IAdditionOperators<IVec2, IVec2, IVec2>,
	ISubtractionOperators<IVec2, IVec2, IVec2>,
	IMultiplyOperators<IVec2, IVec2, IVec2>,
	IDivisionOperators<IVec2, IVec2, IVec2>,
	IModulusOperators<IVec2, IVec2, IVec2>,
	IEquatable<IVec2>
{
	/// <summary>
	/// The X component of the vector.
	/// </summary>
	public int x;
	/// <summary>
	/// The Y component of the vector.
	/// </summary>
	public int y;


	public const int Count = 2;

	/// <summary>
	/// Creates a vector with default values.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public IVec2()
		=> this = default;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public IVec2(int x, int y)
		=> (this.x, this.y) = (x, y);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public IVec2(int all)
		=> (this.x, this.y) = (all, all);

	public IVec2(ReadOnlySpan<int> data)
	{
		if (data.Length < Count)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<IVec2>(ref Unsafe.As<int, byte>( ref MemoryMarshal.GetReference(data)));
	}
	
	public IVec2(ReadOnlySpan<byte> data)
	{
		if (data.Length < sizeof(int) * Count)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<IVec2>(ref MemoryMarshal.GetReference(data));
	}


	public static IVec2 One
		=> new(
			1,
			1
			);

	public static IVec2 Zero
		=> new(
			0,
			0
			);

	/// <inheritdoc/>
	public readonly bool Equals(IVec2 other)
		=> this == other;
	
	/// <inheritdoc/>
	public override readonly bool Equals(object? other)
		=> other is IVec2 otherVector && this == otherVector;
	
	/// <inheritdoc/>
	public override readonly int GetHashCode()
		=> HashCode.Combine(this.x, this.y);
		
	/// <inheritdoc/>
	public override readonly string ToString()
		=> $"<{x}, {y}>";


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(IVec2 lhs, IVec2 rhs)
	{
		return
			lhs.x == rhs.x &&
			lhs.y == rhs.y 
			;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(IVec2 lhs, IVec2 rhs)
	{
		return
			lhs.x != rhs.x ||
			lhs.y != rhs.y
			;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IVec2 operator +(IVec2 lhs, IVec2 rhs)
	{
		return new(
			lhs.x + rhs.x,
			lhs.y + rhs.y
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IVec2 operator -(IVec2 lhs, IVec2 rhs)
	{
		return new(
			lhs.x - rhs.x,
			lhs.y - rhs.y
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IVec2 operator *(IVec2 lhs, IVec2 rhs)
	{
		return new(
			lhs.x * rhs.x,
			lhs.y * rhs.y
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IVec2 operator /(IVec2 lhs, IVec2 rhs)
	{
		return new(
			lhs.x / rhs.x,
			lhs.y / rhs.y
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IVec2 operator %(IVec2 lhs, IVec2 rhs)
	{
		return new(
			lhs.x % rhs.x,
			lhs.y % rhs.y
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IVec2 operator +(IVec2 self)
	{
		return new(
			+self.x,
			+self.y
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IVec2 operator -(IVec2 self)
	{
		return new(
			-self.x,
			-self.y
			);
	}
}