using RawSalt.Graphics.Rendering;

namespace RawSalt.Graphics.Vulkan.Rendering;

internal sealed class VulkanCommandList : CommandList
{
	public override void BeginRenderPass(in RenderPassDesc desc)
	{
		throw new NotImplementedException();
	}

	public override void EndRenderPass()
	{
		throw new NotImplementedException();
	}
}