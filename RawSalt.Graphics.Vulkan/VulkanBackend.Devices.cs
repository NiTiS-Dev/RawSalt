using System.Diagnostics;
using RawSalt.Graphics.Rendering;
using RawSalt.Logging;

namespace RawSalt.Graphics.Vulkan;

public partial class VulkanBackend
{
	public override GraphicsDevice CreateDevice(Adapter? device, RenderSurface renderSurface)
	{
		Debug.Assert(renderSurface != null);
		if (renderSurface is not VulkanSurface vulkanSurface)
		{
			throw new ArgumentException("Provided render surface is not of Vulkan.");
		}

		VulkanAdapter adapter;
		if (device is null)
		{
			IReadOnlyList<VulkanAdapter> adapters = GetAdapters();
			if (adapters.Count == 0)
			{
				// TODO: Add documentation how to fix that error on laptops with switchable graphics?
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

		uint? combined = null;
		uint? graphics = null;
		uint? present = null;

		VkQueueFamilyProperties[] queueFamilies = new VkQueueFamilyProperties[queueFamilyCount];
		instanceApi.vkGetPhysicalDeviceQueueFamilyProperties(physicalDevice, queueFamilies);
		for (uint i = 0; i < queueFamilies.Length; i++)
		{
			bool supportsGraphics = queueFamilies[i].queueFlags.HasFlag(VkQueueFlags.Graphics);

			// Present support must be checked for every family, including graphics one.
			// On most Linux drivers a single family handles both graphics and presentation.
			instanceApi.vkGetPhysicalDeviceSurfaceSupportKHR(physicalDevice, i, vulkanSurface.Surface, out VkBool32 supported);
			bool supportsPresent = supported;

			if (supportsGraphics && supportsPresent)
			{
				combined ??= i;
			}
			else if (supportsGraphics)
			{
				graphics ??= i;
			}
			else if (supportsPresent)
			{
				present ??= i;
			}
		}

		// Prefer a single family that can do both.
		if (combined is uint combinedFamily)
		{
			return CreateDevice(instanceApi, adapter, combinedFamily, combinedFamily);
		}

		if (graphics is uint graphicsFamily && present is uint presentFamily)
		{
			return CreateDevice(instanceApi, adapter, graphicsFamily, presentFamily);
		}

		throw new NotSupportedException($"Adapter \"{adapter.Name}\" does not support required operations.");
	}

	private static unsafe VulkanDevice CreateDevice(VkInstanceApi instanceApi, VulkanAdapter adapter, uint graphicsQueueFamilyIndex, uint presentQueueFamilyIndex)
	{
		float queuePriority = 1.0f;
		bool sameQueueFamily = graphicsQueueFamilyIndex == presentQueueFamilyIndex;
		uint queueCreateInfoCount = sameQueueFamily ? 1U : 2U;
		VkDeviceQueueCreateInfo* queueCreateInfos = stackalloc VkDeviceQueueCreateInfo[(int)queueCreateInfoCount];
		queueCreateInfos[0] = new()
		{
			queueFamilyIndex = graphicsQueueFamilyIndex,
			queueCount = 1,
			pQueuePriorities = &queuePriority
		};

		if (!sameQueueFamily)
		{
			queueCreateInfos[1] = new()
			{
				queueFamilyIndex = presentQueueFamilyIndex,
				queueCount = 1,
				pQueuePriorities = &queuePriority
			};
		}

		VkUtf8String[] extensionNames = [Vk.VK_KHR_SWAPCHAIN_EXTENSION_NAME];
		using VkStringArray extensions = new(extensionNames);

		VkPhysicalDeviceVulkan13Features features13 = new()
		{
			dynamicRendering = true
		};
		VkDeviceCreateInfo createInfo = new()
		{
			pNext = &features13,
			queueCreateInfoCount = queueCreateInfoCount,
			pQueueCreateInfos = queueCreateInfos,
			enabledExtensionCount = extensions.Length,
			ppEnabledExtensionNames = extensions
		};

		VkResult result = instanceApi.vkCreateDevice(adapter.PhysicalDevice, in createInfo, out VkDevice device);
		if (result != VkResult.Success)
		{
			throw new InvalidOperationException($"Failed to create Vulkan device, reason: {result}.");
		}

		VkDeviceApi deviceApi = Vk.GetApi(instanceApi.Instance, device);
		deviceApi.vkGetDeviceQueue(graphicsQueueFamilyIndex, 0, out VkQueue graphicsQueue);
		deviceApi.vkGetDeviceQueue(presentQueueFamilyIndex, 0, out VkQueue presentQueue);
		return new VulkanDevice(adapter, device, instanceApi, deviceApi, graphicsQueueFamilyIndex, graphicsQueue, presentQueueFamilyIndex, presentQueue);
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
