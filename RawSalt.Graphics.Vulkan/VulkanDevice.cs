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

	internal VulkanDevice(VulkanAdapter adapter, VkDevice device, VkDeviceApi deviceApi, uint graphicsQueueFamilyIndex, VkQueue graphicsQueue)
	{
		Adapter = adapter;
		Device = device;
		DeviceApi = deviceApi;
		GraphicsQueueFamilyIndex = graphicsQueueFamilyIndex;
		GraphicsQueue = graphicsQueue;
	}

	public override GraphicsSwapchain CreateSwapchain(View view)
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
