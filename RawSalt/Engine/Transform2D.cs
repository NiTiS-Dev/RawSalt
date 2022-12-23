namespace RawSalt.Engine;

public struct Transform2D
{
	public vec2 position;
	public vec2 scale;
	public SysQuat rotation;
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
	public Transform2D(vec2 pos, vec2 scl, SysQuat rot)
	{
		position = pos;
		scale = scl;
		rotation = rot;
	}
	//
	public SysMat4x4 CreateModelMatrix()
		=> SysMat4x4.Identity
		* SysMat4x4.CreateFromQuaternion(rotation)
		* SysMat4x4.CreateScale(scale.X, scale.Y, 1f)
		* SysMat4x4.CreateTranslation(position.X, position.Y, 0f);
}
