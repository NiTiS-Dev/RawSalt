using System.Diagnostics;
using RawSalt.Graphics.Rendering;

namespace RawSalt.Graphics.Vulkan.Rendering;

internal sealed class VulkanCommandList : CommandList
{
	private readonly VkDeviceApi _api;
	private VulkanRenderTarget? _pass;

	public VkCommandBuffer Buffer { get; }

	public VulkanCommandList(VkDeviceApi api, VkCommandBuffer buffer)
	{
		_api = api;
		Buffer = buffer;
	}

	public unsafe void Begin()
	{
		_api.vkResetCommandBuffer(Buffer, VkCommandBufferResetFlags.None);
		VkCommandBufferBeginInfo info = new()
		{
			flags = VkCommandBufferUsageFlags.OneTimeSubmit
		};
		_api.vkBeginCommandBuffer(Buffer, &info);
	}

	public void End()
	{
		Debug.Assert(_pass == null, "Render pass left open.");
		_api.vkEndCommandBuffer(Buffer);
	}

	public override unsafe void BeginRenderPass(in RenderPass desc)
	{
		if (_pass != null)
		{
			throw new InvalidOperationException("A render pass is already active.");
		}

		if (desc.Target is not VulkanRenderTarget target)
		{
			throw new ArgumentException("Target belongs to a different backend.", nameof(desc));
		}

		Transition(target, VkImageLayout.ColorAttachmentOptimal);

		VkRenderingAttachmentInfo attachment = new()
		{
			imageView = target.View,
			imageLayout = VkImageLayout.ColorAttachmentOptimal,
			loadOp = desc.Clear is null ? VkAttachmentLoadOp.Load : VkAttachmentLoadOp.Clear,
			storeOp = VkAttachmentStoreOp.Store,
		};

		if (desc.Clear is { } color)
		{
			attachment.clearValue = new VkClearValue
			{
				color = new VkClearColorValue(color.Red, color.Blue, color.Green, color.Alpha)
			};
		}

		VkRenderingInfo info = new()
		{
			renderArea = new VkRect2D(new VkOffset2D(0, 0), target.Extent),
			layerCount = 1,
			colorAttachmentCount = 1,
			pColorAttachments = &attachment,
		};

		_api.vkCmdBeginRendering(Buffer, &info);
		_pass = target;
	}

	public override void EndRenderPass()
	{
		if (_pass == null)
		{
			throw new InvalidOperationException("No active render pass.");
		}

		_api.vkCmdEndRendering(Buffer);
		_pass = null;
	}

	public unsafe void Transition(VulkanRenderTarget target, VkImageLayout newLayout)
	{
		if (target.Layout == newLayout)
		{
			return;
		}

		(VkPipelineStageFlags srcStage, VkAccessFlags srcAccess) = Describe(target.Layout);
		(VkPipelineStageFlags dstStage, VkAccessFlags dstAccess) = Describe(newLayout);

		VkImageMemoryBarrier barrier = new()
		{
			oldLayout = target.Layout,
			newLayout = newLayout,
			srcAccessMask = srcAccess,
			dstAccessMask = dstAccess,
			srcQueueFamilyIndex = Vk.VK_QUEUE_FAMILY_IGNORED,
			dstQueueFamilyIndex = Vk.VK_QUEUE_FAMILY_IGNORED,
			image = target.Image,
			subresourceRange = new VkImageSubresourceRange(VkImageAspectFlags.Color, 0, 1, 0, 1),
		};

		_api.vkCmdPipelineBarrier(Buffer, srcStage, dstStage, 0, 0, null, 0, null, 1, &barrier);
		target.Layout = newLayout;
	}

	private static (VkPipelineStageFlags, VkAccessFlags) Describe(VkImageLayout layout)
	{
		return layout switch
		{
			VkImageLayout.Undefined => (VkPipelineStageFlags.ColorAttachmentOutput, VkAccessFlags.None),
			VkImageLayout.ColorAttachmentOptimal => (VkPipelineStageFlags.ColorAttachmentOutput, VkAccessFlags.ColorAttachmentWrite),
			VkImageLayout.PresentSrcKHR => (VkPipelineStageFlags.BottomOfPipe, VkAccessFlags.None),
			_ => throw new NotSupportedException($"Layout {layout} is not handled yet."),
		};
	}
}