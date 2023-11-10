/// Generated with src/RawSalt.Generator/templates/matrix.cs.liquid; please not edit this file



using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Numerics;

namespace RawSalt.Mathematics.Geometry;

public struct DMat3x3
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


	#endregion

	/// <summary>
	/// The 1 row of matrix.
	/// </summary>
	public readonly DVec3 R1
		=> new(
			M11,
			M12,
			M13
			);
	/// <summary>
	/// The 2 row of matrix.
	/// </summary>
	public readonly DVec3 R2
		=> new(
			M21,
			M22,
			M23
			);
	/// <summary>
	/// The 3 row of matrix.
	/// </summary>
	public readonly DVec3 R3
		=> new(
			M31,
			M32,
			M33
			);

	/// <summary>
	/// The 1 column of matrix.
	/// </summary>
	public readonly DVec3 C1
		=> new(
			M11,
			M21,
			M31
			);
	/// <summary>
	/// The 2 column of matrix.
	/// </summary>
	public readonly DVec3 C2
		=> new(
			M12,
			M22,
			M32
			);
	/// <summary>
	/// The 3 column of matrix.
	/// </summary>
	public readonly DVec3 C3
		=> new(
			M13,
			M23,
			M33
			);

	public const int ElementCount = 9;
	public const int RowCount = 3;
	public const int ColumnCount = 3;

	/// <summary>
	/// Creates a matrix with default values.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DMat3x3()
		=> this = default;

	public DMat3x3(ReadOnlySpan<double> data)
	{
		if (data.Length < ElementCount)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<DMat3x3>(ref Unsafe.As<double, byte>( ref MemoryMarshal.GetReference(data)));
	}

	public DMat3x3(ReadOnlySpan<byte> data)
	{
		if (data.Length < sizeof(double) * ElementCount)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<DMat3x3>(ref MemoryMarshal.GetReference(data));
	}

	public static DMat3x3 Identity
	{
		get {
			DMat3x3 retusa = default;

			retusa.M11 = 1;
			retusa.M22 = 1;
			retusa.M33 = 1;

			return retusa;
		}
	}
}