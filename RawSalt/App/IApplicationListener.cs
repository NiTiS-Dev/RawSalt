using NiTiS.IO;

namespace RawSalt.App;

public interface IApplicationListener
{
	void OnInitialize() { }
	void OnClosing() { }
	void OnResize(vec2i size) { }
	void OnFileDropped(IOPath[] paths) { }
	void OnDraw(double delta) { }
	void OnUpdate(double delta) { }
}