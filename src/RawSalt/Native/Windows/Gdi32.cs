using System.Runtime.InteropServices;

namespace RawSalt.Native.Windows;

/// <summary>
/// Windows library for working with graphics?
/// </summary>
public static partial class Gdi32
{
	public const string LibName = "gdi32.dll";

	[DllImport(LibName)]
	public static extern int SetPixel(HDC hdc, int x, int y, COLORREF color);
}
