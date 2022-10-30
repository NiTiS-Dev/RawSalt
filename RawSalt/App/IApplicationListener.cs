using NiTiS.IO;

namespace RawSalt.App;

public interface IApplicationListener
{
	void OnInitialize() { }
	void OnResize(vec2i size) { }
	void OnFileDropped(IOPath[] paths) { }
}