namespace NiTiS.OpenGL;

public enum ClearBufferMask
{
	DepthBufferBit = 0x100,
	StencilBufferBit = 0x400,
	ColorBufferBit = 0x4000,
	CoverageBufferBitNV = 0x8000,
}