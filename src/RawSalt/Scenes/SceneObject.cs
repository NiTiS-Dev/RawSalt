using RawSalt.Mathematics.Geometry;

namespace RawSalt.Scenes;

public abstract class SceneObject
{
	private readonly Scene stage;

	protected SceneObject(Scene stage)
	{
		this.stage = stage;
	}

	public virtual void OnEnterScene(in SceneActionEventArgs args) { }
	public virtual void OnLeaveScene(in SceneActionEventArgs args) { }

	/// <summary>
	/// Removes object from scene.
	/// </summary>
	public void Destroy()
	{
		stage.Remove(this);
	}

	public virtual Vec2 Location { get; set; }
}