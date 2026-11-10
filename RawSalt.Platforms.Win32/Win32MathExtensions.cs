using Windows.Win32.Foundation;
using NiTiS.Mathematics;

namespace RawSalt.Platforms.Win32;

internal static class Win32MathExtensions
{
	extension(RECT rect)
	{
		public Region2<int> AsRegion()
		{
			Vector2<int> origin = new(rect.left, rect.top);
			Vector2<int> size = new(rect.Width, rect.Height);
			return new(origin, size);
		}
	}
}