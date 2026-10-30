using RawSalt.Logging;

namespace RawSalt.Graphics.Vulkan;

public partial class VulkanBackend
{
	public override GraphicsDevice CreateDevice(Adapter? device)
	{
		VulkanAdapter adapter;
		if (device is null)
		{
			IReadOnlyList<VulkanAdapter> adapters = GetAdapters();
			if (adapters.Count == 0)
			{
				throw new InvalidOperationException("No Vulkan adapter is available.");
			}

			adapter = SelectAdapter(adapters);
		}
		else if (device is VulkanAdapter vulkanAdapter)
		{
			adapter = vulkanAdapter;
		}
		else
		{
			// TODO: Add documentation how to fix that error on laptops with switchable graphics?
			throw new ArgumentException("Provided adapter device is not of Vulkan.");
		}

		VkInstanceApi instanceApi = InstanceApi;
		VkPhysicalDevice physicalDevice = adapter.PhysicalDevice;

		VkUtf8String swapchainExtension = Vk.VK_KHR_SWAPCHAIN_EXTENSION_NAME;
		if (!IsDeviceExtensionSupported(instanceApi, physicalDevice, swapchainExtension))
		{
			throw new NotSupportedException($"Adapter \"{adapter.Name}\" does not support {swapchainExtension}.");
		}

		instanceApi.vkGetPhysicalDeviceQueueFamilyProperties(physicalDevice, out uint queueFamilyCount);
		if (queueFamilyCount == 0)
		{
			throw new NotSupportedException($"Adapter \"{adapter.Name}\" does not expose any queue families.");
		}

		VkQueueFamilyProperties[] queueFamilies = new VkQueueFamilyProperties[queueFamilyCount];
		instanceApi.vkGetPhysicalDeviceQueueFamilyProperties(physicalDevice, queueFamilies);

		for (uint i = 0; i < queueFamilies.Length; i++)
		{
			if ((queueFamilies[i].queueFlags & VkQueueFlags.Graphics) != 0)
			{
				Log.Info("Using Vulkan adapter: {0}", adapter.Name);
				return CreateDevice(instanceApi, adapter, i);
			}
		}

		throw new NotSupportedException($"Adapter \"{adapter.Name}\" does not support graphics operations.");
	}

	private static unsafe VulkanDevice CreateDevice(VkInstanceApi instanceApi, VulkanAdapter adapter, uint queueFamilyIndex)
	{
		float queuePriority = 1.0f;
		VkDeviceQueueCreateInfo queueCreateInfo = new()
		{
			queueFamilyIndex = queueFamilyIndex,
			queueCount = 1,
			pQueuePriorities = &queuePriority
		};

		VkUtf8String[] extensionNames = [Vk.VK_KHR_SWAPCHAIN_EXTENSION_NAME];
		using VkStringArray extensions = new(extensionNames);

		VkDeviceCreateInfo createInfo = new()
		{
			queueCreateInfoCount = 1,
			pQueueCreateInfos = &queueCreateInfo,
			enabledExtensionCount = extensions.Length,
			ppEnabledExtensionNames = extensions
		};

		VkResult result = instanceApi.vkCreateDevice(adapter.PhysicalDevice, in createInfo, out VkDevice device);
		if (result != VkResult.Success)
		{
			throw new InvalidOperationException($"Failed to create Vulkan device, reason: {result}.");
		}

		VkDeviceApi deviceApi = Vk.GetApi(instanceApi.Instance, device);
		deviceApi.vkGetDeviceQueue(queueFamilyIndex, 0, out VkQueue graphicsQueue);

		return new VulkanDevice(adapter, device, deviceApi, queueFamilyIndex, graphicsQueue);
	}

	private static VulkanAdapter SelectAdapter(IReadOnlyList<VulkanAdapter> adapters)
	{
		VulkanAdapter? fallback = null;

		foreach (VulkanAdapter adapter in adapters)
		{
			if (adapter.Properties.deviceType == VkPhysicalDeviceType.DiscreteGpu)
			{
				return adapter;
			}

			fallback ??= adapter;
		}

		return fallback ?? adapters[0];
	}

	private static unsafe bool IsDeviceExtensionSupported(VkInstanceApi instanceApi, VkPhysicalDevice physicalDevice, VkUtf8String extensionName)
	{
		VkResult result = instanceApi.vkEnumerateDeviceExtensionProperties(physicalDevice, out uint extensionCount);
		if (result != VkResult.Success && result != VkResult.Incomplete)
		{
			throw new InvalidOperationException($"Failed to enumerate Vulkan device extensions, reason: {result}.");
		}

		if (extensionCount == 0)
		{
			return false;
		}

		Span<VkExtensionProperties> extensions = stackalloc VkExtensionProperties[(int)extensionCount];
		result = instanceApi.vkEnumerateDeviceExtensionProperties(physicalDevice, extensions);
		if (result != VkResult.Success && result != VkResult.Incomplete)
		{
			throw new InvalidOperationException($"Failed to retrieve Vulkan device extensions, reason: {result}.");
		}

		return ContainsExtension(extensions, extensionName);
	}
}
