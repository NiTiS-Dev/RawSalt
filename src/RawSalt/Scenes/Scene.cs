using System.Collections.Generic;

namespace RawSalt.Scenes;

public class Scene : IObjectContainer
{
	private ICamera? activeCamera;
	private List<SceneObject> sceneObjects;

	public void Add(SceneObject sceneObject)
	{
		sceneObjects.Add(sceneObject);
	}
	public bool Remove(SceneObject sceneObject)
	{
		return sceneObjects.Remove(sceneObject);
	}

	public ICamera? ActiveCamera
	{
		set
		{
			activeCamera = value;
		}
		get
		{
			return activeCamera;
		}
	}
}