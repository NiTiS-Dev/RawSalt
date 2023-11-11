using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RawSalt.Mathematics.Geometry;

[StructLayout(LayoutKind.Sequential)]
public partial struct Matrix4x4
{
	public const int ElementCount = 16;
	public const int RowCount = 4;
	public const int ColumnCount = 4;

	#region Elements
	/// <summary>
	/// The 1x1 element of matrix.
	/// </summary>
	public float M11;

	/// <summary>
	/// The 1x2 element of matrix.
	/// </summary>
	public float M12;

	/// <summary>
	/// The 1x3 element of matrix.
	/// </summary>
	public float M13;

	/// <summary>
	/// The 1x4 element of matrix.
	/// </summary>
	public float M14;

	/// <summary>
	/// The 2x1 element of matrix.
	/// </summary>
	public float M21;

	/// <summary>
	/// The 2x2 element of matrix.
	/// </summary>
	public float M22;

	/// <summary>
	/// The 2x3 element of matrix.
	/// </summary>
	public float M23;

	/// <summary>
	/// The 2x4 element of matrix.
	/// </summary>
	public float M24;

	/// <summary>
	/// The 3x1 element of matrix.
	/// </summary>
	public float M31;

	/// <summary>
	/// The 3x2 element of matrix.
	/// </summary>
	public float M32;

	/// <summary>
	/// The 3x3 element of matrix.
	/// </summary>
	public float M33;

	/// <summary>
	/// The 3x4 element of matrix.
	/// </summary>
	public float M34;

	/// <summary>
	/// The 4x1 element of matrix.
	/// </summary>
	public float M41;

	/// <summary>
	/// The 4x2 element of matrix.
	/// </summary>
	public float M42;

	/// <summary>
	/// The 4x3 element of matrix.
	/// </summary>
	public float M43;

	/// <summary>
	/// The 4x4 element of matrix.
	/// </summary>
	public float M44;


	#endregion

	#region Rows & Columns
	/// <summary>
	/// The 1 row of matrix.
	/// </summary>
	public readonly Vec4 R1
		=> new(
			M11,
			M12,
			M13,
			M14
			);
	/// <summary>
	/// The 2 row of matrix.
	/// </summary>
	public readonly Vec4 R2
		=> new(
			M21,
			M22,
			M23,
			M24
			);
	/// <summary>
	/// The 3 row of matrix.
	/// </summary>
	public readonly Vec4 R3
		=> new(
			M31,
			M32,
			M33,
			M34
			);
	/// <summary>
	/// The 4 row of matrix.
	/// </summary>
	public readonly Vec4 R4
		=> new(
			M41,
			M42,
			M43,
			M44
			);

	/// <summary>
	/// The 1 column of matrix.
	/// </summary>
	public readonly Vec4 C1
		=> new(
			M11,
			M21,
			M31,
			M41
			);
	/// <summary>
	/// The 2 column of matrix.
	/// </summary>
	public readonly Vec4 C2
		=> new(
			M12,
			M22,
			M32,
			M42
			);
	/// <summary>
	/// The 3 column of matrix.
	/// </summary>
	public readonly Vec4 C3
		=> new(
			M13,
			M23,
			M33,
			M43
			);
	/// <summary>
	/// The 4 column of matrix.
	/// </summary>
	public readonly Vec4 C4
		=> new(
			M14,
			M24,
			M34,
			M44
			);
	#endregion

	/// <summary>
	/// Creates a matrix with default values.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Matrix4x4()
		=> this = default;

	public Matrix4x4(ReadOnlySpan<float> data)
	{
		if (data.Length < ElementCount)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<Matrix4x4>(ref Unsafe.As<float, byte>( ref MemoryMarshal.GetReference(data)));
	}

	public Matrix4x4(ReadOnlySpan<byte> data)
	{
		if (data.Length < sizeof(float) * ElementCount)
			throw new ArgumentOutOfRangeException(nameof(data));

		this = Unsafe.ReadUnaligned<Matrix4x4>(ref MemoryMarshal.GetReference(data));
	}

	public float this[int row, int column]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get => AsRORaw()[row, column];

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => AsRaw()[row, column] = value;
	}

	public static Matrix4x4 Identity
	{
		get {
			Matrix4x4 retusa = default;

			retusa.M11 = 1;
			retusa.M22 = 1;
			retusa.M33 = 1;
			retusa.M44 = 1;

			return retusa;
		}
	}

	public readonly bool IsIdentity
		=> AsRORaw().IsIdentity;

	public Vec3 Translation
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get => Unsafe.As<Vector3, Vec3>(ref Unsafe.AsRef(AsRORaw().Translation));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => AsRaw().Translation = Unsafe.As<Vec3, Vector3>(ref value);
	}

	public static Matrix4x4 operator +(in Matrix4x4 lhs)
		=> (+lhs.AsRORaw()).AsStruct();

	public static Matrix4x4 operator -(in Matrix4x4 lhs)
		=> (-lhs.AsRORaw()).AsStruct();

	public static Matrix4x4 operator +(in Matrix4x4 lhs, in Matrix4x4 rhs)
		=> (lhs.AsRORaw() + rhs.AsRORaw()).AsStruct();

	public static Matrix4x4 operator -(in Matrix4x4 lhs, in Matrix4x4 rhs)
		=> (lhs.AsRORaw() - rhs.AsRORaw()).AsStruct();

	public static Matrix4x4 operator *(in Matrix4x4 lhs, in Matrix4x4 rhs)
		=> (lhs.AsRORaw() * rhs.AsRORaw()).AsStruct();

	public static Matrix4x4 operator *(in Matrix4x4 lhs, float rhs)
		=> (lhs.AsRORaw() * rhs).AsStruct();

	public static Matrix4x4 operator /(in Matrix4x4 lhs, in Matrix4x4 rhs)
		=> (lhs.AsRORaw() / rhs.AsRORaw()).AsStruct();

	public static Matrix4x4 operator /(in Matrix4x4 lhs, float rhs)
		=> (lhs.AsRORaw() / rhs).AsStruct();

	public static bool operator ==(in Matrix4x4 lhs, in Matrix4x4 rhs)
		=> lhs.AsRORaw() == rhs.AsRORaw();

	public static bool operator !=(in Matrix4x4 lhs, in Matrix4x4 rhs)
		=> lhs.AsRORaw() != rhs.AsRORaw();
}