using RawSalt.Graphics.Pipelines;

namespace RawSalt.Graphics.Rendering;

public abstract class CommandList
{
	public abstract void BeginRenderPass(in RenderPass passDescriptor);
	public abstract void EndRenderPass();

	public abstract void SetPipeline(Pipeline pipeline);
	public abstract void SetUniform<T>(uint set, uint bind, in T data) where T : unmanaged;
	public abstract void SetVertexBuffer(uint slot, Buffer buffer, ulong offset = 0);
	public abstract void SetIndexBuffer(Buffer buffer, ulong offset = 0);
	public abstract void Draw(uint vertexCount, uint instanceCount = 1, uint firstVertex = 0, uint firstInstance = 0);

	public abstract void DrawIndexed(uint indexesCount, uint instanceCount = 1, uint firstIndex = 0,
		int vertexOffset = 0, uint firstInstance = 0);
}