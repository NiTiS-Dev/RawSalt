using NiTiS.Mathematics;
using RawSalt.Graphics.Drawing;
using RawSalt.Graphics.Resources;

namespace RawSalt.Graphics.Vulkan;

public sealed class VulkanTextureResource : TextureResource
{
	private readonly VulkanDevice _device;
	private readonly VkDeviceMemory _memory;
	private VkImageLayout _layout;

	public override TextureDimension Dimension { get; }
	public override Vector3<int> Size { get; }
	public override PixelFormat Format { get; }
	public VulkanDevice Device => _device;
	public VkImage Image { get; }
	public VkImageView View { get; }
	public VkSampler Sampler { get; }

	public unsafe VulkanTextureResource(VulkanDevice device, in TextureDescriptor descriptor, ReadOnlySpan<byte> initialData)
	{
		ValidateDescriptor(device, descriptor);

		_device = device;
		Dimension = descriptor.Dimension;
		Size = descriptor.Size;
		Format = descriptor.Format;

		VkDeviceApi api = device.DeviceApi;
		uint layers = (uint)LayerCount;
		VkImageCreateInfo info = new()
		{
			flags = Dimension == TextureDimension.Cube ? VkImageCreateFlags.CubeCompatible : VkImageCreateFlags.None,
			imageType = Dimension switch
			{
				TextureDimension.Texture1D => VkImageType.Image1D,
				TextureDimension.Texture3D => VkImageType.Image3D,
				_ => VkImageType.Image2D,
			},
			format = descriptor.Format.ToVkFormat(),
			extent = new VkExtent3D((uint)Size.X, (uint)Size.Y, (uint)Size.Z),
			mipLevels = 1,
			arrayLayers = layers,
			samples = VkSampleCountFlags.Count1,
			tiling = VkImageTiling.Optimal,
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
			viewType = Dimension switch
			{
				TextureDimension.Texture1D => VkImageViewType.Image1D,
				TextureDimension.Texture3D => VkImageViewType.Image3D,
				TextureDimension.Cube => VkImageViewType.ImageCube,
				_ => VkImageViewType.Image2D,
			},
			format = info.format,
			subresourceRange = new VkImageSubresourceRange(VkImageAspectFlags.Color, 0, 1, 0, layers),
		};
		api.vkCreateImageView(&viewInfo, out VkImageView view).CheckResult();
		View = view;

		VkSamplerCreateInfo samplerInfo = new()
		{
			magFilter = VkFilter.Linear,
			minFilter = VkFilter.Linear,
			mipmapMode = VkSamplerMipmapMode.Nearest,
			addressModeU = descriptor.Sampling.ToVk(),
			addressModeV = descriptor.Sampling.ToVk(),
			addressModeW = descriptor.Sampling.ToVk(),
			compareOp = VkCompareOp.Never,
			minLod = 0,
			maxLod = 0,
		};
		api.vkCreateSampler(&samplerInfo, out VkSampler sampler).CheckResult();
		Sampler = sampler;

		if (initialData.IsEmpty)
		{
			// Sampling descriptors declare ShaderReadOnlyOptimal, so the image must actually be in it.
			_device.RunOnce(cmd => Transition(cmd, VkImageLayout.ShaderReadOnlyOptimal));
		}
		else
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
				for (uint layer = 0; layer < (uint)LayerCount; layer++)
				{
					VkBufferImageCopy region = CreateCopyRegion(layer);
					api.vkCmdCopyBufferToImage(cmd, staging, Image, VkImageLayout.TransferDstOptimal, 1, &region);
				}
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
				for (uint layer = 0; layer < (uint)LayerCount; layer++)
				{
					VkBufferImageCopy region = CreateCopyRegion(layer);
					api.vkCmdCopyImageToBuffer(cmd, Image, VkImageLayout.TransferSrcOptimal, staging, 1, &region);
				}
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

	private static void ValidateDescriptor(VulkanDevice device, in TextureDescriptor descriptor)
	{
		if (descriptor.Format == PixelFormat.Unknown)
		{
			throw new ArgumentException("Texture format must be specified.", nameof(descriptor));
		}

		Vector3<int> size = descriptor.Size;
		uint limit;
		switch (descriptor.Dimension)
		{
			case TextureDimension.Texture1D:
				if (size.X <= 0 || size.Y != 1 || size.Z != 1)
				{
					throw new ArgumentOutOfRangeException(nameof(descriptor), "A 1D texture must have a positive width and unit height and depth.");
				}
				limit = device.Adapter.Limits.MaxImageDimension1D;
				break;

			case TextureDimension.Texture2D:
				if (size.X <= 0 || size.Y <= 0 || size.Z != 1)
				{
					throw new ArgumentOutOfRangeException(nameof(descriptor), "A 2D texture must have a positive width and height and unit depth.");
				}
				limit = device.Adapter.Limits.MaxImageDimension2D;
				break;

			case TextureDimension.Texture3D:
				if (size.X <= 0 || size.Y <= 0 || size.Z <= 0)
				{
					throw new ArgumentOutOfRangeException(nameof(descriptor), "A 3D texture must have positive width, height and depth.");
				}
				limit = device.Adapter.Limits.MaxImageDimension3D;
				break;

			case TextureDimension.Cube:
				if (size.X <= 0 || size.X != size.Y || size.Z != 1)
				{
					throw new ArgumentOutOfRangeException(nameof(descriptor), "A cubemap must have equal, positive width and height and unit depth.");
				}
				limit = device.Adapter.Limits.MaxImageDimensionCube;
				break;

			default:
				throw new ArgumentOutOfRangeException(nameof(descriptor), $"Unsupported texture dimension {descriptor.Dimension}.");
		}

		if ((uint)size.X > limit || (uint)size.Y > limit || (uint)size.Z > limit)
		{
			throw new ArgumentOutOfRangeException(nameof(descriptor), "Texture size exceeds the device limit.");
		}
	}

	private void ValidateSize(int byteCount)
	{
		if ((ulong)byteCount != ByteSize)
		{
			throw new ArgumentException($"Expected {ByteSize} bytes of pixel data, got {byteCount}.");
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

	private VkBufferImageCopy CreateCopyRegion(uint layer)
	{
		ulong layerBytes = (ulong)Size.X * (ulong)Size.Y * (ulong)Size.Z * (ulong)Format.GetBytesPerPixel();
		return new VkBufferImageCopy
		{
			bufferOffset = layer * layerBytes,
			imageSubresource = new VkImageSubresourceLayers(VkImageAspectFlags.Color, 0, layer, 1),
			imageOffset = new VkOffset3D(0, 0, 0),
			imageExtent = new VkExtent3D((uint)Size.X, (uint)Size.Y, (uint)Size.Z),
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
			subresourceRange = new VkImageSubresourceRange(VkImageAspectFlags.Color, 0, 1, 0, (uint)LayerCount),
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

	protected override void Dispose(bool disposing)
	{
		_device.DeviceApi.vkDestroySampler(Sampler);
		_device.DeviceApi.vkDestroyImageView(View);
		_device.DeviceApi.vkDestroyImage(Image);
		_device.DeviceApi.vkFreeMemory(_memory);
	}
}
