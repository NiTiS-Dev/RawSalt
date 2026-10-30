using System.Diagnostics.CodeAnalysis;
using RawSalt.Graphics.Rendering;

namespace RawSalt.Graphics.Vulkan.Rendering;

public sealed class VulkanRenderer : Renderer
{
	public VulkanDevice Device { get; }
	public VulkanSwapchain Swapchain { get; }

	public VulkanRenderer(VulkanDevice device, VulkanSwapchain swapchain)
	{
		Device = device;
		Swapchain = swapchain;
	}

	public override bool TryBeginFrame([NotNullWhen(true)] out RenderFrame? frame)
	{
		frame = null;
		return false;
	}

	public override void EndFrame(RenderFrame frame)
	{
		throw new NotImplementedException();
	}

	protected override void Dispose(bool disposing)
	{
		throw new NotImplementedException();
	}
}