using Silk.NET.SDL;

namespace RawSalt.Engine;

public struct Transform2D
{
	public vec2 position;
	public vec2 scale;
	public quat rotation;
	public Transform2D()
	{
		position = default;
		scale = vec2.One;
		rotation = default;
	}
	public Transform2D(vec2 pos)
	{
		position = pos;
	}
	public Transform2D(vec2 pos, vec2 scl)
	{

		position = pos;
		scale = scl;
	}
	public Transform2D(vec2 pos, vec2 scl, quat rot)
	{
		position = pos;
		scale = scl;
		rotation = rot;
	}
	//
	public mat4 CreateModelMatrix()
		=> mat4.Identity
		* mat4.CreateFromQuaternion(rotation)
		* mat4.CreateScale(scale.X, scale.Y, 1f)
		* mat4.CreateTranslation(position.X, position.Y, 0f);
}
