using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NiTiS.GLFW;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct GlfwImage
{
	public int Width;
	public int Height;
	public byte* Pixels;
}
