using RawSalt.Threading;

namespace RawSalt.Graphics.Rendering;

[ThreadSafety(AllowedThread.Main | AllowedThread.Render)]
internal sealed class RenderSnapshot
{
	public Vec2i Size { get; set; }
	public CameraData Camera { get; set; }

	public void Reset()
	{

	}
}