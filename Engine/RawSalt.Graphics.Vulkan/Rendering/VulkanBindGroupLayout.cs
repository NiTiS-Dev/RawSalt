using System.Diagnostics;
using RawSalt.Graphics.Pipelines;

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
		DescriptorSet = device.DescriptorHeap.Allocate(handle, _resources);
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

	public void SetTexture(uint binding, VulkanTextureResource texture)
	{
		ThrowIfDisposed();
		if (!ReferenceEquals(texture.Device, Device))
		{
			throw new ArgumentException("Texture was created by a different device.", nameof(texture));
		}

		ShaderResource resource = _resources.FirstOrDefault(r => r.Binding == binding);
		if (resource.Type != BindingType.CombinedImageSampler)
		{
			throw new ArgumentOutOfRangeException(nameof(binding), $"Binding {binding} is not a combined image sampler in this layout.");
		}

		// TODO: The descriptor set may still be referenced by an in-flight frame.
		Device.WaitIdle();

		Device.DescriptorHeap.SetTexture(DescriptorSet, binding, texture);
		_unsetImages.Remove(binding);
	}

	protected override void Dispose(bool disposing)
	{
		Device.DescriptorHeap.Free(DescriptorSet);
		Device.DeviceApi.vkDestroyDescriptorSetLayout(Handle);
	}
}
