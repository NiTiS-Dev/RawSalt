using System.Diagnostics;
using System.Numerics;
using RawSalt.Graphics.Pipelines;
using RawSalt.Graphics.Rendering;

namespace RawSalt.Graphics.Vulkan.Rendering;

internal sealed class VulkanCommandList : CommandList
{
	// These are Vulkan API minimum requirements, on desktop GPU there are probably more
	private const int MaxGroups = 4;
	private const int MaxDynamicPerGroup = 4;

	private readonly VkDeviceApi _api;
	private readonly UploadMapArena _uploadMapArena;
	private VulkanRenderTarget? _pass;

	public VkCommandBuffer Buffer { get; }

	private VulkanPipeline? _pipeline;

	// Per bind group state. Recorded offsets stay valid across pipeline changes
	// as long as the group's layout is unchanged.
	private readonly VulkanBindGroupLayout?[] _groupLayouts = new VulkanBindGroupLayout?[MaxGroups];
	private readonly uint[] _dynamicOffsets = new uint[MaxGroups * MaxDynamicPerGroup];
	private readonly uint[] _setMasks = new uint[MaxGroups];   // bit i = dynamic binding i of the group was set
	private uint _dirtyGroups;                                 // bit g = group g must be (re)bound before the next draw


	public VulkanCommandList(VkDeviceApi api, VkCommandBuffer buffer, UploadMapArena uploadMapArena)
	{
		_api = api;
		_uploadMapArena = uploadMapArena;
		Buffer = buffer;
	}

	public unsafe void Begin()
	{
		_api.vkResetCommandBuffer(Buffer, VkCommandBufferResetFlags.None);

		// The command buffer can only be reset once its previous submission has finished,
		// which is exactly when the frame slot's upload arena is free to be reused too.
		_uploadMapArena.Reset();
		ResetBindingState();

		VkCommandBufferBeginInfo info = new()
		{
			flags = VkCommandBufferUsageFlags.OneTimeSubmit
		};
		_api.vkBeginCommandBuffer(Buffer, &info);
	}

	public void End()
	{
		Debug.Assert(_pass == null, "Render pass left open.");
		_api.vkEndCommandBuffer(Buffer);
	}

	public override unsafe void BeginRenderPass(in RenderPass passDescriptor)
	{
		if (_pass != null)
		{
			throw new InvalidOperationException("A render pass is already active.");
		}

		if (passDescriptor.Target is not VulkanRenderTarget target)
		{
			throw new ArgumentException("Target belongs to a different backend.", nameof(passDescriptor));
		}

		Transition(target, VkImageLayout.ColorAttachmentOptimal);

		VkRenderingAttachmentInfo attachment = new()
		{
			imageView = target.View,
			imageLayout = VkImageLayout.ColorAttachmentOptimal,
			loadOp = passDescriptor.Clear is null ? VkAttachmentLoadOp.Load : VkAttachmentLoadOp.Clear,
			storeOp = VkAttachmentStoreOp.Store,
		};

		if (passDescriptor.Clear is { } color)
		{
			attachment.clearValue = new VkClearValue
			{
				color = new VkClearColorValue(color.Red, color.Green, color.Blue, color.Alpha)
			};
		}

		VkRenderingInfo info = new()
		{
			renderArea = new VkRect2D(new VkOffset2D(0, 0), target.Extent),
			layerCount = 1,
			colorAttachmentCount = 1,
			pColorAttachments = &attachment,
		};

		VkViewport viewport = new()
		{
			x = 0, y = target.Extent.height,
			width = target.Extent.width,
			// negative height: Y-up NDC
			height = -(float)target.Extent.height,
			minDepth = 0,
			maxDepth = 1,
		};
		VkRect2D scissor = new(new VkOffset2D(0, 0), target.Extent);
		_api.vkCmdSetViewport(Buffer, 0, 1, &viewport);
		_api.vkCmdSetScissor(Buffer, 0, 1, &scissor);

		_api.vkCmdBeginRendering(Buffer, &info);
		_pass = target;
	}

	public override void EndRenderPass()
	{
		if (_pass == null)
		{
			throw new InvalidOperationException("No active render pass.");
		}

		_api.vkCmdEndRendering(Buffer);
		_pass = null;
	}

	public override void SetPipeline(Pipeline pipeline)
	{
		if (pipeline is not VulkanPipeline vkPipeline)
		{
			throw new ArgumentException("Pipeline must be a Vulkan pipeline.", nameof(pipeline));
		}
		Debug.Assert(_pass != null, "SetPipeline outside a render pass.");
		Debug.Assert(vkPipeline.ColorFormat == _pass.Format.ToPixelFormat(), "Pipeline color format does not match the render target.");

		VulkanBindGroupLayout[] layouts = vkPipeline.BindGroupLayouts;
		Debug.Assert(layouts.Length <= MaxGroups, $"A pipeline can use at most {MaxGroups} bind groups for now.");

		// Descriptor sets stay bound across pipeline changes only while the layouts are compatible.
		// Everything from the first differing group on has to be bound again.
		int first = FirstChangedGroup(_pipeline?.BindGroupLayouts, layouts);
		for (int g = first; g < layouts.Length; g++)
		{
			if (!ReferenceEquals(_groupLayouts[g], layouts[g]))
			{
				_groupLayouts[g] = layouts[g];
				_setMasks[g] = 0;   // recorded offsets belonged to a different layout
			}
			_dirtyGroups |= 1u << g;
		}

		_pipeline = vkPipeline;
		_api.vkCmdBindPipeline(Buffer, VkPipelineBindPoint.Graphics, vkPipeline.Handle);
	}

	public override void SetUniform<T>(uint set, uint bind, in T data)
	{
		SetUniform(set, bind, _uploadMapArena.Allocate(in data));
	}

	public void SetUniform(uint set, uint bind, in UniformRef buffer)
	{
		Debug.Assert(_pipeline != null, "SetPipeline must be called before setting uniforms.");
		Debug.Assert(set < _pipeline.BindGroupLayouts.Length, $"The pipeline has no bind group {set}.");

		VulkanBindGroupLayout layout = _pipeline.BindGroupLayouts[set];
		int index = layout.GetDynamicOffsetIndex(bind);
		Debug.Assert(index < MaxDynamicPerGroup);
		Debug.Assert(buffer.Size >= Array.Find(layout.Bindings, e => e.Binding == bind).Size, "Uniform data is smaller than the block declared in the layout.");

		_dynamicOffsets[set * MaxDynamicPerGroup + index] = buffer.Offset;
		_setMasks[set] |= 1u << index;
		_dirtyGroups |= 1u << (int)set;
	}

	public override unsafe void SetVertexBuffer(uint slot, Buffer buffer, ulong offset = 0)
	{
		Debug.Assert(buffer is VulkanBuffer);

		VkBuffer handle = ((VulkanBuffer)buffer).Handle;
		_api.vkCmdBindVertexBuffers(Buffer, slot, 1, &handle, &offset);
	}

	public override void SetIndexBuffer(Buffer buffer, ulong offset = 0)
	{
		Debug.Assert(buffer is VulkanBuffer);

		VkBuffer handle = ((VulkanBuffer)buffer).Handle;
		_api.vkCmdBindIndexBuffer(Buffer, handle, offset, VkIndexType.Uint32);
	}

	public override void Draw(uint vertexCount, uint instanceCount = 1, uint firstVertex = 0, uint firstInstance = 0)
	{
		BindGroups();
		_api.vkCmdDraw(Buffer, vertexCount, instanceCount, firstVertex, firstInstance);
	}

	public override void DrawIndexed(uint indexesCount, uint instanceCount = 1, uint firstIndex = 0, int vertexOffset = 0, uint firstInstance = 0)
	{
		BindGroups();
		_api.vkCmdDrawIndexed(Buffer, indexesCount, instanceCount, firstIndex, vertexOffset, firstInstance);
	}

	private unsafe void BindGroups()
	{
		ValidateBindings();

		VulkanPipeline pipeline = _pipeline!;
		uint dirty = _dirtyGroups;
		_dirtyGroups = 0;

		while (dirty != 0)
		{
			int g = BitOperations.TrailingZeroCount(dirty);
			dirty &= dirty - 1;

			VulkanBindGroupLayout layout = pipeline.BindGroupLayouts[g];
			VkDescriptorSet descriptorSet = layout.DescriptorSet;
			fixed (uint* pOffsets = &_dynamicOffsets[g * MaxDynamicPerGroup])
			{
				_api.vkCmdBindDescriptorSets(
					Buffer,
					VkPipelineBindPoint.Graphics,
					pipeline.Layout,
					firstSet: (uint)g,
					descriptorSetCount: 1,
					&descriptorSet,
					(uint)layout.DynamicOffsetCount,
					pOffsets
				);
			}
		}
	}

	[Conditional("DEBUG")]
	private void ValidateBindings()
	{
		Debug.Assert(_pipeline != null, "Draw without a pipeline.");
		VulkanBindGroupLayout[] layouts = _pipeline!.BindGroupLayouts;
		for (int g = 0; g < layouts.Length; g++)
		{
			uint required = (1u << layouts[g].DynamicOffsetCount) - 1;
			Debug.Assert(_setMasks[g] == required, $"Bind group {g}: not all uniform buffers were set before drawing.");
		}
	}

	private void ResetBindingState()
	{
		_pipeline = null;
		_dirtyGroups = 0;
		Array.Clear(_groupLayouts);
		Array.Clear(_setMasks);
	}

	private static int FirstChangedGroup(VulkanBindGroupLayout[]? previous, VulkanBindGroupLayout[] current)
	{
		if (previous == null)
		{
			return 0;
		}

		int count = Math.Min(previous.Length, current.Length);
		for (int g = 0; g < count; g++)
		{
			if (!ReferenceEquals(previous[g], current[g]))
			{
				return g;
			}
		}
		return count;
	}

	public unsafe void Transition(VulkanRenderTarget target, VkImageLayout newLayout)
	{
		if (target.Layout == newLayout)
		{
			return;
		}

		(VkPipelineStageFlags srcStage, VkAccessFlags srcAccess) = Describe(target.Layout);
		(VkPipelineStageFlags dstStage, VkAccessFlags dstAccess) = Describe(newLayout);

		VkImageMemoryBarrier barrier = new()
		{
			oldLayout = target.Layout,
			newLayout = newLayout,
			srcAccessMask = srcAccess,
			dstAccessMask = dstAccess,
			srcQueueFamilyIndex = Vk.VK_QUEUE_FAMILY_IGNORED,
			dstQueueFamilyIndex = Vk.VK_QUEUE_FAMILY_IGNORED,
			image = target.Image,
			subresourceRange = new VkImageSubresourceRange(VkImageAspectFlags.Color, 0, 1, 0, 1),
		};

		_api.vkCmdPipelineBarrier(Buffer, srcStage, dstStage, 0, 0, null, 0, null, 1, &barrier);
		target.Layout = newLayout;
	}

	private static (VkPipelineStageFlags, VkAccessFlags) Describe(VkImageLayout layout)
	{
		return layout switch
		{
			VkImageLayout.Undefined => (VkPipelineStageFlags.ColorAttachmentOutput, VkAccessFlags.None),
			VkImageLayout.ColorAttachmentOptimal => (VkPipelineStageFlags.ColorAttachmentOutput, VkAccessFlags.ColorAttachmentWrite),
			VkImageLayout.PresentSrcKHR => (VkPipelineStageFlags.BottomOfPipe, VkAccessFlags.None),
			_ => throw new NotSupportedException($"Layout {layout} is not handled yet."),
		};
	}
}