using RawSalt.Graphics.Rendering;
using RawSalt.Graphics.Vulkan.Rendering;
using RawSalt.Logging;
using RawSalt.Platforms;

namespace RawSalt.Graphics.Vulkan;

public sealed class VulkanDevice : GraphicsDevice
{
	public VulkanAdapter Adapter { get; }
	public VkPhysicalDevice PhysicalDevice => Adapter.PhysicalDevice;
	public VkDevice Device { get; }
	public VkInstanceApi InstanceApi { get; }
	public VkDeviceApi DeviceApi { get; }
	public uint GraphicsQueueFamily { get; }
	public VkQueue GraphicsQueue { get; }
	public uint PresentQueueFamily { get; }
	public VkQueue PresentQueue { get; }

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
		DeviceApi.vkDestroyDevice();
	}
}
