namespace RawSalt.Graphics;

/// <summary>
/// Buffer allowed usage flags.
/// </summary>
[Flags]
public enum BufferUsage
{
	/// <summary>
	/// No usage.
	/// </summary>
	None = 0,

	/// <summary>
	/// Buffer can be used for vertices.
	/// </summary>
	Vertex = 1,

	/// <summary>
	/// Buffer can be used for vertex indices.
	/// </summary>
	Index = 1 << 1,

	/// <summary>
	/// Buffer can be used for uniform data.
	/// </summary>
	Uniform = 1 << 2,
}