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

		VkDescriptorPoolSize poolSize = new()
		{
			type = VkDescriptorType.UniformBufferDynamic,
			descriptorCount = 64
		};
		VkDescriptorPoolCreateInfo poolInfo = new()
		{
			maxSets = 64,
			poolSizeCount = 1,
			pPoolSizes = &poolSize
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
