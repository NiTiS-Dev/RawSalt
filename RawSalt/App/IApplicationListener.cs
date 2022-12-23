using NiTiS.IO;

namespace RawSalt.App;

public interface IApplicationListener
{
	/// <summary>
	/// Invokes when listener is subscribes
	/// </summary>
	void OnSubscribe();
	/// <summary>
	/// Invokes when listener is unsubscribes
	/// </summary>
	void OnUnsubscribe();

	/// <summary>
	/// Invokes before application exit
	/// </summary>
	/// <param name="code">Exit code<br/>0 means user-managed exit</param>
	void OnApplicationExit(int code);
	/// <summary>
	/// Invokes when window is resized
	/// </summary>
	/// <param name="newSize">New application size</param>
	void OnApplicationResize(ivec2 newSize);
	/// <summary>
	/// Render tick
	/// </summary>
	void OnApplicationRenderFrame();
	/// <summary>
	/// Update tick
	/// </summary>
	void OnApplicationUpdate();
}