using NiTiS.Mathematics;
using RawSalt.Graphics.Drawing;

namespace RawSalt.Graphics.Vulkan;

public sealed class VulkanTexture2D : Texture2D
{
	private readonly VulkanDevice _device;
	private readonly VkDeviceMemory _memory;
	private VkImageLayout _layout;

	public override PixelFormat Format { get; }
	public override Vector2<int> Size { get; }
	public VkImage Image { get; }
	public VkImageView View { get; }

	public unsafe VulkanTexture2D(VulkanDevice device, in Texture2DDescriptor descriptor, ReadOnlySpan<byte> initialData)
	{
		if (descriptor.Size.X <= 0 || descriptor.Size.Y <= 0)
		{
			throw new ArgumentOutOfRangeException(nameof(descriptor), "Texture size must be positive.");
		}

		if ((uint)descriptor.Size.X > device.Adapter.Limits.MaxImageDimension2D ||
		    (uint)descriptor.Size.Y > device.Adapter.Limits.MaxImageDimension2D)
		{
			throw new ArgumentOutOfRangeException(nameof(descriptor), "Texture size exceeds the device limit.");
		}

		_device = device;
		Size = descriptor.Size;
		Format = descriptor.Format;

		VkDeviceApi api = device.DeviceApi;
		VkImageCreateInfo info = new()
		{
			imageType = VkImageType.Image2D,
			format = descriptor.Format.ToVkFormat(),
			extent = new VkExtent3D((uint)Size.X, (uint)Size.Y, 1),
			mipLevels = 1,
			arrayLayers = 1,
			samples = VkSampleCountFlags.Count1,
			tiling = VkImageTiling.Linear,
			usage = VkImageUsageFlags.Sampled | VkImageUsageFlags.TransferDst | VkImageUsageFlags.TransferSrc,
			sharingMode = VkSharingMode.Exclusive,
			initialLayout = _layout = VkImageLayout.Undefined,
		};
		api.vkCreateImage(&info, out VkImage image).CheckResult();
		Image = image;

		api.vkGetImageMemoryRequirements(image, out VkMemoryRequirements req);
		VkMemoryAllocateInfo alloc = new()
		{
			allocationSize = req.size,
			memoryTypeIndex = device.FindMemoryType(req.memoryTypeBits, VkMemoryPropertyFlags.DeviceLocal),
		};
		api.vkAllocateMemory(&alloc, out _memory).CheckResult();
		api.vkBindImageMemory(image, _memory, 0).CheckResult();

		VkImageViewCreateInfo viewInfo = new()
		{
			image = image,
			viewType = VkImageViewType.Image2D,
			format = info.format,
			subresourceRange = new VkImageSubresourceRange(VkImageAspectFlags.Color, 0, 1, 0, 1),
		};
		api.vkCreateImageView(&viewInfo, out VkImageView view).CheckResult();
		View = view;

		if (!initialData.IsEmpty)
		{
			Update(initialData);
		}
	}

	public override unsafe void Update(ReadOnlySpan<byte> pixels)
	{
		ThrowIfDisposed();
		ValidateSize(pixels.Length);
		if (pixels.IsEmpty)
		{
			return;
		}

		VkDeviceApi api = _device.DeviceApi;
		(VkBuffer staging, VkDeviceMemory stagingMemory) = CreateStagingBuffer((ulong)pixels.Length);
		try
		{
			void* mapped;
			api.vkMapMemory(stagingMemory, 0, (ulong)pixels.Length, VkMemoryMapFlags.None, &mapped).CheckResult();
			pixels.CopyTo(new Span<byte>(mapped, pixels.Length));
			api.vkUnmapMemory(stagingMemory);

			_device.RunOnce(cmd =>
			{
				Transition(cmd, VkImageLayout.TransferDstOptimal);
				VkBufferImageCopy region = CreateCopyRegion();
				api.vkCmdCopyBufferToImage(cmd, staging, Image, VkImageLayout.TransferDstOptimal, 1, &region);
				Transition(cmd, VkImageLayout.ShaderReadOnlyOptimal);
			});
		}
		finally
		{
			api.vkDestroyBuffer(staging);
			api.vkFreeMemory(stagingMemory);
		}
	}

	public override unsafe void GetPixels(Span<byte> destination)
	{
		ThrowIfDisposed();
		ValidateSize(destination.Length);
		if (destination.IsEmpty)
		{
			return;
		}

		VkDeviceApi api = _device.DeviceApi;
		(VkBuffer staging, VkDeviceMemory stagingMemory) = CreateStagingBuffer((ulong)destination.Length);
		try
		{
			_device.RunOnce(cmd =>
			{
				Transition(cmd, VkImageLayout.TransferSrcOptimal);
				VkBufferImageCopy region = CreateCopyRegion();
				api.vkCmdCopyImageToBuffer(cmd, Image, VkImageLayout.TransferSrcOptimal, staging, 1, &region);
				Transition(cmd, VkImageLayout.ShaderReadOnlyOptimal);
			});

			void* mapped;
			api.vkMapMemory(stagingMemory, 0, (ulong)destination.Length, VkMemoryMapFlags.None, &mapped).CheckResult();
			new ReadOnlySpan<byte>(mapped, destination.Length).CopyTo(destination);
			api.vkUnmapMemory(stagingMemory);
		}
		finally
		{
			api.vkDestroyBuffer(staging);
			api.vkFreeMemory(stagingMemory);
		}
	}

	private void ValidateSize(int byteCount)
	{
		int expected = Size.X * Size.Y * Format.GetBytesPerPixel();
		if (byteCount != expected)
		{
			throw new ArgumentException($"Expected {expected} bytes of pixel data, got {byteCount}.");
		}
	}

	private unsafe (VkBuffer, VkDeviceMemory) CreateStagingBuffer(ulong size)
	{
		VkDeviceApi api = _device.DeviceApi;
		VkBufferCreateInfo info = new()
		{
			size = size,
			usage = VkBufferUsageFlags.TransferSrc | VkBufferUsageFlags.TransferDst,
			sharingMode = VkSharingMode.Exclusive,
		};
		api.vkCreateBuffer(&info, out VkBuffer buffer).CheckResult();

		api.vkGetBufferMemoryRequirements(buffer, out VkMemoryRequirements req);
		VkMemoryAllocateInfo alloc = new()
		{
			allocationSize = req.size,
			memoryTypeIndex = _device.FindMemoryType(req.memoryTypeBits,
				VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent),
		};
		api.vkAllocateMemory(&alloc, out VkDeviceMemory memory).CheckResult();
		api.vkBindBufferMemory(buffer, memory, 0).CheckResult();
		return (buffer, memory);
	}

	private VkBufferImageCopy CreateCopyRegion()
	{
		return new VkBufferImageCopy
		{
			imageSubresource = new VkImageSubresourceLayers(VkImageAspectFlags.Color, 0, 0, 1),
			imageOffset = new VkOffset3D(0, 0, 0),
			imageExtent = new VkExtent3D((uint)Size.X, (uint)Size.Y, 1),
		};
	}

	private unsafe void Transition(VkCommandBuffer cmd, VkImageLayout newLayout)
	{
		if (_layout == newLayout)
		{
			return;
		}

		(VkPipelineStageFlags srcStage, VkAccessFlags srcAccess) = Describe(_layout);
		(VkPipelineStageFlags dstStage, VkAccessFlags dstAccess) = Describe(newLayout);

		VkImageMemoryBarrier barrier = new()
		{
			oldLayout = _layout,
			newLayout = newLayout,
			srcAccessMask = srcAccess,
			dstAccessMask = dstAccess,
			srcQueueFamilyIndex = Vk.VK_QUEUE_FAMILY_IGNORED,
			dstQueueFamilyIndex = Vk.VK_QUEUE_FAMILY_IGNORED,
			image = Image,
			subresourceRange = new VkImageSubresourceRange(VkImageAspectFlags.Color, 0, 1, 0, 1),
		};

		_device.DeviceApi.vkCmdPipelineBarrier(cmd, srcStage, dstStage, 0, 0, null, 0, null, 1, &barrier);
		_layout = newLayout;
	}

	private static (VkPipelineStageFlags, VkAccessFlags) Describe(VkImageLayout layout)
	{
		return layout switch
		{
			VkImageLayout.Undefined => (VkPipelineStageFlags.TopOfPipe, VkAccessFlags.None),
			VkImageLayout.TransferDstOptimal => (VkPipelineStageFlags.Transfer, VkAccessFlags.TransferWrite),
			VkImageLayout.TransferSrcOptimal => (VkPipelineStageFlags.Transfer, VkAccessFlags.TransferRead),
			VkImageLayout.ShaderReadOnlyOptimal => (VkPipelineStageFlags.AllGraphics, VkAccessFlags.ShaderRead),
			_ => throw new NotSupportedException($"Layout {layout} is not handled yet."),
		};
	}

	// TODO: deferred destruction.
	protected override void Dispose(bool disposing)
	{
		_device.WaitIdle();
		_device.DeviceApi.vkDestroyImageView(View);
		_device.DeviceApi.vkDestroyImage(Image);
		_device.DeviceApi.vkFreeMemory(_memory);
	}
}