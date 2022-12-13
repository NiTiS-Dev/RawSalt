namespace NiTiS.Windowing;

public interface IWindowHost
{
	IWindow CreateWindow(WindowOptions options);
}
