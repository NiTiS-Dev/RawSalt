using Windows.Win32.Foundation;
using NiTiS.Mathematics;

namespace RawSalt.Windowing.Win32;

internal static class Win32MathExtensions
{
	extension(RECT rect)
	{
		public Region2<int> AsRegion()
		{
			Vector2<int> origin = new(rect.left, rect.bottom);
			Vector2<int> size = new(rect.Width, rect.Height);
			return new(origin, size);
		}
	}
}