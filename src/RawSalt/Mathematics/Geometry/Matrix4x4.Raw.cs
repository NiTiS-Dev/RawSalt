using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace RawSalt.Mathematics.Geometry;

public partial struct Matrix4x4
{
	[UnscopedRef]
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	internal ref Raw AsRaw()
		=> ref Unsafe.As<Matrix4x4, Raw>(ref this);

	[UnscopedRef]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal readonly ref readonly Raw AsRORaw()
		=> ref Unsafe.As<Matrix4x4, Raw>(ref Unsafe.AsRef(in this));

	internal struct Raw
	{
		[UnscopedRef]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal ref Matrix4x4 AsStruct()
		=> ref Unsafe.As<Raw, Matrix4x4>(ref this);


		public Vector4 Row1;
		public Vector4 Row2;
		public Vector4 Row3;
		public Vector4 Row4;

		private const float BillboardEpsilon = 1e-4f;
		private const float BillboardMinAngle = 1.0f - (0.1f * (MathF.PI / 180.0f)); // 0.1 degrees
		private const float DecomposeEpsilon = 0.0001f;

		public static Raw Identity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				Raw retusa;

				retusa.Row1 = Vector4.UnitX;
				retusa.Row2 = Vector4.UnitY;
				retusa.Row3 = Vector4.UnitZ;
				retusa.Row4 = Vector4.UnitW;

				return retusa;
			}
		}

		public float this[int row, int column]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				if ((uint)row >= RowCount)
					throw new ArgumentOutOfRangeException(nameof(row));

				return Unsafe.Add(ref Unsafe.AsRef(in Row1), row)[column];
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				if ((uint)row >= RowCount)
					throw new ArgumentOutOfRangeException(nameof(row));

				Unsafe.Add(ref Row1, row)[column] = value;
			}
		}

		public readonly bool IsIdentity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Row1 == Vector4.UnitX
					&& Row2 == Vector4.UnitY
					&& Row3 == Vector4.UnitZ
					&& Row4 == Vector4.UnitW;
			}
		}

		public Vector3 Translation
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get => new(Row4.X, Row4.Y, Row4.Z);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				Row4 = new Vector4(value, Row4.W);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Raw operator +(in Raw left, in Raw right)
		{
			Raw retusa;

			retusa.Row1 = left.Row1 + right.Row1;
			retusa.Row2 = left.Row2 + right.Row2;
			retusa.Row3 = left.Row3 + right.Row3;
			retusa.Row4 = left.Row4 + right.Row4;

			return retusa;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Raw operator -(in Raw left, in Raw right)
		{
			Raw retusa;

			retusa.Row1 = left.Row1 - right.Row1;
			retusa.Row2 = left.Row2 - right.Row2;
			retusa.Row3 = left.Row3 - right.Row3;
			retusa.Row4 = left.Row4 - right.Row4;

			return retusa;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Raw operator *(in Raw left, in Raw right)
		{
			Raw retusa;

			retusa.Row1 = left.Row1 * right.Row1;
			retusa.Row2 = left.Row2 * right.Row2;
			retusa.Row3 = left.Row3 * right.Row3;
			retusa.Row4 = left.Row4 * right.Row4;

			return retusa;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Raw operator /(in Raw left, in Raw right)
		{
			Raw retusa;

			retusa.Row1 = left.Row1 / right.Row1;
			retusa.Row2 = left.Row2 / right.Row2;
			retusa.Row3 = left.Row3 / right.Row3;
			retusa.Row4 = left.Row4 / right.Row4;

			return retusa;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(in Raw left, in Raw right)
		{
			return left.Row1 == right.Row1
				&& left.Row2 == right.Row2
				&& left.Row3 == right.Row3
				&& left.Row4 == right.Row4;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(in Raw left, in Raw right)
		{
			return left.Row1 != right.Row1
				|| left.Row2 != right.Row2
				|| left.Row3 != right.Row3
				|| left.Row4 != right.Row4;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Raw CreateFromAxisAngle(in Vector3 axis, float angle)
		{
			float x = axis.X;
			float y = axis.Y;
			float z = axis.Z;

			float sa = MathF.Sin(angle);
			float ca = MathF.Cos(angle);

			float xx = x * x;
			float yy = y * y;
			float zz = z * z;

			float xy = x * y;
			float xz = x * z;
			float yz = y * z;

			Raw retusa;

			retusa.Row1 = new Vector4(
				xx + ca * (1.0f - xx),
				xy - ca * xy + sa * z,
				xz - ca * xz - sa * y,
				0
			);
			retusa.Row2 = new Vector4(
				xy - ca * xy - sa * z,
				yy + ca * (1.0f - yy),
				yz - ca * yz + sa * x,
				0
			);
			retusa.Row3 = new Vector4(
				xz - ca * xz + sa * y,
				yz - ca * yz - sa * x,
				zz + ca * (1.0f - zz),
				0
			);
			retusa.Row4 = Vector4.UnitW;

			return retusa;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Raw CreateFromQuaternion(in Quaternion quaternion)
		{
			float xx = quaternion.X * quaternion.X;
			float yy = quaternion.Y * quaternion.Y;
			float zz = quaternion.Z * quaternion.Z;

			float xy = quaternion.X * quaternion.Y;
			float wz = quaternion.Z * quaternion.W;
			float xz = quaternion.Z * quaternion.X;
			float wy = quaternion.Y * quaternion.W;
			float yz = quaternion.Y * quaternion.Z;
			float wx = quaternion.X * quaternion.W;

			Raw retusa;

			retusa.Row1 = new Vector4(
				1.0f - 2.0f * (yy + zz),
				2.0f * (xy + wz),
				2.0f * (xz - wy),
				0
			);
			retusa.Row2 = new Vector4(
				2.0f * (xy - wz),
				1.0f - 2.0f * (zz + xx),
				2.0f * (yz + wx),
				0
			);
			retusa.Row3 = new Vector4(
				2.0f * (xz + wy),
				2.0f * (yz - wx),
				1.0f - 2.0f * (yy + xx),
				0
			);
			retusa.Row4 = Vector4.UnitW;

			return retusa;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Raw CreateFromYawPitchRoll(float yaw, float pitch, float roll)
		{
			Quaternion q = Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll);
			return CreateFromQuaternion(q);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Raw CreateLookTo(in Vector3 cameraPosition, in Vector3 cameraDirection, in Vector3 cameraUpVector)
		{
			Vector3 axisZ = Vector3.Normalize(-cameraDirection);
			Vector3 axisX = Vector3.Normalize(Vector3.Cross(cameraUpVector, axisZ));
			Vector3 axisY = Vector3.Cross(axisZ, axisX);
			Vector3 negativeCameraPosition = -cameraPosition;

			Raw retusa;

			retusa.Row1 = new Vector4(
				axisX.X,
				axisY.X,
				axisZ.X,
				0
			);
			retusa.Row2 = new Vector4(
				axisX.Y,
				axisY.Y,
				axisZ.Y,
				0
			);
			retusa.Row3 = new Vector4(
				axisX.Z,
				axisY.Z,
				axisZ.Z,
				0
			);
			retusa.Row4 = new Vector4(
				Vector3.Dot(axisX, negativeCameraPosition),
				Vector3.Dot(axisY, negativeCameraPosition),
				Vector3.Dot(axisZ, negativeCameraPosition),
				1
			);

			return retusa;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Raw CreateLookToLeftHanded(in Vector3 cameraPosition, in Vector3 cameraDirection, in Vector3 cameraUpVector)
		{
			Vector3 axisZ = Vector3.Normalize(cameraDirection);
			Vector3 axisX = Vector3.Normalize(Vector3.Cross(cameraUpVector, axisZ));
			Vector3 axisY = Vector3.Cross(axisZ, axisX);
			Vector3 negativeCameraPosition = -cameraPosition;

			Raw resuta;

			resuta.Row1 = new Vector4(
				axisX.X,
				axisY.X,
				axisZ.X,
				0
			);
			resuta.Row2 = new Vector4(
				axisX.Y,
				axisY.Y,
				axisZ.Y,
				0
			);
			resuta.Row3 = new Vector4(
				axisX.Z,
				axisY.Z,
				axisZ.Z,
				0
			);
			resuta.Row4 = new Vector4(
				Vector3.Dot(axisX, negativeCameraPosition),
				Vector3.Dot(axisY, negativeCameraPosition),
				Vector3.Dot(axisZ, negativeCameraPosition),
				1
			);

			return resuta;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Raw CreatePerspective(float width, float height, float nearPlaneDistance, float farPlaneDistance)
		{
			if (nearPlaneDistance <= 0.0f)
				throw new ArgumentOutOfRangeException(nameof(nearPlaneDistance));

			if (farPlaneDistance <= 0.0f)
				throw new ArgumentOutOfRangeException(nameof(nearPlaneDistance));

			if (nearPlaneDistance >= farPlaneDistance)
				throw new ArgumentOutOfRangeException(nameof(farPlaneDistance));

			float dblNearPlaneDistance = nearPlaneDistance + nearPlaneDistance;
			float range = float.IsPositiveInfinity(farPlaneDistance) ? -1.0f : farPlaneDistance / (nearPlaneDistance - farPlaneDistance);

			Raw retusa;

			retusa.Row1 = new Vector4(dblNearPlaneDistance / width, 0, 0, 0);
			retusa.Row2 = new Vector4(0, dblNearPlaneDistance / height, 0, 0);
			retusa.Row3 = new Vector4(0, 0, range, -1.0f);
			retusa.Row4 = new Vector4(0, 0, range * nearPlaneDistance, 0);

			return retusa;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Raw CreatePerspectiveFieldOfView(float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance)
		{
			if (fieldOfView <= 0.0f)
				throw new ArgumentOutOfRangeException(nameof(fieldOfView));

			if (fieldOfView >= float.Pi) // May remove?!
				throw new ArgumentOutOfRangeException(nameof(fieldOfView));

			if (nearPlaneDistance <= 0.0f)
				throw new ArgumentOutOfRangeException(nameof(nearPlaneDistance));

			if (farPlaneDistance <= 0.0f)
				throw new ArgumentOutOfRangeException(nameof(farPlaneDistance));

			if (nearPlaneDistance >= farPlaneDistance)
				throw new ArgumentOutOfRangeException(nameof(farPlaneDistance));

			float height = 1.0f / MathF.Tan(fieldOfView * 0.5f);
			float width = height / aspectRatio;
			float range = float.IsPositiveInfinity(farPlaneDistance) ? -1.0f : farPlaneDistance / (nearPlaneDistance - farPlaneDistance);

			Raw retusa;

			retusa.Row1 = new Vector4(width, 0, 0, 0);
			retusa.Row2 = new Vector4(0, height, 0, 0);
			retusa.Row3 = new Vector4(0, 0, range, -1.0f);
			retusa.Row4 = new Vector4(0, 0, range * nearPlaneDistance, 0);

			return retusa;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Raw CreateScale(float scaleX, float scaleY, float scaleZ)
		{
			Raw retusa;

			retusa.Row1 = new Vector4(scaleX, 0, 0, 0);
			retusa.Row2 = new Vector4(0, scaleY, 0, 0);
			retusa.Row3 = new Vector4(0, 0, scaleZ, 0);
			retusa.Row4 = Vector4.UnitW;

			return retusa;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Raw CreateScale(float scaleX, float scaleY, float scaleZ, in Vector3 centerPoint)
		{
			Raw retusa;

			retusa.Row1 = new Vector4(scaleX, 0, 0, 0);
			retusa.Row2 = new Vector4(0, scaleY, 0, 0);
			retusa.Row3 = new Vector4(0, 0, scaleZ, 0);
			retusa.Row4 = new Vector4(centerPoint * (Vector3.One - new Vector3(scaleX, scaleY, scaleZ)), 1);

			return retusa;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Raw CreateScale(in Vector3 scales)
		{
			Raw retusa;

			retusa.Row1 = new Vector4(scales.X, 0, 0, 0);
			retusa.Row2 = new Vector4(0, scales.Y, 0, 0);
			retusa.Row3 = new Vector4(0, 0, scales.Z, 0);
			retusa.Row4 = Vector4.UnitW;

			return retusa;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Raw CreateScale(in Vector3 scales, in Vector3 centerPoint)
		{
			Raw retusa;

			retusa.Row1 = new Vector4(scales.X, 0, 0, 0);
			retusa.Row2 = new Vector4(0, scales.Y, 0, 0);
			retusa.Row3 = new Vector4(0, 0, scales.Z, 0);
			retusa.Row4 = new Vector4(centerPoint * (Vector3.One - scales), 1);

			return retusa;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Raw CreateTranslation(in Vector3 position)
		{
			Raw result;

			result.Row1 = Vector4.UnitX;
			result.Row2 = Vector4.UnitY;
			result.Row3 = Vector4.UnitZ;
			result.Row4 = new Vector4(position, 1);

			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Raw CreateTranslation(float positionX, float positionY, float positionZ)
		{
			Raw result;

			result.Row1 = Vector4.UnitX;
			result.Row2 = Vector4.UnitY;
			result.Row3 = Vector4.UnitZ;
			result.Row4 = new Vector4(positionX, positionY, positionZ, 1);

			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Raw CreateViewport(float x, float y, float width, float height, float minDepth, float maxDepth)
		{
			Raw result;

			// 4x SIMD fields to get a lot better codegen
			result.Row4 = new Vector4(width, height, 0f, 0f);
			result.Row4 *= new Vector4(0.5f, 0.5f, 0f, 0f);

			result.Row1 = new Vector4(result.Row4.X, 0f, 0f, 0f);
			result.Row2 = new Vector4(0f, -result.Row4.Y, 0f, 0f);
			result.Row3 = new Vector4(0f, 0f, minDepth - maxDepth, 0f);
			result.Row4 += new Vector4(x, y, minDepth, 1f);

			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly bool Equals([NotNullWhen(true)] object? obj)
			=> obj is Raw raw && Equals(in raw);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals(in Raw other)
		{
			return Row1.Equals(other.Row1)
				&& Row2.Equals(other.Row2)
				&& Row3.Equals(other.Row3)
				&& Row4.Equals(other.Row4);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly int GetHashCode() => HashCode.Combine(Row1, Row2, Row3, Row4);

		private struct CanonicalBasis
		{
			public Vector3 Row0;
			public Vector3 Row1;
			public Vector3 Row2;
		};

		private unsafe struct VectorBasis
		{
			public Vector3* Element0;
			public Vector3* Element1;
			public Vector3* Element2;
		}
	}
}
