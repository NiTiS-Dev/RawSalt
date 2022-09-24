using Silk.NET.Maths;
using NiTiS.IO;

namespace RawSalt.Core;

public interface IApplicationListener
{
	void Init() { }
	void Resize(uint newX, uint newY) { }
	void FileDropped(File file) { }
}
