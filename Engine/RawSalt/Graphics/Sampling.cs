namespace RawSalt.Graphics;

// TODO(low): Compare different API sampling options
// Maybe extend this enum to packed struct, so user can set repeating by axes, and select color: (opaque black, transparent white)
// Pack:
// 2 bytes for border color;
// 2x3 bytes for sampling per axis
// 1 byte for min filter
// 1 byte for mag filter
/// <summary>
/// Describe how shader program should behave when UV coordinates are out of valid bounds.
/// </summary>
public enum Sampling
{
	/// <summary>
	/// UV coordinates clamp to nearest pixel.
	/// </summary>
	ClampToEdge,

	/// <summary>
	/// Texture is repeated out of bounds.
	/// </summary>
	Repeat,

	/// <summary>
	/// Texture is repeated out of bounds; mirrored.
	/// </summary>
	MirrorRepeat
}