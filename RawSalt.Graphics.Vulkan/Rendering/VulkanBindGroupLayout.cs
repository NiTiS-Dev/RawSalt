using System.Diagnostics;
using RawSalt.Graphics.Rendering;

namespace RawSalt.Graphics.Vulkan.Rendering;

public sealed class VulkanBindGroupLayout : BindGroupLayout
{
	private readonly HashSet<uint> _unsetImages;

	public VulkanDevice Device { get; }
	public VkDescriptorSetLayout Handle { get; }
	public VkDescriptorSet DescriptorSet { get; }

	internal bool HasUnsetImages => _unsetImages.Count > 0;

	public unsafe VulkanBindGroupLayout(VulkanDevice device, ReadOnlySpan<BindingEntry> entries)
		: base(entries)
	{
		Device = device;

		BindingEntry[] bindings = Bindings;
		_unsetImages = bindings
			.Where(static e => e.Type == BindingType.CombinedImageSampler)
			.Select(static e => e.Binding)
			.ToHashSet();

		VkDescriptorSetLayoutBinding* pBindings = stackalloc VkDescriptorSetLayoutBinding[bindings.Length];
		for (int i = 0; i < bindings.Length; i++)
		{
			BindingEntry entry = bindings[i];
			Debug.Assert(entry.Count > 0);

			pBindings[i] = new()
			{
				binding = entry.Binding,
				stageFlags = entry.Stages.ToVkFlags(),
				descriptorType = entry.Type.ToVk(),
				descriptorCount = entry.Count,
			};
		}

		VkDescriptorSetLayoutCreateInfo info = new()
		{
			bindingCount = (uint)bindings.Length,
			pBindings = pBindings,
		};
		device.DeviceApi.vkCreateDescriptorSetLayout(&info, out VkDescriptorSetLayout handle).CheckResult();
		DescriptorSet = AllocateAndWrite(device, handle, bindings);
		Handle = handle;
	}

	public override unsafe void SetTexture(uint binding, Texture2D texture)
	{
		ThrowIfDisposed();
		if (texture is not VulkanTexture2D vulkanTexture || vulkanTexture.Device != Device)
		{
			throw new ArgumentException("Texture was created by a different backend or device.", nameof(texture));
		}

		BindingEntry entry = Bindings.FirstOrDefault(e => e.Binding == binding);
		if (entry.Type != BindingType.CombinedImageSampler)
		{
			throw new ArgumentOutOfRangeException(nameof(binding),
				$"Binding {binding} is not a combined image sampler in this layout.");
		}

		if (entry.Count != 1)
		{
			throw new NotSupportedException("Image sampler arrays are not supported yet.");
		}

		// shortcut: the descriptor set may still be referenced by an in-flight frame.
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

	private static unsafe VkDescriptorSet AllocateAndWrite(VulkanDevice device, VkDescriptorSetLayout setLayout,
		BindingEntry[] bindings)
	{
		VkDeviceApi api = device.DeviceApi;

		VkDescriptorSetAllocateInfo allocInfo = new()
		{
			descriptorPool = device.DescriptorPool,
			descriptorSetCount = 1,
			pSetLayouts = &setLayout,
		};
		api.vkAllocateDescriptorSets(in allocInfo, out VkDescriptorSet set).CheckResult();

		int count = bindings.Count(static e => e.Type == BindingType.UniformBuffer);
		if (count == 0)
		{
			return set;
		}

		VkDescriptorBufferInfo* infos = stackalloc VkDescriptorBufferInfo[count];
		VkWriteDescriptorSet* writes = stackalloc VkWriteDescriptorSet[count];
		int index = 0;
		foreach (BindingEntry entry in bindings)
		{
			if (entry.Type != BindingType.UniformBuffer)
			{
				continue;
			}

			infos[index] = new()
			{
				buffer = device.UploadHeap.Handle,
				offset = 0,
				range = entry.Size
			};
			writes[index] = new()
			{
				dstSet = set,
				dstBinding = entry.Binding,
				dstArrayElement = 0,
				descriptorCount = 1,
				descriptorType = entry.Type.ToVk(),
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
