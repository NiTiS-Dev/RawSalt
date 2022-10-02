using RawSalt.Core;
using RawSalt.Core.Scenes;
using RawSalt.Core.Scenes.Objects;
using System.Collections.Generic;

namespace RawSalt;

/// <summary>
/// Scene :)
/// </summary>
public class Scene : INamedObject
{
	public SceneUnloadType UnloadType { get; init; }
	private List<VirtualObject> actors = new(4);
	public string Name { get; set; } = "scene.scn";

	public void AppendObject(VirtualObject @object)
	{
		actors.Add(@object);
	}
	public virtual void Load(Application app)
	{
		System.Console.WriteLine($"{Name}.Load()");
	}
	public virtual void Unload()
	{
		System.Console.WriteLine($"{Name}.Unload()");
	}
}