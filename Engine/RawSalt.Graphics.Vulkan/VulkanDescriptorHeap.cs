using System.Diagnostics;
using RawSalt.Graphics.Pipelines;

namespace RawSalt.Graphics.Vulkan;

/// <summary>
/// Owns the device's descriptor storage. Everything that needs a descriptor set goes through this class.
/// </summary>
internal sealed unsafe class VulkanDescriptorHeap : Disposable
{
	private const uint InitialSets = 64;
	private const uint InitialUniformBuffers = 64;
	private const uint InitialImageSamplers = 64;

	private readonly VulkanDevice _device;
	private readonly List<VkDescriptorPool> _pools = [];
	private readonly Dictionary<VkDescriptorSet, VkDescriptorPool> _poolOfSet = [];

	private VkDescriptorPool _current;
	private uint _sets = InitialSets;
	private uint _uniformBuffers = InitialUniformBuffers;
	private uint _imageSamplers = InitialImageSamplers;

	public VulkanDescriptorHeap(VulkanDevice device)
	{
		_device = device;
		_current = CreatePool(_sets, _uniformBuffers, _imageSamplers);
	}

	/// <summary>
	/// Allocates a descriptor set for <paramref name="setLayout"/> and points its uniform buffers at
	/// the device upload heap. Per-draw ranges are selected with dynamic offsets at bind time.
	/// </summary>
	public VkDescriptorSet Allocate(VkDescriptorSetLayout setLayout, ShaderResource[] resources)
	{
		ThrowIfDisposed();
		VkDeviceApi api = _device.DeviceApi;

		uint uniformBuffers = (uint)resources.Count(static r => r.Type == BindingType.UniformBuffer);
		uint imageSamplers = (uint)resources.Length - uniformBuffers;

		VkDescriptorSet set;
		while (true)
		{
			VkDescriptorSetAllocateInfo allocInfo = new()
			{
				descriptorPool = _current,
				descriptorSetCount = 1,
				pSetLayouts = &setLayout,
			};
			VkResult result = api.vkAllocateDescriptorSets(in allocInfo, out set);
			if (result == VkResult.Success)
			{
				break;
			}

			// The pool is out of sets or of this layout's descriptors; a larger pool may fit it.
			if (result is not (VkResult.ErrorOutOfPoolMemory or VkResult.ErrorFragmentedPool))
			{
				throw new VkException(result, "Failed to allocate a descriptor set.");
			}

			Grow(uniformBuffers, imageSamplers);
		}

		_poolOfSet[set] = _current;
		WriteUniformBuffers(api, set, resources);
		return set;
	}

	public void SetTexture(VkDescriptorSet set, uint binding, VulkanTextureResource texture)
	{
		ThrowIfDisposed();
		VkDescriptorImageInfo imageInfo = new()
		{
			sampler = texture.Sampler,
			imageView = texture.View,
			imageLayout = VkImageLayout.ShaderReadOnlyOptimal,
		};
		VkWriteDescriptorSet write = new()
		{
			dstSet = set,
			dstBinding = binding,
			dstArrayElement = 0,
			descriptorCount = 1,
			descriptorType = VkDescriptorType.CombinedImageSampler,
			pImageInfo = &imageInfo,
		};
		_device.DeviceApi.vkUpdateDescriptorSets(1, &write, 0, null);
	}

	/// <summary>
	/// Returns a set to its pool. The caller must ensure the GPU is no longer using it.
	/// </summary>
	public void Free(VkDescriptorSet set)
	{
		if (IsDisposed || !_poolOfSet.Remove(set, out VkDescriptorPool pool))
		{
			return;
		}

		_device.DeviceApi.vkFreeDescriptorSets(pool, 1, &set);
	}

	/// <summary>
	/// Adds a pool twice the size of the current one, big enough for the layout that did not fit.
	/// </summary>
	private void Grow(uint uniformBuffers, uint imageSamplers)
	{
		uint sets = _sets * 2;
		uint buffers = Math.Max(_uniformBuffers, uniformBuffers) * 2;
		uint samplers = Math.Max(_imageSamplers, imageSamplers) * 2;
		Debug.Assert(buffers >= uniformBuffers && samplers >= imageSamplers, "Grown pool must fit the requested layout.");

		_current = CreatePool(sets, buffers, samplers);
		_sets = sets;
		_uniformBuffers = buffers;
		_imageSamplers = samplers;
	}

	private VkDescriptorPool CreatePool(uint maxSets, uint uniformBuffers, uint imageSamplers)
	{
		VkDescriptorPoolSize* poolSizes = stackalloc VkDescriptorPoolSize[2];
		poolSizes[0] = new()
		{
			type = VkDescriptorType.UniformBufferDynamic,
			descriptorCount = uniformBuffers
		};
		poolSizes[1] = new()
		{
			type = VkDescriptorType.CombinedImageSampler,
			descriptorCount = imageSamplers
		};
		VkDescriptorPoolCreateInfo poolInfo = new()
		{
			flags = VkDescriptorPoolCreateFlags.FreeDescriptorSet,
			maxSets = maxSets,
			poolSizeCount = 2,
			pPoolSizes = poolSizes
		};
		_device.DeviceApi.vkCreateDescriptorPool(&poolInfo, out VkDescriptorPool pool).CheckResult();
		_pools.Add(pool);
		return pool;
	}

	private void WriteUniformBuffers(VkDeviceApi api, VkDescriptorSet set, ShaderResource[] resources)
	{
		int count = resources.Count(static r => r.Type == BindingType.UniformBuffer);
		if (count == 0)
		{
			return;
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
				buffer = _device.UploadHeap.Handle,
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
	}

	protected override void Dispose(bool disposing)
	{
		foreach (VkDescriptorPool pool in _pools)
		{
			_device.DeviceApi.vkDestroyDescriptorPool(pool);
		}
	}
}