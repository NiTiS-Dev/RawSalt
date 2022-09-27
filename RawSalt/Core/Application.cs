using RawSalt.Core.Scenes;
using RawSalt.Graphics.Textures;
using Silk.NET.OpenGL;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RawSalt.Core;

public class Application
{
	public PlatformType Platform { get; init; }
	private readonly List<IApplicationListener> applicationListeners = new(4);
	private protected GL gl;
	private protected Atlas atlas;
	private Scene? scene;
	public GL GL => gl;
	public Atlas Atlas => atlas;
	public Scene? Scene
	{
		get
		{
			return scene;
		}
		set
		{
			UnloadScene(scene?.UnloadType ?? default);

			scene = value;
			scene?.Load(this);
		}
	}
	public Application(PlatformType platform)
	{
		Platform = platform;

		atlas = new();
	}
	private void UnloadScene(SceneUnloadType unloadType)
	{
		if (scene == null)
			return;

		switch (unloadType)
		{
			case SceneUnloadType.WaitUntilUnloadingEnd:
				scene.Unload();
				break;
			case SceneUnloadType.AsyncUnloadingAndSkip:
				Task unload = new(() =>
				{
					Task.Delay(1000).Wait();
					scene.Unload();
				});
				unload.Start();
				break;
		}
	}
	public void Subscribe(IApplicationListener listener)
	{
		applicationListeners.Add(listener);
	}
	public void Unsubscribe(IApplicationListener listener)
	{
		applicationListeners.Remove(listener);
	}
}
