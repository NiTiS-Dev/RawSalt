using RawSalt.Platforms;
using RawSalt.Threading;

namespace RawSalt.Graphics.Rendering;

[ThreadSafety(AllowedThread.Main | AllowedThread.Render)]
public abstract class RenderSurface
{
	public View View { get; }

	protected RenderSurface(View view)
	{
		View = view;
	}
}