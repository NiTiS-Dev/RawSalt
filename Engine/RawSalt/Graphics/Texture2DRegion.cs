using NiTiS.Mathematics;
using RawSalt.Threading;

namespace RawSalt.Graphics;

/// <summary>
/// Main-thread safe rectangle inside a <see cref="Texture2D"/>.
/// </summary>
[ThreadSafety(AllowedThread.Main | AllowedThread.Render)]
public readonly record struct Texture2DRegion(Texture2D Texture, Vec2i Offset, Vec2i Size)
{
	public Region2<float> UV
	{
		get
		{
			Vec2i wholeSize = Texture.Size;

			float baseU = float.MapValue(Offset.X, 0f, wholeSize.X, 0f, 1f);
			float baseV = float.MapValue(Offset.Y, 0f, wholeSize.Y, 1f, 0f);
			float u = float.MapValue(Offset.X + Size.X, 0f, wholeSize.X, 0f, 1f);
			float v = float.MapValue(Offset.Y + Size.Y, 0f, wholeSize.Y, 1f, 0f);

			return new Region2<float>(baseU, baseV, u, v);
		}
	}
}