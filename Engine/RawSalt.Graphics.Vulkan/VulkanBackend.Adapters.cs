using RawSalt.Logging;

namespace RawSalt.Graphics.Vulkan;

public partial class VulkanBackend
{
	private readonly Lock _instanceLock = new();
	private List<VulkanAdapter>? _adapters;

	public override IReadOnlyList<VulkanAdapter> GetAdapters()
	{
		if (_adapters == null)
		{
			var adapters = CreateAdapters();
			Interlocked.CompareExchange(ref _adapters, adapters, null);
		}

		return _adapters;

		List<VulkanAdapter> CreateAdapters()
		{
			VkInstanceApi instanceApi;
			try
			{
				instanceApi = InstanceApi;
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Failed to create Vulkan instance.");
				return [];
			}

			VkResult result = instanceApi.vkEnumeratePhysicalDevices(out uint count);
			if (result != VkResult.Success && result != VkResult.Incomplete)
			{
				Log.Error("Failed to enumerate Vulkan physical devices, reason: {0}", result);
				return [];
			}

			if (count == 0)
			{
				return [];
			}

			VkPhysicalDevice[] devices = new VkPhysicalDevice[count];
			result = instanceApi.vkEnumeratePhysicalDevices(devices);
			if (result != VkResult.Success && result != VkResult.Incomplete)
			{
				Log.Error("Failed to retrieve Vulkan physical devices, reason: {0}", result);
				return [];
			}

			List<VulkanAdapter> adapters = new(devices.Length);
			foreach (VkPhysicalDevice device in devices)
			{
				instanceApi.vkGetPhysicalDeviceProperties(device, out VkPhysicalDeviceProperties properties);
				adapters.Add(new VulkanAdapter(device, properties));
			}

			return adapters;
		}
	}
}