using RawSalt.Graphics.Rendering;
using RawSalt.Logging;
using RawSalt.Platforms;

namespace RawSalt.Graphics.Vulkan;

public sealed class VulkanDevice : GraphicsDevice
{
	public VulkanAdapter Adapter { get; }
	public VkPhysicalDevice PhysicalDevice => Adapter.PhysicalDevice;
	public VkDevice Device { get; }
	public VkDeviceApi DeviceApi { get; }
	public uint GraphicsQueueFamilyIndex { get; }
	public VkQueue GraphicsQueue { get; }
	public uint PresentingQueueFamilyIndex { get; }
	public VkQueue PresentingQueue { get; }

	internal VulkanDevice(VulkanAdapter adapter, VkDevice device, VkDeviceApi deviceApi,
		uint graphicsQueueFamilyIndex, VkQueue graphicsQueue,
		uint presentingQueueFamilyIndex, VkQueue presentingQueue)
	{
		Adapter = adapter;
		Device = device;
		DeviceApi = deviceApi;
		GraphicsQueueFamilyIndex = graphicsQueueFamilyIndex;
		GraphicsQueue = graphicsQueue;
		PresentingQueueFamilyIndex = presentingQueueFamilyIndex;
		PresentingQueue = presentingQueue;
	}

	public override Swapchain CreateSwapchain(RenderSurface surface)
	{
		throw new NotImplementedException();
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
