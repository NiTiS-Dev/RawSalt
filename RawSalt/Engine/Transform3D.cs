using NiTiS.Math;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace RawSalt.Engine;

public struct Transform3D
{
	public vec3 position;
	public vec3 scale;
	public SysQuat rotation;
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
	public Transform3D(vec3 pos, vec3 scl, SysQuat rot)
	{
		position = pos;
		scale = scl;
		rotation = rot;
	}

	//TODO: Optimize
	public SysMat4x4 CreateModelMatrix()
		=> SysMat4x4.Identity
		* SysMat4x4.CreateFromQuaternion(rotation)
		* SysMat4x4.CreateScale(scale)
		* SysMat4x4.CreateTranslation(position);
}
