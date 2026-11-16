using System.Numerics;
using NiTiS.Mathematics;
using RawSalt.Threading;

namespace RawSalt.Graphics.Rendering;

[ThreadSafety(AllowedThread.Main | AllowedThread.Render)]
internal sealed class RenderSnapshot
{
	public Vec2i Size { get; set; }
	public CameraData Camera { get; set; }
	public List<RenderItem> Items { get; } = [];

	public void Reset()
	{
		Items.Clear();
	}
}

public struct RenderItem
{
	public Matrix4x4 World;
	//public Region3<float> WorldBounds;
	//public Mesh Mesh;
	//public Material Material;
	//public uint Layer;
}