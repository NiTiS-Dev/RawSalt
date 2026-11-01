namespace RawSalt.Graphics;

[Flags]
public enum BufferUsage
{
	None = 0,
	Vertex = 1,
	Index = 1 << 1,
	Uniform = 1 << 2,
}