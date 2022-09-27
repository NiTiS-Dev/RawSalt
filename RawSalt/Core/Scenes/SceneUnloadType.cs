namespace RawSalt.Core.Scenes;

public enum SceneUnloadType : byte
{
	/// <summary>
	/// <code>
	/// scene.Unload();
	/// scene = newScene;
	/// </code>
	/// </summary>
	WaitUntilUnloadingEnd = 0,
	/// <summary>
	/// <code>
	/// Task unload = new( ()=> {
	///		scene.Unload();
	/// });
	/// unload.Invoke();
	/// scene = newScene;
	/// </code>
	/// </summary>
	AsyncUnloadingAndSkip = 2,
}
