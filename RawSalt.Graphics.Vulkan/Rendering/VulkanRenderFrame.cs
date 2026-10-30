using RawSalt.Graphics.Rendering;

namespace RawSalt.Graphics.Vulkan.Rendering;

internal sealed class VulkanRenderFrame : RenderFrame
{
	public VkFence Fence { get; }
	public VulkanRenderTarget Target { get; } = new();
	public VulkanCommandList CommandList { get; }

	public override RenderTarget BackBuffer => Target;
	public override CommandList Commands => CommandList;

	public VulkanRenderFrame(VkFence fence, VulkanCommandList commandList)
	{
		Fence = fence;
		CommandList = commandList;
	}
}