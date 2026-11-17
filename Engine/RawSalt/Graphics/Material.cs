using RawSalt.Graphics.Pipelines;
using RawSalt.Graphics.Rendering;
using RawSalt.Threading;

namespace RawSalt.Graphics;

public abstract class Material
{
	public Pipeline Pipeline { get; }

	protected Material(Pipeline pipeline)
	{
		Pipeline = pipeline;
	}

	[ThreadSafety(AllowedThread.Render)]
	public abstract void Apply(CommandList cmd, in RenderItem item, in CameraData camera);
}