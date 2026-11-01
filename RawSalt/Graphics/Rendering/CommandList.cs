namespace RawSalt.Graphics.Rendering;

public abstract class CommandList
{
	public abstract void BeginRenderPass(in RenderPass passDescriptor);
	public abstract void EndRenderPass();

	public abstract void SetPipeline(Pipeline pipeline);
	public abstract void SetVertexBuffer(uint slot, Buffer buffer, ulong offset = 0);
	public abstract void Draw(uint vertexCount, uint instanceCount = 1, uint firstVertex = 0, uint firstInstance = 0);
}