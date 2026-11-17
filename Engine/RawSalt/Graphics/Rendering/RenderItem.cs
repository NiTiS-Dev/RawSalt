using System.Numerics;

namespace RawSalt.Graphics.Rendering;

/// <summary>
/// A draw command collected from a scene object and replayed on the render thread.
/// </summary>
public struct RenderItem
{
	public Matrix4x4 World;
	public Mesh? Mesh;
	public Material? Material;
	public uint Layer;
	public uint Order;
}