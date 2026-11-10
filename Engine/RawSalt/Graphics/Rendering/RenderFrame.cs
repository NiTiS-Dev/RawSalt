namespace RawSalt.Graphics.Rendering;

public abstract class RenderFrame
{
	public abstract RenderTarget BackBuffer { get; }
	public abstract CommandList Commands { get; }
}