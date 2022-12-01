using System.Runtime.InteropServices;

namespace NiTiS.GLFW;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct GlfwGammaRamp
{
	public short* Red;
	public short* Green;
	public short* Blue;
	public uint Size;
}