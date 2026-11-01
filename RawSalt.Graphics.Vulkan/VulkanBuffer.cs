namespace RawSalt.Graphics.Vulkan;

public sealed class VulkanBuffer : Buffer
{
	private readonly VulkanDevice _device;
	private readonly VkDeviceMemory _memory;

	public VkBuffer Handle { get; }
	public override ulong Size { get; }

	public unsafe VulkanBuffer(VulkanDevice device, in BufferDescriptor descriptor, ReadOnlySpan<byte> initialData)
	{
		if ((ulong)initialData.Length > descriptor.Size)
		{
			throw new ArgumentException("Initial data is larger than the buffer.", nameof(initialData));
		}

		_device = device;
		Size = descriptor.Size;
		VkDeviceApi api = device.DeviceApi;

		VkBufferUsageFlags usage = 0;
		if ((descriptor.Usage & BufferUsage.Vertex) != 0) usage |= VkBufferUsageFlags.VertexBuffer;
		if ((descriptor.Usage & BufferUsage.Index) != 0) usage |= VkBufferUsageFlags.IndexBuffer;
		if ((descriptor.Usage & BufferUsage.Uniform) != 0) usage |= VkBufferUsageFlags.UniformBuffer;

		VkBufferCreateInfo info = new()
		{
			size = descriptor.Size,
			usage = usage,
			sharingMode = VkSharingMode.Exclusive
		};
		api.vkCreateBuffer(&info, out VkBuffer buffer);
		api.vkGetBufferMemoryRequirements(buffer, out VkMemoryRequirements req);

		VkMemoryAllocateInfo alloc = new()
		{
			allocationSize = req.size,
			memoryTypeIndex = device.FindMemoryType(req.memoryTypeBits, VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent),
		};
		api.vkAllocateMemory(&alloc, out _memory);
		api.vkBindBufferMemory(buffer, _memory, 0);
		Handle = buffer;

		if (!initialData.IsEmpty)
		{
			void* mapped;
			api.vkMapMemory(_memory, 0, descriptor.Size, 0, &mapped);
			initialData.CopyTo(new Span<byte>(mapped, initialData.Length));
			api.vkUnmapMemory(_memory);
		}
	}

	// TODO: deferred destruction.
	protected override void Dispose(bool disposing)
	{
		_device.DeviceApi.vkDestroyBuffer(Handle);
		_device.DeviceApi.vkFreeMemory(_memory);
	}
}