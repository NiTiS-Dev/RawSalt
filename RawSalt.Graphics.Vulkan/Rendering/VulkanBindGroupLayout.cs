using System.Diagnostics;
using RawSalt.Graphics.Rendering;

namespace RawSalt.Graphics.Vulkan.Rendering;

public sealed class VulkanBindGroupLayout : BindGroupLayout
{
	public VulkanDevice Device { get; }
	public VkDescriptorSetLayout Handle { get; }

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
		Handle = handle;
	}

	protected override void Dispose(bool disposing)
	{
		Device.DeviceApi.vkDestroyDescriptorSetLayout(Handle);
	}
}