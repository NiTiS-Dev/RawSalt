using System.Diagnostics;
using RawSalt.Graphics.Pipelines;
using RawSalt.Graphics.Rendering;

namespace RawSalt.Graphics.Vulkan.Rendering;

internal sealed class VulkanBindGroupLayout : Disposable
{
	// Binding numbers of all dynamic-offset bindings, sorted ascending.
	// Vulkan expects the dynamic offsets array in exactly this order.
	private readonly uint[] _dynamicBindings;
	private readonly ShaderResource[] _resources;
	private readonly HashSet<uint> _unsetImages;

	public VulkanDevice Device { get; }
	public VkDescriptorSetLayout Handle { get; }
	public VkDescriptorSet DescriptorSet { get; }

	public int DynamicOffsetCount => _dynamicBindings.Length;

	internal bool HasUnsetImages => _unsetImages.Count > 0;

	public unsafe VulkanBindGroupLayout(VulkanDevice device, ReadOnlySpan<ShaderResource> resources)
	{
		Device = device;
		_resources = resources.ToArray();

		_dynamicBindings = _resources
			.Where(static r => r.Type == BindingType.UniformBuffer)
			.Select(static r => r.Binding)
			.Order().ToArray();
		_unsetImages = _resources
			.Where(static r => r.Type == BindingType.CombinedImageSampler)
			.Select(static r => r.Binding)
			.ToHashSet();

		VkDescriptorSetLayoutBinding* pBindings = stackalloc VkDescriptorSetLayoutBinding[_resources.Length];
		for (int i = 0; i < _resources.Length; i++)
		{
			ShaderResource resource = _resources[i];
			Debug.Assert(resource.Type != BindingType.UniformBuffer || resource.Size > 0);

			pBindings[i] = new()
			{
				binding = resource.Binding,
				// TODO: Get that info from reflection
				// Likely, when parameters provided via `uniform Foo bar` parameter of shader program
				// Slang can limit it's scope to only that shader program
				// Though, will it provide any performance? Maybe this stageFlags are only exists for guard purposes.
				stageFlags = (ShaderStages.Vertex | ShaderStages.Fragment).ToVkFlags(),
				descriptorType = resource.Type.ToVk(),
				descriptorCount = 1,
			};
		}

		VkDescriptorSetLayoutCreateInfo info = new()
		{
			bindingCount = (uint)_resources.Length,
			pBindings = pBindings,
		};
		device.DeviceApi.vkCreateDescriptorSetLayout(&info, out VkDescriptorSetLayout handle).CheckResult();
		DescriptorSet = AllocateAndWrite(device, handle, _resources);
		Handle = handle;
	}

	public int GetDynamicOffsetIndex(uint binding)
	{
		int index = Array.BinarySearch(_dynamicBindings, binding);
		if (index < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(binding), $"Binding {binding} is not a uniform buffer in this layout.");
		}

		return index;
	}

	public unsafe void SetTexture(uint binding, Texture2D texture)
	{
		ThrowIfDisposed();
		if (texture is not VulkanTexture2D vulkanTexture || vulkanTexture.Device != Device)
		{
			throw new ArgumentException("Texture was created by a different backend or device.", nameof(texture));
		}

		ShaderResource resource = _resources.FirstOrDefault(r => r.Binding == binding);
		if (resource.Type != BindingType.CombinedImageSampler)
		{
			throw new ArgumentOutOfRangeException(nameof(binding), $"Binding {binding} is not a combined image sampler in this layout.");
		}

		// TODO: The descriptor set may still be referenced by an in-flight frame.
		Device.WaitIdle();

		VkDescriptorImageInfo imageInfo = new()
		{
			sampler = vulkanTexture.Sampler,
			imageView = vulkanTexture.View,
			imageLayout = VkImageLayout.ShaderReadOnlyOptimal,
		};
		VkWriteDescriptorSet write = new()
		{
			dstSet = DescriptorSet,
			dstBinding = binding,
			dstArrayElement = 0,
			descriptorCount = 1,
			descriptorType = VkDescriptorType.CombinedImageSampler,
			pImageInfo = &imageInfo,
		};
		Device.DeviceApi.vkUpdateDescriptorSets(1, &write, 0, null);
		_unsetImages.Remove(binding);
	}

	private static unsafe VkDescriptorSet AllocateAndWrite(VulkanDevice device, VkDescriptorSetLayout setLayout, ShaderResource[] resources)
	{
		VkDeviceApi api = device.DeviceApi;

		VkDescriptorSetAllocateInfo allocInfo = new()
		{
			descriptorPool = device.DescriptorPool,
			descriptorSetCount = 1,
			pSetLayouts = &setLayout,
		};
		api.vkAllocateDescriptorSets(in allocInfo, out VkDescriptorSet set).CheckResult();

		int count = resources.Count(static r => r.Type == BindingType.UniformBuffer);
		if (count == 0)
		{
			return set;
		}

		VkDescriptorBufferInfo* infos = stackalloc VkDescriptorBufferInfo[count];
		VkWriteDescriptorSet* writes = stackalloc VkWriteDescriptorSet[count];
		int index = 0;
		foreach (ShaderResource resource in resources)
		{
			if (resource.Type != BindingType.UniformBuffer)
			{
				continue;
			}

			infos[index] = new()
			{
				buffer = device.UploadHeap.Handle,
				offset = 0,
				range = resource.Size
			};
			writes[index] = new()
			{
				dstSet = set,
				dstBinding = resource.Binding,
				dstArrayElement = 0,
				descriptorCount = 1,
				descriptorType = resource.Type.ToVk(),
				pBufferInfo = &infos[index],
			};
			index++;
		}
		api.vkUpdateDescriptorSets((uint)count, writes, 0, null);
		return set;
	}

	protected override void Dispose(bool disposing)
	{
		Device.DeviceApi.vkDestroyDescriptorSetLayout(Handle);
	}
}
