using RawSalt.Graphics.Pipelines;
using RawSalt.Graphics.Rendering;
using RawSalt.Graphics.Resources;
using RawSalt.Graphics.Vulkan.Rendering;
using RawSalt.Logging;

namespace RawSalt.Graphics.Vulkan;

public sealed class VulkanDevice : GraphicsDevice
{
	public override VulkanAdapter Adapter { get; }
	public VkPhysicalDevice PhysicalDevice => Adapter.PhysicalDevice;
	public VkPhysicalDeviceMemoryProperties MemoryProperties { get; }
	public VkDevice Device { get; }
	public VkInstanceApi InstanceApi { get; }
	public VkDeviceApi DeviceApi { get; }
	public uint GraphicsQueueFamily { get; }
	public VkQueue GraphicsQueue { get; }
	public uint PresentQueueFamily { get; }
	public VkQueue PresentQueue { get; }
	public VkFormat DepthFormat { get; }
	internal VulkanDescriptorHeap DescriptorHeap { get; }
	internal VulkanUploadHeap UploadHeap { get; }

	internal VulkanDevice(VulkanAdapter adapter, VkDevice device, VkInstanceApi instanceApi, VkDeviceApi deviceApi,
		uint graphicsQueueFamily, VkQueue graphicsQueue,
		uint presentQueueFamily, VkQueue presentQueue)
	{
		Adapter = adapter;
		Device = device;
		InstanceApi = instanceApi;
		DeviceApi = deviceApi;
		GraphicsQueueFamily = graphicsQueueFamily;
		GraphicsQueue = graphicsQueue;
		PresentQueueFamily = presentQueueFamily;
		PresentQueue = presentQueue;

		InstanceApi.vkGetPhysicalDeviceMemoryProperties(PhysicalDevice, out VkPhysicalDeviceMemoryProperties properties);
		MemoryProperties = properties;
		DepthFormat = PickDepthFormat();
		DescriptorHeap = new(this);
		UploadHeap = new(this, 1024);
	}

	public override Swapchain CreateSwapchain(RenderSurface surface)
	{
		AssertRenderThread();
		if (surface is not VulkanSurface vulkanSurface)
		{
			throw new ArgumentException("Provided surface is not of Vulkan");
		}

		return new VulkanSwapchain(this, vulkanSurface);
	}

	public override Renderer CreateRenderer(Swapchain swapchain)
	{
		AssertRenderThread();
		if (swapchain is not VulkanSwapchain vulkanSwapchain)
		{
			throw new ArgumentException("Provided swapchain is not of Vulkan");
		}

		return new VulkanRenderer(this, vulkanSwapchain);
	}

	protected override BufferResource CreateBufferResource(in BufferDescriptor descriptor, ReadOnlySpan<byte> data)
	{
		return new VulkanBufferResource(this, descriptor, data);
	}

	protected override TextureResource CreateTextureResource(in TextureDescriptor descriptor, ReadOnlySpan<byte> data)
	{
		return new VulkanTextureResource(this, descriptor, data);
	}

	protected override PipelineResource CreatePipelineResource(PipelineDescriptor descriptor)
	{
		return new VulkanPipeline(this, descriptor);
	}

	private VkFormat PickDepthFormat()
	{
		ReadOnlySpan<VkFormat> candidates = [VkFormat.D32Sfloat, VkFormat.D16Unorm];
		foreach (VkFormat format in candidates)
		{
			InstanceApi.vkGetPhysicalDeviceFormatProperties(PhysicalDevice, format, out VkFormatProperties properties);
			if ((properties.optimalTilingFeatures & VkFormatFeatureFlags.DepthStencilAttachment) != 0)
			{
				return format;
			}
		}

		throw new NotSupportedException("Adapter does not support a depth attachment format.");
	}

	internal uint FindMemoryType(uint typeBits, VkMemoryPropertyFlags required)
	{
		for (uint i = 0; i < MemoryProperties.memoryTypeCount; i++)
		{
			bool allowed = (typeBits & (1u << (int)i)) != 0;
			if (allowed && (MemoryProperties.memoryTypes[(int)i].propertyFlags & required) == required)
			{
				return i;
			}
		}

		throw new InvalidOperationException("No suitable memory type.");
	}

	// shortcut: immediate submits wait for the queue, batch them when transfers become hot.
	internal unsafe void RunOnce(Action<VkCommandBuffer> record)
	{
		AssertRenderThread();
		VkDeviceApi api = DeviceApi;
		VkCommandPoolCreateInfo poolInfo = new()
		{
			queueFamilyIndex = GraphicsQueueFamily,
		};
		api.vkCreateCommandPool(&poolInfo, out VkCommandPool pool).CheckResult();

		VkCommandBufferAllocateInfo allocInfo = new()
		{
			commandPool = pool,
			level = VkCommandBufferLevel.Primary,
			commandBufferCount = 1,
		};
		VkCommandBuffer buffer;
		api.vkAllocateCommandBuffers(&allocInfo, &buffer).CheckResult();

		VkCommandBufferBeginInfo beginInfo = new()
		{
			flags = VkCommandBufferUsageFlags.OneTimeSubmit,
		};
		api.vkBeginCommandBuffer(buffer, &beginInfo).CheckResult();
		record(buffer);
		api.vkEndCommandBuffer(buffer).CheckResult();

		VkSubmitInfo submitInfo = new()
		{
			commandBufferCount = 1,
			pCommandBuffers = &buffer,
		};
		api.vkQueueSubmit(GraphicsQueue, 1, &submitInfo, VkFence.Null).CheckResult();
		api.vkQueueWaitIdle(GraphicsQueue).CheckResult();

		api.vkDestroyCommandPool(pool);
	}

	public override void WaitIdle()
	{
		AssertRenderThread();
		VkResult result = DeviceApi.vkDeviceWaitIdle();
		if (result != VkResult.Success)
		{
			Log.Warning("Failed to wait for Vulkan device idle, reason: {0}", result);
		}
	}

	protected override void Dispose(bool disposing)
	{
		// base waits idle, drains retired resources and disposes the pools; the device must stay alive.
		base.Dispose(disposing);
		DescriptorHeap.Dispose();
		UploadHeap.Dispose();
		DeviceApi.vkDestroyDevice();
	}
}
