using NiTiS.Mathematics;

namespace RawSalt.Graphics.Vulkan;

public sealed class VulkanSwapchain : Swapchain
{
	public VulkanDevice Device { get; }
	public VkDeviceApi DeviceApi => Device.DeviceApi;
	public VkInstanceApi InstanceApi => Device.InstanceApi;
	public VulkanSurface Surface { get; }

	public VkSwapchainKHR Swapchain { get; private set; } = VkSwapchainKHR.Null;
	public VkExtent2D Extent { get; private set; }
	public override Vector2<int> SurfaceSize => new((int)Extent.width, (int)Extent.height);
	public uint ImageCount => (uint)_images.Length;

	public uint FramesInFlight { get; }

	public uint CurrentImageIndex { get; private set; }
	public VkImage CurrentImage => _images[CurrentImageIndex];
	public VkImageView CurrentImageView => _views[CurrentImageIndex];
	public VkImage CurrentDepthImage => _depthImages[CurrentImageIndex];
	public VkImageView CurrentDepthView => _depthViews[CurrentImageIndex];
	public VkSemaphore ImageAvailableSemaphore => _imageAvailable[FrameIndex];
	public VkSemaphore RenderFinishedSemaphore => _renderFinished[CurrentImageIndex];
	public uint FrameIndex { get; private set; }

	private readonly VkSurfaceFormatKHR _surfaceFormat;
	private readonly VkPresentModeKHR _presentMode;

	public VkFormat Format => _surfaceFormat.format;

	private VkImage[] _images = [];
	private VkImageView[] _views = [];
	private VkImage[] _depthImages = [];
	private VkImageView[] _depthViews = [];
	private VkDeviceMemory[] _depthMemory = [];
	private VkSemaphore[] _imageAvailable = [];
	private VkSemaphore[] _renderFinished = [];

	private Vector2<int> _requestedSize;
	private volatile bool _recreateRequested;

	public override void Resize(Vector2<int> requestedSize)
	{
		_requestedSize = requestedSize; // TODO: Make read/write atomic
		_recreateRequested = true;
	}

	internal unsafe VulkanSwapchain(VulkanDevice device, VulkanSurface surface)
	{
		Device = device;
		Surface = surface;
		FramesInFlight = 2;
		_requestedSize = surface.View.SurfaceSize;

		VkPhysicalDevice physical = device.PhysicalDevice;

		// Presentation support for the queue family we present from
		InstanceApi.vkGetPhysicalDeviceSurfaceSupportKHR(physical, device.PresentQueueFamily, surface.Surface,
			out VkBool32 supported);
		if (!supported)
		{
			throw new InvalidOperationException("The present queue family cannot present to this surface.");
		}

		_surfaceFormat = ChooseSurfaceFormat(physical);
		_presentMode = ChoosePresentMode(physical);

		if (!Recreate())
		{
			_recreateRequested = true;
		}
	}

	public override bool TryAcquire()
	{
		ThrowIfDisposed();

		if (_recreateRequested)
		{
			// Clear first: a Resize() racing with this call re-sets the flag.
			_recreateRequested = false;
			if (!Recreate())
			{
				_recreateRequested = true;
				return false;
			}
		}

		if (Swapchain == VkSwapchainKHR.Null)
		{
			return false;
		}

		VkResult result = DeviceApi.vkAcquireNextImageKHR(Swapchain, ulong.MaxValue, _imageAvailable[FrameIndex],
			VkFence.Null, out uint imageIndex);

		switch (result)
		{
			case VkResult.Success:
				break;

			case VkResult.SuboptimalKHR:
				_recreateRequested = true;
				break;

			case VkResult.ErrorOutOfDateKHR:
				if (!Recreate())
				{
					_recreateRequested = true;
				}
				return false;

			default:
				throw new InvalidOperationException($"Failed to acquire next image, reason: {result}");
		}

		CurrentImageIndex = imageIndex;
		return true;
	}

	public override unsafe void Present()
	{
		ThrowIfDisposed();

		VkSemaphore waitSemaphore = _renderFinished[CurrentImageIndex];
		VkSwapchainKHR swapchain = Swapchain;
		uint imageIndex = CurrentImageIndex;

		VkPresentInfoKHR presentInfo = new()
		{
			waitSemaphoreCount = 1,
			pWaitSemaphores = &waitSemaphore,
			swapchainCount = 1,
			pSwapchains = &swapchain,
			pImageIndices = &imageIndex,
		};

		VkResult result = DeviceApi.vkQueuePresentKHR(Device.PresentQueue, &presentInfo);

		switch (result)
		{
			case VkResult.Success:
				break;

			case VkResult.SuboptimalKHR:
			case VkResult.ErrorOutOfDateKHR:
				_recreateRequested = true;
				break;

			default:
				throw new InvalidOperationException($"Failed to present queue, reason: {result}");
		}

		FrameIndex = (FrameIndex + 1) % FramesInFlight;
	}

	private unsafe bool Recreate()
	{
		InstanceApi.vkGetPhysicalDeviceSurfaceCapabilitiesKHR(Device.PhysicalDevice, Surface.Surface,
			out VkSurfaceCapabilitiesKHR capabilities);

		Vector2<int> requiredSize = _requestedSize;
		VkExtent2D extent = capabilities.currentExtent;
		if (extent.width == uint.MaxValue) // Special code: choose yourself
		{
			extent.width = uint.Clamp((uint)requiredSize.X, capabilities.minImageExtent.width, capabilities.maxImageExtent.width);
			extent.height = uint.Clamp((uint)requiredSize.Y, capabilities.minImageExtent.height, capabilities.maxImageExtent.height);
		}

		if (extent.width == 0 || extent.height == 0)
		{
			// View in minimized state or something.
			return false;
		}

		uint imageCount = capabilities.minImageCount + 1;
		if (capabilities.maxImageCount > 0 && imageCount > capabilities.maxImageCount)
		{
			imageCount = capabilities.maxImageCount;
		}
		// TODO: Adjust FramesInFlight to be smaller?

		VkCompositeAlphaFlagsKHR alpha = PickCompositeAlpha(capabilities.supportedCompositeAlpha);

		VkImageUsageFlags usage = VkImageUsageFlags.ColorAttachment;
		if ((capabilities.supportedUsageFlags & VkImageUsageFlags.TransferDst) != 0)
		{
			usage |= VkImageUsageFlags.TransferDst;
		}

		uint* families = stackalloc uint[2] { Device.GraphicsQueueFamily, Device.PresentQueueFamily };
		bool concurrent = Device.GraphicsQueueFamily != Device.PresentQueueFamily;

		VkSwapchainKHR oldSwapchain = Swapchain;

		VkSwapchainCreateInfoKHR createInfo = new()
		{
			surface = Surface.Surface,
			minImageCount = imageCount,
			imageFormat = _surfaceFormat.format,
			imageColorSpace = _surfaceFormat.colorSpace,
			imageExtent = extent,
			imageArrayLayers = 1,
			imageUsage = usage,
			imageSharingMode = concurrent ? VkSharingMode.Concurrent : VkSharingMode.Exclusive,
			queueFamilyIndexCount = concurrent ? 2U : 0U,
			pQueueFamilyIndices = concurrent ? families : null,
			preTransform = capabilities.currentTransform,
			compositeAlpha = alpha,
			presentMode = _presentMode,
			clipped = true,
			oldSwapchain = oldSwapchain,
		};

		DeviceApi.vkDeviceWaitIdle();

		VkResult result = DeviceApi.vkCreateSwapchainKHR(&createInfo, out VkSwapchainKHR newSwapchain);
		if (result != VkResult.Success)
		{
			throw new InvalidOperationException($"Failed to create swapchain, reason: {result}");
		}

		DestroyDerivedResources();
		if (oldSwapchain != VkSwapchainKHR.Null)
		{
			DeviceApi.vkDestroySwapchainKHR(oldSwapchain);
		}

		Swapchain = newSwapchain;
		Extent = extent;

		DeviceApi.vkGetSwapchainImagesKHR(newSwapchain, out uint actualCount);
		_images = new VkImage[actualCount];
		fixed (VkImage* pImages = _images)
		{
			DeviceApi.vkGetSwapchainImagesKHR(newSwapchain, &actualCount, pImages);
		}

		_views = new VkImageView[actualCount];
		for (int i = 0; i < actualCount; i++)
		{
			VkImageViewCreateInfo viewInfo = new()
			{
				image = _images[i],
				viewType = VkImageViewType.Image2D,
				format = _surfaceFormat.format,
				subresourceRange = new VkImageSubresourceRange(VkImageAspectFlags.Color, 0, 1, 0, 1),
			};
			DeviceApi.vkCreateImageView(&viewInfo, out _views[i]);
		}

		_depthImages = new VkImage[actualCount];
		_depthViews = new VkImageView[actualCount];
		_depthMemory = new VkDeviceMemory[actualCount];
		for (int i = 0; i < actualCount; i++)
		{
			VkImageCreateInfo depthInfo = new()
			{
				imageType = VkImageType.Image2D,
				format = Device.DepthFormat,
				extent = new VkExtent3D(extent.width, extent.height, 1),
				mipLevels = 1,
				arrayLayers = 1,
				samples = VkSampleCountFlags.Count1,
				tiling = VkImageTiling.Optimal,
				usage = VkImageUsageFlags.DepthStencilAttachment,
				sharingMode = VkSharingMode.Exclusive,
				initialLayout = VkImageLayout.Undefined,
			};
			DeviceApi.vkCreateImage(&depthInfo, out _depthImages[i]).CheckResult();

			DeviceApi.vkGetImageMemoryRequirements(_depthImages[i], out VkMemoryRequirements requirements);
			VkMemoryAllocateInfo allocateInfo = new()
			{
				allocationSize = requirements.size,
				memoryTypeIndex = Device.FindMemoryType(requirements.memoryTypeBits, VkMemoryPropertyFlags.DeviceLocal),
			};
			DeviceApi.vkAllocateMemory(&allocateInfo, out _depthMemory[i]).CheckResult();
			DeviceApi.vkBindImageMemory(_depthImages[i], _depthMemory[i], 0).CheckResult();

			VkImageViewCreateInfo depthViewInfo = new()
			{
				image = _depthImages[i],
				viewType = VkImageViewType.Image2D,
				format = Device.DepthFormat,
				subresourceRange = new VkImageSubresourceRange(VkImageAspectFlags.Depth, 0, 1, 0, 1),
			};
			DeviceApi.vkCreateImageView(&depthViewInfo, out _depthViews[i]);
		}

		_renderFinished = new VkSemaphore[actualCount];
		for (int i = 0; i < actualCount; i++)
		{
			DeviceApi.vkCreateSemaphore(out _renderFinished[i]);
		}

		_imageAvailable = new VkSemaphore[FramesInFlight];
		for (int i = 0; i < FramesInFlight; i++)
		{
			DeviceApi.vkCreateSemaphore(out _imageAvailable[i]);
		}

		FrameIndex = 0;
		return true;
	}

	private void DestroyDerivedResources()
	{
		foreach (VkImageView v in _views)
		{
			DeviceApi.vkDestroyImageView(v);
		}

		for (int i = 0; i < _depthImages.Length; i++)
		{
			DeviceApi.vkDestroyImageView(_depthViews[i]);
			DeviceApi.vkDestroyImage(_depthImages[i]);
			DeviceApi.vkFreeMemory(_depthMemory[i]);
		}

		foreach (VkSemaphore s in _imageAvailable)
		{
			DeviceApi.vkDestroySemaphore(s);
		}

		foreach (VkSemaphore s in _renderFinished)
		{
			DeviceApi.vkDestroySemaphore(s);
		}

		_images = [];
		_views = [];
		_depthImages = [];
		_depthViews = [];
		_depthMemory = [];
		_imageAvailable = [];
		_renderFinished = [];
	}

	private static VkCompositeAlphaFlagsKHR PickCompositeAlpha(VkCompositeAlphaFlagsKHR supported)
	{
		ReadOnlySpan<VkCompositeAlphaFlagsKHR> preference =
		[
			VkCompositeAlphaFlagsKHR.Opaque,
			VkCompositeAlphaFlagsKHR.PreMultiplied,
			VkCompositeAlphaFlagsKHR.PostMultiplied,
			VkCompositeAlphaFlagsKHR.Inherit,
		];

		foreach (VkCompositeAlphaFlagsKHR flag in preference)
		{
			if ((supported & flag) != 0)
			{
				return flag;
			}
		}

		return VkCompositeAlphaFlagsKHR.Opaque;
	}

	private unsafe VkSurfaceFormatKHR ChooseSurfaceFormat(VkPhysicalDevice physical)
	{
		InstanceApi.vkGetPhysicalDeviceSurfaceFormatsKHR(physical, Surface.Surface, out uint count);
		VkSurfaceFormatKHR* formats = stackalloc VkSurfaceFormatKHR[(int)count];
		InstanceApi.vkGetPhysicalDeviceSurfaceFormatsKHR(physical, Surface.Surface, &count, formats);

		for (int i = 0; i < count; i++)
		{
			if (formats[i].format == VkFormat.B8G8R8A8Srgb &&
			    formats[i].colorSpace == VkColorSpaceKHR.SrgbNonLinear)
			{
				return formats[i];
			}
		}

		for (int i = 0; i < count; i++)
		{
			if (formats[i].colorSpace == VkColorSpaceKHR.SrgbNonLinear)
			{
				return formats[i];
			}
		}

		return formats[0];
	}

	private unsafe VkPresentModeKHR ChoosePresentMode(VkPhysicalDevice physical)
	{
		const VkPresentModeKHR target = VkPresentModeKHR.Mailbox;

		InstanceApi.vkGetPhysicalDeviceSurfacePresentModesKHR(physical, Surface.Surface, out uint count);
		VkPresentModeKHR* modes = stackalloc VkPresentModeKHR[(int)count];
		InstanceApi.vkGetPhysicalDeviceSurfacePresentModesKHR(physical, Surface.Surface, &count, modes);

		for (int i = 0; i < count; i++)
		{
			if (modes[i] == target)
			{
				return target;
			}
		}

		return VkPresentModeKHR.Mailbox;
	}

	protected override void Dispose(bool disposing)
	{
		if (Swapchain != VkSwapchainKHR.Null)
		{
			DeviceApi.vkDeviceWaitIdle();
			DestroyDerivedResources();
			DeviceApi.vkDestroySwapchainKHR(Swapchain);
			Swapchain = VkSwapchainKHR.Null;
		}
	}
}