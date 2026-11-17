using System.Diagnostics;
using System.Numerics;
using RawSalt.Graphics.Rendering;
using RawSalt.Threading;

namespace RawSalt.Scenes;

public sealed class Scene
{
	/// <summary>
	/// Application this scene lives in.
	/// </summary>
	public Application Application { get; }

	private readonly RootObject _sceneRoot = new();

	/// <summary>
	/// Camera used to fill the frame's camera data during collection.
	/// </summary>
	public Camera? MainCamera { get; set; }

	internal Scene(Application application)
	{
		Debug.Assert(application != null);
		Application = application;
	}

	public void Add(SceneObject obj) => _sceneRoot.Add(obj);
	public void Remove(SceneObject obj) => _sceneRoot.Remove(obj);

	/// <summary>
	/// Fills camera data and records the draw list for one frame.
	/// </summary>
	[ThreadSafety(AllowedThread.Main)]
	public void Collect(ref CameraData camera, Vec2i viewport, List<RenderItem> items)
	{
		items.Clear();
		if (MainCamera is { } mainCamera)
		{
			mainCamera.SetViewport(viewport.X, viewport.Y);
			camera = new CameraData(mainCamera.View, mainCamera.Projection);
		}

		_sceneRoot.Collect(items, Matrix4x4.Identity);
	}

	[ThreadSafety(AllowedThread.Render | AllowedThread.Main)]
	internal void LoadGraphics() => _sceneRoot.LoadGraphics();

	[ThreadSafety(AllowedThread.Render | AllowedThread.Main)]
	internal void UnloadGraphics() => _sceneRoot.UnloadGraphics();

	private sealed class RootObject : SceneObject;
}