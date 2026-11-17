using RawSalt.Threading;

namespace RawSalt.Graphics.Rendering;

[ThreadSafety(AllowedThread.Main | AllowedThread.Render)]
internal sealed class RenderSnapshot
{
	public Vec2i Size { get; set; }
	// TODO(high): Allow several render targets, cameras, and post-process effects
	public CameraData Camera { get; set; }
	public List<RenderItem> Items { get; } = [];

	public void Reset()
	{
		Items.Clear();
	}
}