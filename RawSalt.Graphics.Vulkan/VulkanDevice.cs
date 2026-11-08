using RawSalt.Graphics.Pipelines;
using RawSalt.Graphics.Rendering;
using RawSalt.Graphics.Vulkan.Rendering;
using RawSalt.Logging;
using RawSalt.Platforms;

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
	public VkDescriptorPool DescriptorPool { get; }
	internal VulkanUploadHeap UploadHeap { get; }

	internal unsafe VulkanDevice(VulkanAdapter adapter, VkDevice device, VkInstanceApi instanceApi, VkDeviceApi deviceApi,
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

		VkDescriptorPoolSize* poolSizes = stackalloc VkDescriptorPoolSize[2];
		poolSizes[0] = new()
		{
			type = VkDescriptorType.UniformBufferDynamic,
			descriptorCount = 64
		};
		poolSizes[1] = new()
		{
			type = VkDescriptorType.CombinedImageSampler,
			descriptorCount = 64
		};
		VkDescriptorPoolCreateInfo poolInfo = new()
		{
			maxSets = 64,
			poolSizeCount = 2,
			pPoolSizes = poolSizes
		};
		DeviceApi.vkCreateDescriptorPool(&poolInfo, out VkDescriptorPool pool).CheckResult();
		DescriptorPool = pool;
		UploadHeap = new(this, 1024);
	}

	public override Swapchain CreateSwapchain(RenderSurface surface)
	{
		if (surface is not VulkanSurface vulkanSurface)
		{
			throw new ArgumentException("Provided surface is not of Vulkan");
		}

		return new VulkanSwapchain(this, vulkanSurface);
	}

	public override Renderer CreateRenderer(Swapchain swapchain)
	{
		if (swapchain is not VulkanSwapchain vulkanSwapchain)
		{
			throw new ArgumentException("Provided swapchain is not of Vulkan");
		}

		return new VulkanRenderer(this, vulkanSwapchain);
	}

	public override Buffer CreateBuffer(in BufferDescriptor descriptor, ReadOnlySpan<byte> data)
	{
		return new VulkanBuffer(this, descriptor, data);
	}

	public override Texture2D CreateTexture(in Texture2DDescriptor descriptor, ReadOnlySpan<byte> data)
	{
		return new VulkanTexture2D(this, descriptor, data);
	}

	public override ShaderModule CreateShader(ShaderStages stage, ReadOnlySpan<byte> code)
	{
		return new VulkanShaderModule(this, stage, code);
	}

	public override BindGroupLayout CreateBindGroupLayout(params ReadOnlySpan<BindingEntry> bindings)
	{
		return new VulkanBindGroupLayout(this, bindings);
	}

	public override Pipeline CreatePipeline(PipelineDescriptor descriptor)
	{
		return new VulkanPipeline(this, descriptor);
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
		VkResult result = DeviceApi.vkDeviceWaitIdle();
		if (result != VkResult.Success)
		{
			Log.Warning("Failed to wait for Vulkan device idle, reason: {0}", result);
		}
	}

	protected override void Dispose(bool disposing)
	{
		WaitIdle();
		DeviceApi.vkDestroyDescriptorPool(DescriptorPool);
		UploadHeap.Dispose();
		DeviceApi.vkDestroyDevice();
	}
}
