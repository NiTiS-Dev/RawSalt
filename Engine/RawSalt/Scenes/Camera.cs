using System.Numerics;

namespace RawSalt.Scenes;

public class Camera
{
	private float _nearPlane;
	private float _farPlane;

	public Camera(CameraProjection projection, float nearPlane = 0.1f, float farPlane = 100f)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(nearPlane);
		ArgumentOutOfRangeException.ThrowIfNegative(farPlane);
		ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(nearPlane, farPlane);

		_nearPlane = nearPlane;
		_farPlane = farPlane;

		ProjectionMode = projection;
	}

	public CameraProjection ProjectionMode { get; set; }

	public Vector3 Position
	{
		get => field;
		set => field = value;
	}

	public Quaternion Rotation
	{
		get;
		set => field = Quaternion.Normalize(value);
	} = Quaternion.Identity;

	public Vector3 Forward => Vector3.Transform(-Vector3.UnitZ, Rotation);

	public Vector3 Right => Vector3.Transform(Vector3.UnitX, Rotation);

	public Vector3 Up => Vector3.Transform(Vector3.UnitY, Rotation);

	public float AspectRatio
	{
		get;
		set
		{
			ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);

			field = value;
		}
	} = 16f / 9f;

	public float NearPlane
	{
		get => _nearPlane;
		set
		{
			ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);

			_nearPlane = value;
		}
	}

	public float FarPlane
	{
		get => _farPlane;
		set
		{
			ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, NearPlane);

			_farPlane = value;
		}
	}

	/// <summary>
	/// Vertical size of the orthographic viewing volume.
	/// </summary>
	public float OrthographicHeight
	{
		get;
		set
		{
			ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);

			field = value;
		}
	} = 10f;

	/// <summary>
	/// Local-to-world transformation.
	/// </summary>
	public Matrix4x4 World => Matrix4x4.CreateFromQuaternion(Rotation) * Matrix4x4.CreateTranslation(Position);

	/// <summary>
	/// World-to-camera transformation.
	/// </summary>
	public Matrix4x4 View
	{
		get
		{
			Matrix4x4.Invert(World, out Matrix4x4 view);
			return view;
		}
	}

	/// <summary>
	/// Camera projection transformation.
	/// </summary>
	public Matrix4x4 Projection
	{
		get
		{
			return ProjectionMode switch
			{
				CameraProjection.Perspective => Matrix4x4.CreatePerspectiveFieldOfView(
						FieldOfView,
						AspectRatio,
						NearPlane,
						FarPlane),

				CameraProjection.Orthographic => Matrix4x4.CreateOrthographic(
						width: OrthographicHeight * AspectRatio,
						height: OrthographicHeight,
						NearPlane,
						FarPlane),

				_ => throw new InvalidOperationException("Unknown camera projection mode.")
			};
		}
	}

	/// <summary>
	/// Vertical field of view, in radians.
	/// </summary>
	public float FieldOfView
	{
		get;
		set
		{
			ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);
			ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(value, float.Pi);

			field = value;
		}
	} = MathF.PI / 3f;

	public void SetViewport(int width, int height)
	{
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(width);
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(height);

		AspectRatio = (float)width / height;
	}

	public void LookAt(Vector3 target)
	{
		LookAt(target, Vector3.UnitY);
	}

	public void LookAt(Vector3 target, Vector3 up)
	{
		const float min = 1e-8f;
		Vector3 direction = target - Position;

		if (direction.LengthSquared() < min)
		{
			throw new ArgumentException("Target cannot be equal to camera position.", nameof(target));
		}

		if (Vector3.Cross(direction, up).LengthSquared() < min)
		{
			throw new ArgumentException("Up vector cannot be parallel to the viewing direction.", nameof(up));
		}

		Matrix4x4 view = Matrix4x4.CreateLookAt(Position, target, up);

		Matrix4x4.Invert(view, out Matrix4x4 world);

		Rotation = Quaternion.CreateFromRotationMatrix(world);
	}

	public void Translate(Vector3 translation)
	{
		Position += translation;
	}

	public void Rotate(Quaternion rotation)
	{
		Rotation = Quaternion.Normalize(Rotation * rotation);
	}
}