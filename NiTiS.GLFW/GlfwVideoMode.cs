using System.Runtime.InteropServices;

namespace NiTiS.GLFW;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct GlfwVideoMode
{
	public int Width;
	public int Height;
	public int RedBits;
	public int GreenBits;
	public int BlueBits;
	public int RefreshRate;
}
