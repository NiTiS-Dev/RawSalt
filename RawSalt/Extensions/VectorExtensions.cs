namespace RawSalt.Extensions;

public static unsafe class VectorExtensions
{
	public static SysVec2 ToSystem(this vec2 vector)
		=> *(SysVec2*)&vector;
	public static SysVec3 ToSystem(this vec3 vector)
		=> *(SysVec3*)&vector;
	public static SysVec4 ToSystem(this vec4 vector)
		=> *(SysVec4*)&vector;
}