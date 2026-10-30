namespace RawSalt.Graphics.Rendering;

public abstract class CommandList
{
	public abstract void BeginRenderPass(in RenderPassDesc desc);
	public abstract void EndRenderPass();
}