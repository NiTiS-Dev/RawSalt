namespace RawSalt.Engine;

public struct Transform3D
{
	public vec3 position;
	public vec3 scale;
	public quat rotation;
	public Transform3D()
	{
		position = default;
		scale = vec3.One;
		rotation = default;
	}
	public Transform3D(vec3 pos)
	{
		position = pos;
	}
	public Transform3D(vec3 pos, vec3 scl)
	{

		position = pos;
		scale = scl;
	}
	public Transform3D(vec3 pos, vec3 scl, quat rot)
	{
		position = pos;
		scale = scl;
		rotation = rot;
	}
	//
	public mat4 CreateModelMatrix()
		=> mat4.Identity
		* mat4.CreateFromQuaternion(rotation)
		* mat4.CreateScale(scale)
		* mat4.CreateTranslation(position);
}
