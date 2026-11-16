using RawSalt.Graphics.Drawing;
using RawSalt.Threading;

namespace RawSalt.Graphics.Resources;

/// <summary>
/// GPU texture of any dimension. Render thread only; users hold a typed handle instead.
/// </summary>
[ThreadSafety(AllowedThread.Render)]
public abstract class TextureResource : Disposable
{
	public abstract TextureDimension Dimension { get; }

	/// <summary>
	/// Image extent in pixels. Cubemaps use X and Y for one face; Z is the depth of a volume.
	/// </summary>
	public abstract Vec3i Size { get; }

	public abstract PixelFormat Format { get; }

	/// <summary>
	/// Number of array layers. A cubemap has six, one per face.
	/// </summary>
	public int LayerCount => Dimension == TextureDimension.Cube ? 6 : 1;

	/// <summary>
	/// Size of the tightly packed pixel data for the whole texture, in bytes.
	/// </summary>
	public ulong ByteSize => (ulong)Size.X * (ulong)Size.Y * (ulong)Size.Z * (ulong)LayerCount * (ulong)Format.BytesPerPixel;

	/// <summary>
	/// Replaces the whole texture with <paramref name="pixels"/>.
	/// </summary>
	/// <param name="pixels">Tightly packed pixel data matching <see cref="Format"/>.</param>
	public abstract void Update(ReadOnlySpan<byte> pixels);

	/// <summary>
	/// Copies the whole texture into <paramref name="destination"/>.
	/// </summary>
	/// <param name="destination">Tightly packed pixel data matching <see cref="Format"/>.</param>
	public abstract void GetPixels(Span<byte> destination);

	public void Update(PixelMap2D pixels)
	{
		if (Dimension != TextureDimension.Texture2D ||
		    pixels.Width != Size.X || pixels.Height != Size.Y || pixels.Format != Format)
		{
			throw new ArgumentException("Pixel map format and size must match a 2D texture.", nameof(pixels));
		}

		Update(pixels.GetBytes());
	}

	public byte[] GetPixels()
	{
		byte[] pixels = new byte[ByteSize];
		GetPixels(pixels);
		return pixels;
	}
}
