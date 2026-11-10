using RawSalt.Platforms;

namespace RawSalt.Graphics.Rendering;

public abstract class RenderSurface
{
	public View View { get; }

	protected RenderSurface(View view)
	{
		View = view;
	}
}