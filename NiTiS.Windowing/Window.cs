using NiTiS.Math;

namespace NiTiS.Windowing;

public abstract class Window : IWindow, IWindowHost
{
	protected WindowOptions options;
	protected bool isInitialized;
	/// <summary>
	/// Creates window instance
	/// </summary>
	/// <param name="options">Window creation options</param>
	public Window(WindowOptions options)
	{
		this.options = options;
	}
	/// <summary>
	/// Create and initialize window
	/// </summary>
	public virtual void Initialize()
	{
		isInitialized = true;
	}
	/// <summary>
	/// Make window visible
	/// </summary>
	public virtual void Show()
		=> IsVisible = true;
	/// <summary>
	/// Hide window to taskbar
	/// </summary>
	public virtual void Hide()
		=> IsVisible = false;
	/// <summary>
	/// Swap front and back buffers of this window
	/// </summary>
	public abstract void SwapBuffers();
	/// <summary>
	/// Window name
	/// </summary>
	public abstract string Title { get; set; }
	public abstract bool IsVisible { get; set; }
	public abstract bool AlwaysOnTop { get; set; }
	public abstract bool TransperentFramebuffer { get; set; }
	public abstract Vector2D<int> Position { get; set; }
	public abstract Vector2D<int> Size { get; set; }
	public abstract WindowState State { get; set; }
	public abstract WindowBorder Border { get; set; }
	public abstract string WindowClassName { get; }
	public abstract IWindow CreateWindow(WindowOptions options);
	public abstract void Destroy();
}
