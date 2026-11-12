using RawSalt.Graphics.Rendering;
using RawSalt.Threading;

namespace RawSalt.Graphics.Drawing;

[ThreadSafety(AllowedThread.Main | AllowedThread.Render)]
internal sealed class RenderSnapshot
{
	public Vec2i Size { get; set; }
	public CameraData Camera { get; set; }

	public void Reset()
	{

	}
}