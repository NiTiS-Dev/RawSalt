using System.Numerics;
using RawSalt.Graphics;
using RawSalt.Graphics.Rendering;
using RawSalt.Threading;

namespace RawSalt.Scenes;

public sealed class WorldObject : SceneObject
{
	private volatile Mesh? _mesh;
	private volatile Material? _material;

	/// <summary>
	/// Local-space object position.
	/// </summary>
	public Vector3 Position { get; set; }

	/// <summary>
	/// Local-space object rotation.
	/// </summary>
	public Quaternion Rotation { get; set; }

	/// <summary>
	/// Local-space object scale.
	/// </summary>
	public Vector3 Scale { get; set; } = Vector3.One;

	/// <summary>
	/// Graphics used to draw this object. Assign on the render thread in LoadGraphics.
	/// </summary>
	public Mesh? Mesh
	{
		get => _mesh;
		set => _mesh = value;
	}

	/// <summary>
	/// Graphics used to draw this object. Assign on the render thread in LoadGraphics.
	/// </summary>
	public Material? Material
	{
		get => _material;
		set => _material = value;
	}

	/// <summary>
	/// Local-to-parent transformation.
	/// </summary>
	public override Matrix4x4 World => Matrix4x4.CreateScale(Scale) * Matrix4x4.CreateFromQuaternion(Rotation) * Matrix4x4.CreateTranslation(Position);

	[ThreadSafety(AllowedThread.Main)]
	public override void Collect(List<RenderItem> items, in Matrix4x4 parentWorld)
	{
		Matrix4x4 world = World * parentWorld;
		if (_graphicsLoaded && _mesh is { } mesh && _material is { } material)
		{
			items.Add(new RenderItem
			{
				World = world,
				Mesh = mesh,
				Material = material,
				Layer = Layer,
				Order = (uint)items.Count
			});
		}

		base.Collect(items, in world);
	}
}