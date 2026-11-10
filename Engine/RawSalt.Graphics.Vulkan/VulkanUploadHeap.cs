using System.Diagnostics;
using RawSalt.Graphics.Rendering;

namespace RawSalt.Graphics.Vulkan;

internal sealed unsafe class VulkanUploadHeap : Disposable
{
	private readonly VulkanDevice _device;
	private readonly VkDeviceMemory _memory;
	private readonly byte* _mapped;

	public VkBuffer Handle { get; }
	public uint Size { get; }

	public VulkanUploadHeap(VulkanDevice device, uint size)
	{
		_device = device;
		Size = size;
		VkDeviceApi api = device.DeviceApi;

		VkBufferCreateInfo bufferInfo = new()
		{
			size = size,
			usage = VkBufferUsageFlags.UniformBuffer,
			sharingMode = VkSharingMode.Exclusive,
		};
		api.vkCreateBuffer(&bufferInfo, out VkBuffer buffer).CheckResult();
		Handle = buffer;

		api.vkGetBufferMemoryRequirements(buffer, out VkMemoryRequirements req);
		VkMemoryAllocateInfo allocateInfo = new()
		{
			allocationSize = req.size,
			memoryTypeIndex = device.FindMemoryType(req.memoryTypeBits, VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent)
		};
		api.vkAllocateMemory(&allocateInfo, out _memory).CheckResult();
		api.vkBindBufferMemory(buffer, _memory, 0).CheckResult();

		void* mapped;
		api.vkMapMemory(_memory, 0, size, VkMemoryMapFlags.None, &mapped).CheckResult();
		_mapped = (byte*)mapped;
	}

	public UploadMapArena CreateArena(int index, uint regionSize)
	{
		uint offset = (uint)index * regionSize;
		Debug.Assert(offset + regionSize <= Size, "Region is outside the upload heap.");
		return new UploadMapArena(_device, _mapped + offset, offset, regionSize);
	}

	protected override void Dispose(bool disposing)
	{
		VkDeviceApi api = _device.DeviceApi;
		api.vkUnmapMemory(_memory);
		api.vkDestroyBuffer(Handle);
		api.vkFreeMemory(_memory);
	}

}