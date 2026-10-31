using System.Diagnostics.CodeAnalysis;
using RawSalt.Graphics.Rendering;

namespace RawSalt.Graphics.Vulkan.Rendering;

public sealed class VulkanRenderer : Renderer
{

	private readonly VulkanDevice _device;
	private readonly VulkanSwapchain _swapchain;
	private readonly VkCommandPool _pool;
	private readonly VulkanRenderFrame[] _frames;
	private VulkanRenderFrame? _active;

	private VkDeviceApi Api => _device.DeviceApi;

	public unsafe VulkanRenderer(VulkanDevice device, VulkanSwapchain swapchain)
	{
		_device = device;
		_swapchain = swapchain;

		VkCommandPoolCreateInfo poolInfo = new()
		{
			flags = VkCommandPoolCreateFlags.ResetCommandBuffer,
			queueFamilyIndex = device.GraphicsQueueFamily,
		};
		Api.vkCreateCommandPool(&poolInfo, out _pool);

		uint count = swapchain.FramesInFlight;
		VkCommandBuffer[] buffers = new VkCommandBuffer[count];
		VkCommandBufferAllocateInfo allocInfo = new()
		{
			commandPool = _pool,
			level = VkCommandBufferLevel.Primary,
			commandBufferCount = count,
		};
		fixed (VkCommandBuffer* p = buffers)
		{
			Api.vkAllocateCommandBuffers(&allocInfo, p);
		}

		_frames = new VulkanRenderFrame[count];
		VkFenceCreateInfo fenceInfo = new() { flags = VkFenceCreateFlags.Signaled };
		for (int i = 0; i < count; i++)
		{
			Api.vkCreateFence(&fenceInfo, out VkFence fence);
			_frames[i] = new VulkanRenderFrame(fence, new VulkanCommandList(Api, buffers[i]));
		}
	}

	public override bool TryBeginFrame([NotNullWhen(true)] out RenderFrame? frame)
	{
		frame = null;
		if (_active != null)
		{
			throw new InvalidOperationException("The previous frame was not ended yet.");
		}

		Api.vkWaitForFences(_frames[_swapchain.FrameIndex].Fence, true, ulong.MaxValue);

		if (!_swapchain.TryAcquire())
		{
			return false;
		}

		VulkanRenderFrame f = _frames[_swapchain.FrameIndex];
		Api.vkResetFences(f.Fence);

		f.Target.Image = _swapchain.CurrentImage;
		f.Target.View = _swapchain.CurrentImageView;
		f.Target.Extent = _swapchain.Extent;
		f.Target.Layout = VkImageLayout.Undefined;
		f.CommandList.Begin();

		_active = f;
		frame = f;
		return true;
	}

	public override unsafe void EndFrame(RenderFrame frame)
	{
		if (_active == null || !ReferenceEquals(frame, _active))
		{
			throw new InvalidOperationException("Frame does not match the active frame.");
		}

		VulkanRenderFrame f = _active;
		_active = null;

		f.CommandList.Transition(f.Target, VkImageLayout.PresentSrcKHR);
		f.CommandList.End();

		// Read these BEFORE Present(): it advances FrameIndex.
		VkSemaphore wait = _swapchain.ImageAvailableSemaphore;
		VkSemaphore signal = _swapchain.RenderFinishedSemaphore;
		VkCommandBuffer cmd = f.CommandList.Buffer;
		VkPipelineStageFlags waitStage = VkPipelineStageFlags.ColorAttachmentOutput;

		VkSubmitInfo submit = new()
		{
			waitSemaphoreCount = 1,
			pWaitSemaphores = &wait,
			pWaitDstStageMask = &waitStage,
			commandBufferCount = 1,
			pCommandBuffers = &cmd,
			signalSemaphoreCount = 1,
			pSignalSemaphores = &signal,
		};

		VkResult result = Api.vkQueueSubmit(_device.GraphicsQueue, 1, &submit, f.Fence);
		if (result != VkResult.Success)
		{
			throw new VkException(result, "Failed to submit frame.");
		}

		_swapchain.Present();
	}

	protected override void Dispose(bool disposing)
	{
		Api.vkDeviceWaitIdle();
		foreach (VulkanRenderFrame f in _frames)
		{
			Api.vkDestroyFence(f.Fence);
		}

		Api.vkDestroyCommandPool(_pool);
	}
}