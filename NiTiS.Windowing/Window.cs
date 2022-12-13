using NiTiS.Math;

namespace NiTiS.Windowing;

public abstract class Window : IWindow, IWindowHost
{
	protected WindowOptions options;
	protected bool isInitialized;
	public Window(WindowOptions options)
	{
		this.options = options;
	}
	public virtual void Initialize()
	{
		isInitialized = true;
	}
	public virtual void Show()
		=> IsVisible = true;
	public virtual void Hide()
		=> IsVisible = false;
	public abstract string Title { get; set; }
	public abstract bool IsVisible { get; set; }
	public abstract bool AlwaysOnTop { get; set; }
	public abstract bool TransperentFramebuffer { get; set; }
	public abstract Vector2D<int> Position { get; set; }
	public abstract Vector2D<int> Size { get; set; }
	public abstract WindowState State { get; set; }
	public abstract WindowBorder Border { get; set; }
	public abstract string WindowClassName { get; }

	public abstract IWindow CreateWindow();
}
