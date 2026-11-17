using System.Numerics;
using RawSalt.Graphics.Rendering;
using RawSalt.Threading;

namespace RawSalt.Scenes;

/// <summary>
/// A node in the scene tree. Collection happens on the main thread, graphics lifetime on the render thread.
/// </summary>
public abstract class SceneObject
{
	private readonly List<SceneObject> _children = [];

	internal volatile bool _graphicsLoaded;

	public SceneObject? Parent { get; private set; }
	public IReadOnlyList<SceneObject> Children => _children;

	/// <summary>
	/// Draw order within a layer. Lower values are drawn first.
	/// </summary>
	public uint Layer { get; set; }

	public void Add(SceneObject child)
	{
		ArgumentNullException.ThrowIfNull(child);
		if (child.Parent != null)
		{
			throw new InvalidOperationException("The object already has a parent.");
		}

		_children.Add(child);
		child.Parent = this;
	}

	public void Remove(SceneObject child)
	{
		if (child.Parent != this)
		{
			return;
		}

		_children.Remove(child);
		child.Parent = null;
	}

	/// <summary>
	/// Local-to-parent transformation.
	/// </summary>
	public virtual Matrix4x4 World => Matrix4x4.Identity;

	[ThreadSafety(AllowedThread.Main)]
	public virtual void Collect(List<RenderItem> items, in Matrix4x4 parentWorld)
	{
		foreach (SceneObject child in _children)
		{
			child.Collect(items, in parentWorld);
		}
	}

	[ThreadSafety(AllowedThread.Render)]
	protected internal virtual void LoadGraphics()
	{
		_graphicsLoaded = true;
		foreach (SceneObject child in _children)
		{
			child.LoadGraphics();
		}
	}

	[ThreadSafety(AllowedThread.Render)]
	protected internal virtual void UnloadGraphics()
	{
		foreach (SceneObject child in _children)
		{
			child.UnloadGraphics();
		}
		_graphicsLoaded = false;
	}
}