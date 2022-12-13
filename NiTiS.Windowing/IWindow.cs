using NiTiS.Math;

namespace NiTiS.Windowing;

public interface IWindow
{
	string Title { get; set; }

	bool IsVisible { get; set; }
	bool AlwaysOnTop { get; set; }
	bool TransperentFramebuffer { get; set; }
	Vector2D<int> Position { get; set; }
	Vector2D<int> Size { get; set; }
	WindowState State { get; set; }
	WindowBorder Border { get; set; }

	string? WindowClassName { get; }
}

public interface IWindowHost
{
	IWindow CreateWindow();
}
