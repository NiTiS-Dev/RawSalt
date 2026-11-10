using RawSalt.Threading;

namespace RawSalt.Graphics.Rendering;

[ThreadSafety(AllowedThread.Render)]
public abstract class RenderFrame
{
	public abstract RenderTarget BackBuffer { get; }
	public abstract CommandList Commands { get; }
}