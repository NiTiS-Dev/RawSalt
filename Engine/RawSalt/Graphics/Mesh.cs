using RawSalt.Graphics.Rendering;
using RawSalt.Threading;

namespace RawSalt.Graphics;

public abstract class Mesh : Disposable
{
	[ThreadSafety(AllowedThread.Render)]
	public abstract void Render(CommandList cmd);
}