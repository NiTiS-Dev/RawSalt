namespace RawSalt.Graphics.Rendering;

/// <summary>
/// Polygon culling mode.
/// </summary>
public enum CullMode
{
	/// <summary>
	/// None of edges is culled.
	/// </summary>
	None,

	/// <summary>
	/// Front face is culled.
	/// </summary>
	Front,

	/// <summary>
	/// Back face is culled. Default option.
	/// </summary>
	Back,

	/// <summary>
	/// Both faces are culled.
	/// </summary>
	FrontAndBack
}