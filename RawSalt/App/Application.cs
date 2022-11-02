using NiTiS.IO;
using Silk.NET.Core.Contexts;
using Silk.NET.OpenGL;
using System.Collections.Generic;

namespace RawSalt.App;

public abstract class Application
{
	private readonly List<IApplicationListener> applicationListeners;
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
	public virtual void Initialize()
	{
	}
	public virtual void Resize(vec2i size)
	{

	}
	public virtual void Closing()
	{

	}
	public virtual void FileDropped(IOPath[] paths)
	{

	}
	public virtual void Draw(double delta)
	{

	}
	public virtual void Update(double delta)
	{

	}
}