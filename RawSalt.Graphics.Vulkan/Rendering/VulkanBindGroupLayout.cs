using System.Diagnostics;
using RawSalt.Graphics.Rendering;

namespace RawSalt.Graphics.Vulkan.Rendering;

public sealed class VulkanBindGroupLayout : BindGroupLayout
{
	public VulkanDevice Device { get; }
	public VkDescriptorSetLayout Handle { get; }
	public VkDescriptorSet DescriptorSet { get; }

	public unsafe VulkanBindGroupLayout(VulkanDevice device, ReadOnlySpan<BindingEntry> entries)
		: base(entries)
	{
		Device = device;

		BindingEntry[] bindings = Bindings;
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

		int count = bindings.Length;
		if (count == 0)
		{
			return set;
		}

		VkDescriptorBufferInfo* infos = stackalloc VkDescriptorBufferInfo[count];
		VkWriteDescriptorSet* writes = stackalloc VkWriteDescriptorSet[count];
		for (int i = 0; i < count; i++)
		{
			BindingEntry entry = bindings[i];
			infos[i] = new()
			{
				buffer = device.UploadHeap.Handle,
				offset = 0,
				range = entry.Size
			};
			writes[i] = new()
			{
				dstSet = set,
				dstBinding = entry.Binding,
				dstArrayElement = 0,
				descriptorCount = 1,
				descriptorType = entry.Type.ToVk(),
				pBufferInfo = &infos[i],
			};
		}
		api.vkUpdateDescriptorSets((uint)count, writes, 0, null);
		return set;
	}


	protected override void Dispose(bool disposing)
	{
		Device.DeviceApi.vkDestroyDescriptorSetLayout(Handle);
	}
}