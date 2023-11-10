/// Generated with src/RawSalt.Generator/templates/matrix.cs.liquid; please not edit this file



using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Numerics;

namespace RawSalt.Mathematics.Geometry;

public struct DMat4x4
{
	#region Elements
	/// <summary>
	/// The 1x1 element of matrix.
	/// </summary>
	public double M11;

	/// <summary>
	/// The 1x2 element of matrix.
	/// </summary>
	public double M12;

	/// <summary>
	/// The 1x3 element of matrix.
	/// </summary>
	public double M13;

	/// <summary>
	/// The 1x4 element of matrix.
	/// </summary>
	public double M14;

	/// <summary>
	/// The 2x1 element of matrix.
	/// </summary>
	public double M21;

	/// <summary>
	/// The 2x2 element of matrix.
	/// </summary>
	public double M22;

	/// <summary>
	/// The 2x3 element of matrix.
	/// </summary>
	public double M23;

	/// <summary>
	/// The 2x4 element of matrix.
	/// </summary>
	public double M24;

	/// <summary>
	/// The 3x1 element of matrix.
	/// </summary>
	public double M31;

	/// <summary>
	/// The 3x2 element of matrix.
	/// </summary>
	public double M32;

	/// <summary>
	/// The 3x3 element of matrix.
	/// </summary>
	public double M33;

	/// <summary>
	/// The 3x4 element of matrix.
	/// </summary>
	public double M34;

	/// <summary>
	/// The 4x1 element of matrix.
	/// </summary>
	public double M41;

	/// <summary>
	/// The 4x2 element of matrix.
	/// </summary>
	public double M42;

	/// <summary>
	/// The 4x3 element of matrix.
	/// </summary>
	public double M43;

	/// <summary>
	/// The 4x4 element of matrix.
	/// </summary>
	public double M44;


	#endregion

	/// <summary>
	/// The 1 row of matrix.
	/// </summary>
	public readonly DVec4 R1
		=> new(
			M11,
			M12,
			M13,
			M14
			);
	/// <summary>
	/// The 2 row of matrix.
	/// </summary>
	public readonly DVec4 R2
		=> new(
			M21,
			M22,
			M23,
			M24
			);
	/// <summary>
	/// The 3 row of matrix.
	/// </summary>
	public readonly DVec4 R3
		=> new(
			M31,
			M32,
			M33,
			M34
			);
	/// <summary>
	/// The 4 row of matrix.
	/// </summary>
	public readonly DVec4 R4
		=> new(
			M41,
			M42,
			M43,
			M44
			);

	/// <summary>
	/// The 1 column of matrix.
	/// </summary>
	public readonly DVec4 C1
		=> new(
			M11,
			M21,
			M31,
			M41
			);
	/// <summary>
	/// The 2 column of matrix.
	/// </summary>
	public readonly DVec4 C2
		=> new(
			M12,
			M22,
			M32,
			M42
			);
	/// <summary>
	/// The 3 column of matrix.
	/// </summary>
	public readonly DVec4 C3
		=> new(
			M13,
			M23,
			M33,
			M43
			);
	/// <summary>
	/// The 4 column of matrix.
	/// </summary>
	public readonly DVec4 C4
		=> new(
			M14,
			M24,
			M34,
			M44
			);

	public const int ElementCount = 16;
	public const int RowCount = 4;
	public const int ColumnCount = 4;

	/// <summary>
	/// Creates a matrix with default values.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DMat4x4()
		=> this = default;

	public DMat4x4(ReadOnlySpan<double> data)
	{
		if (data.Length < ElementCount)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<DMat4x4>(ref Unsafe.As<double, byte>( ref MemoryMarshal.GetReference(data)));
	}

	public DMat4x4(ReadOnlySpan<byte> data)
	{
		if (data.Length < sizeof(double) * ElementCount)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<DMat4x4>(ref MemoryMarshal.GetReference(data));
	}

	public static DMat4x4 Identity
	{
		get {
			DMat4x4 retusa = default;

			retusa.M11 = 1;
			retusa.M22 = 1;
			retusa.M33 = 1;
			retusa.M44 = 1;

			return retusa;
		}
	}
}