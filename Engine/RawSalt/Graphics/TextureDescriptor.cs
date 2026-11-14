using RawSalt.Graphics.Drawing;

namespace RawSalt.Graphics;

/// <summary>
/// Backend of a texture image.
/// </summary>
/// <remarks>
/// <see cref="Size"/> is the image extent in pixels. A cubemap uses <see cref="Size"/> for one face
/// and has six layers; every other dimension has a single layer.
/// </remarks>
public readonly record struct TextureDescriptor(
	TextureDimension Dimension,
	Vec3i Size,
	PixelFormat Format,
	Sampling Sampling = Sampling.Repeat)
{
	public static TextureDescriptor Create(int width, PixelFormat format, Sampling sampling = Sampling.Repeat)
	{
		return new TextureDescriptor(TextureDimension.Texture1D, new Vec3i(width, 1, 1), format, sampling);
	}

	public static TextureDescriptor Create(int width, int height, PixelFormat format, Sampling sampling = Sampling.Repeat)
	{
		return new TextureDescriptor(TextureDimension.Texture2D, new Vec3i(width, height, 1), format, sampling);
	}

	public static TextureDescriptor Create(int width, int height, int depth, PixelFormat format, Sampling sampling = Sampling.Repeat)
	{
		return new TextureDescriptor(TextureDimension.Texture3D, new Vec3i(width, height, depth), format, sampling);
	}

	public static TextureDescriptor CreateCube(int width, int height, PixelFormat format, Sampling sampling = Sampling.Repeat)
	{
		return new TextureDescriptor(TextureDimension.Cube, new Vec3i(width, height, 1), format, sampling);
	}
}
