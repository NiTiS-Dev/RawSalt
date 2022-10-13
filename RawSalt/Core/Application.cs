using RawSalt.Graphics.Textures;
using RawSalt.Resources;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RawSalt.Core;

public class Application
{
	public PlatformType Platform { get; init; }
	private readonly List<IApplicationListener> applicationListeners = new(4);
	private protected GL gl;
	private protected Atlas atlas;
	private protected ResourceManager resourceManager;
	public GL GL => gl;
	public Atlas Atlas => atlas;
	public ResourceManager ResourceManager => resourceManager;
	public Application(PlatformType platform)
	{
		Platform = platform;

		resourceManager = new();
		atlas = new();
	}
	public void Subscribe(IApplicationListener listener)
	{
		applicationListeners.Add(listener);
	}
	public void Unsubscribe(IApplicationListener listener)
	{
		applicationListeners.Remove(listener);
	}

	protected virtual void Launch()
	{
	}
	public virtual void FileDrop(string[] filePaths)
	{
    }
	public virtual void Resize(vec2i newSize)
	{

	}
	public virtual void Closing()
	{
		ResourceManager.Clear();
	}
	public virtual void Initialize()
	{

	}
	public virtual void GraphicUpdate(double delta)
	{

	}
	public virtual void Update(double delta)
	{

	}
}
