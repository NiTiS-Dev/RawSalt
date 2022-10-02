using Silk.NET.Maths;
using System;
using System.Numerics;

namespace RawSalt.Core.Scenes.Objects;

public class Camera
{
	public vec3 Position { get; set; }
	public vec3 Front { get; set; }
	public vec3 Up { get; set; }
	public float AspectRatio { get; set; }
	public float Yaw { get; set; } = -90f;
	public float Pitch { get; set; }
	private float Zoom = 45f;
	public Camera(vec3 position, vec3 front, vec3 up, float aspectRatio)
	{
		Position = position;
		AspectRatio = aspectRatio;
		Front = front;
		Up = up;
	}
	public void ModifyDirection(float xOffset, float yOffset)
	{
		Yaw += xOffset;
		Pitch -= yOffset;

		//We don't want to be able to look behind us by going over our head or under our feet so make sure it stays within these bounds
		Pitch = Math.Clamp(Pitch, -89f, 89f);

		vec3 cameraDirection = vec3.One;
		cameraDirection.X = MathF.Cos(SaltMath.DegreesToRadians(Yaw)) * MathF.Cos(SaltMath.DegreesToRadians(Pitch));
		cameraDirection.Y = MathF.Sin(SaltMath.DegreesToRadians(Pitch));
		cameraDirection.Z = MathF.Sin(SaltMath.DegreesToRadians(Yaw)) * MathF.Cos(SaltMath.DegreesToRadians(Pitch));

		Front = Vector3D.Normalize(cameraDirection);
	}
	public void IncresseZoom(float zoom)
	{
		Zoom = Math.Clamp(Zoom - zoom, 1f, 45f);
	}
	public mat4 GetView()
		=> Matrix4X4.CreateLookAt(Position, Position + Front, Up);
	public mat4 GetProjection()
		=> Matrix4X4.CreatePerspectiveFieldOfView(SaltMath.DegreesToRadians(Zoom), AspectRatio, 0.001f, 1100.0f);
}
public enum CameraType
{
	Perspective = 0,
	Orthographic = 1,
}
