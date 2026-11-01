namespace RawSalt.Graphics;

public enum ShaderCodeInputKind
{
	/// <summary>
	/// Shaders require textual .glsl format to create shader module.
	/// </summary>
	Glsl,

	/// <summary>
	/// Shaders require binary SPIR-V format to create shader module.
	/// </summary>
	Spirv,
}