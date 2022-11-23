using NiTiS.IO;
using Silk.NET.Core.Contexts;
using Silk.NET.OpenGL;
using System.Collections.Generic;

namespace RawSalt.App;

public abstract class Application
{
	private protected readonly List<IApplicationListener> applicationListeners;
	public Application()
	{
		applicationListeners = new(8);
	}
	public virtual void Subscribe(IApplicationListener listener)
	{
		applicationListeners.Add(listener);
	}
	public virtual void Unsubscribe(IApplicationListener listener)
	{
		applicationListeners.Remove(listener);
	}
	/// <summary>
	/// Initialize all application system (kinda sound system or input handling)<br/>
	/// Run <see cref="Initialize"/> before <see cref="Run"/>!
	/// </summary>
	public virtual void Initialize()
	{
	}
	/// <summary>
	/// Create application window<br/>
	/// Startup <see cref="Draw(double)"/> <![CDATA[&]]> <see cref="Update(double)"/> cycle
	/// </summary>
	public virtual void Run()
	{

	}
	/// <summary>
	/// Called when window is resized
	/// </summary>
	/// <param name="size">Window new size</param>
	public virtual void Resize(vec2i size)
	{

	}
	/// <summary>
	/// Ends up all processes<br/>
	/// Automaticy called by <see cref="Close"/>
	/// </summary>
	public virtual void Closing()
	{

	}
	/// <summary>
	/// Close application
	/// </summary>
	public virtual void Close()
	{

	}
	/// <summary>
	/// Invoked when user drop file(s) into application window
	/// </summary>
	/// <param name="paths">Array of dropped files (or directories)</param>
	public virtual void FileDropped(IOPath[] paths)
	{

	}
	/// <summary>
	/// Render image for window
	/// </summary>
	/// <param name="delta">FPS = 1/delta<br/> Time during previous drawing</param>
	public virtual void Draw(double delta)
	{

	}
	/// <summary>
	/// TODO: summary
	/// </summary>
	/// <param name="delta">UPF = 1/delta<br/>Time during previous update</param>
	public virtual void Update(double delta)
	{

	}
}