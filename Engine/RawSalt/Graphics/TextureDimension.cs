namespace RawSalt.Graphics;

/// <summary>
/// Shape of a texture image.
/// </summary>
public enum TextureDimension
{
	/// <summary>
	/// One-dimensional image sampled by a single coordinate.
	/// </summary>
	Texture1D,

	/// <summary>
	/// Two-dimensional image.
	/// </summary>
	Texture2D,

	/// <summary>
	/// Three-dimensional volume image.
	/// </summary>
	Texture3D,

	/// <summary>
	/// Cubemap: six square faces stored as array layers.
	/// </summary>
	Cube,
}
