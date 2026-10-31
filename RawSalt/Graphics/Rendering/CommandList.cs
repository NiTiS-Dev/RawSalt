namespace RawSalt.Graphics.Rendering;

public abstract class CommandList
{
	public abstract void BeginRenderPass(in RenderPass desc);
	public abstract void EndRenderPass();
}