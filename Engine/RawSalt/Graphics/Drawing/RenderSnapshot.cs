using RawSalt.Threading;

namespace RawSalt.Graphics.Drawing;

[ThreadSafety(AllowedThread.Main | AllowedThread.Render)]
internal sealed class RenderSnapshot
{
	public Vec2i Size;

	public void Reset()
	{

	}
}