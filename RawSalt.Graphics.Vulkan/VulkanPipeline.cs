using System.Diagnostics;
using RawSalt.Graphics.Drawing;
using RawSalt.Graphics.Rendering;
using RawSalt.Graphics.Vulkan.Rendering;

namespace RawSalt.Graphics.Vulkan;

public sealed class VulkanPipeline : Pipeline
{
	private readonly VulkanDevice _device;
	public VulkanBindGroupLayout[] BindGroupLayouts { get; }
	public VkPipelineLayout Layout { get; }

	public VkPipeline Handle { get; }
	public PixelFormat ColorFormat { get; }

	public unsafe VulkanPipeline(VulkanDevice device, PipelineDescriptor descriptor)
	{
		_device = device;
		ColorFormat = descriptor.ColorFormat;
		VkDeviceApi api = device.DeviceApi;

		VulkanShaderModule? vs = descriptor.VertexShader as VulkanShaderModule;
		VulkanShaderModule? fs = descriptor.FragmentShader as VulkanShaderModule;
		Debug.Assert(vs != null);
		Debug.Assert(fs != null);

		VkPipelineLayoutCreateInfo layoutInfo = new();
		VkDescriptorSetLayout* pDescriptorSetLayouts = stackalloc VkDescriptorSetLayout[descriptor.BindGroupLayout.Length];
		if (!descriptor.BindGroupLayout.IsDefaultOrEmpty)
		{
			BindGroupLayouts = new VulkanBindGroupLayout[descriptor.BindGroupLayout.Length];
			for (int i = 0; i < descriptor.BindGroupLayout.Length; i++)
			{
				BindGroupLayout bindingLayout = descriptor.BindGroupLayout[i];
				if (bindingLayout is not VulkanBindGroupLayout vkLayout)
				{
					throw new ArgumentException("BindingLayout was created by a different backend.", nameof(descriptor));
				}

				BindGroupLayouts[i] = vkLayout;
				pDescriptorSetLayouts[i] = vkLayout.Handle;
			}

			layoutInfo.setLayoutCount = (uint)descriptor.BindGroupLayout.Length;
			layoutInfo.pSetLayouts = pDescriptorSetLayouts;
		}
		else
		{
			BindGroupLayouts = [];
		}
		api.vkCreatePipelineLayout(&layoutInfo, out VkPipelineLayout layout).CheckResult();
		Layout = layout;

		VkVertexInputAttributeDescription[] attributes;
		if (descriptor.VertexLayout is { } vl)
		{
			attributes = vl.Attributes.Select(static a => new VkVertexInputAttributeDescription
			{
				location = a.Location,
				binding = 0,
				format = a.Format.ToVkFormat(),
				offset = a.Offset,
			}).ToArray();
		}
		else
		{
			attributes = [];
		}

		VkVertexInputBindingDescription binding = new()
		{
			binding = 0,
			stride = descriptor.VertexLayout?.Stride ?? 0,
			inputRate = VkVertexInputRate.Vertex,
		};

		VkPrimitiveTopology topology = descriptor.Topology switch
		{
			Topology.TriangleList => VkPrimitiveTopology.TriangleList,
			Topology.TriangleStrip => VkPrimitiveTopology.TriangleStrip,
			_ => VkPrimitiveTopology.PointList,
		};

		VkCullModeFlags cull = descriptor.CullMode switch
		{
			CullMode.Front => VkCullModeFlags.Front,
			CullMode.Back => VkCullModeFlags.Back,
			CullMode.FrontAndBack => VkCullModeFlags.FrontAndBack,
			_ => VkCullModeFlags.None,
		};

		VkFormat colorFormat = descriptor.ColorFormat.ToVkFormat();
		VkPipelineRenderingCreateInfo rendering = new()
		{
			colorAttachmentCount = 1,
			pColorAttachmentFormats = &colorFormat,
		};

		VkDynamicState* dynamic = stackalloc VkDynamicState[2]
		{
			VkDynamicState.Viewport,
			VkDynamicState.Scissor
		};
		VkPipelineDynamicStateCreateInfo dynamicState = new()
		{
			dynamicStateCount = 2,
			pDynamicStates = dynamic
		};

		VkPipelineViewportStateCreateInfo viewportState = new()
		{
			viewportCount = 1, scissorCount = 1
		};

		VkPipelineInputAssemblyStateCreateInfo inputAssembly = new()
		{
			topology = topology
		};

		VkPipelineRasterizationStateCreateInfo raster = new()
		{
			polygonMode = VkPolygonMode.Fill,
			cullMode = cull,
			frontFace = descriptor.FrontFace == FrontFace.CounterClockWise
				? VkFrontFace.CounterClockwise
				: VkFrontFace.Clockwise,
			lineWidth = 1f,
		};

		VkPipelineMultisampleStateCreateInfo multisample = new()
		{
			rasterizationSamples = VkSampleCountFlags.Count1
		};

		VkPipelineColorBlendAttachmentState blendAttachment = new()
		{
			blendEnable = false,
			colorWriteMask = VkColorComponentFlags.All
		};
		VkPipelineColorBlendStateCreateInfo blend = new()
		{
			attachmentCount = 1,
			pAttachments = &blendAttachment
		};

		fixed (byte* pEntryName = "main"u8)
		fixed (VkVertexInputAttributeDescription* pAttributes = attributes)
		{
			VkPipelineShaderStageCreateInfo* stages = stackalloc VkPipelineShaderStageCreateInfo[2];
			stages[0] = new()
			{
				stage = VkShaderStageFlags.Vertex,
				module = vs.Module,
				pName = pEntryName
			};
			stages[1] = new()
			{
				stage = VkShaderStageFlags.Fragment,
				module = fs.Module,
				pName = pEntryName
			};

			VkPipelineVertexInputStateCreateInfo vertexInput = new()
			{
				vertexBindingDescriptionCount = descriptor.VertexLayout != null ? 1u : 0u,
				pVertexBindingDescriptions = &binding,
				vertexAttributeDescriptionCount = (uint)attributes.Length,
				pVertexAttributeDescriptions = pAttributes,
			};

			VkGraphicsPipelineCreateInfo info = new()
			{
				pNext = &rendering,
				stageCount = 2,
				pStages = stages,
				pVertexInputState = &vertexInput,
				pInputAssemblyState = &inputAssembly,
				pViewportState = &viewportState,
				pRasterizationState = &raster,
				pMultisampleState = &multisample,
				pColorBlendState = &blend,
				pDynamicState = &dynamicState,
				layout = Layout,
				renderPass = VkRenderPass.Null // VK_KHR_dynamic_rendering is included in Vulkan core 1.3
			};

			VkPipeline pipeline;
			VkResult result = api.vkCreateGraphicsPipelines(VkPipelineCache.Null, 1, &info, &pipeline);
			if (result != VkResult.Success)
			{
				api.vkDestroyPipelineLayout(Layout);
				throw new VkException(result, "Failed to create pipeline");
			}

			Handle = pipeline;
		}
	}

	protected override void Dispose(bool disposing)
	{
		_device.DeviceApi.vkDestroyPipeline(Handle);
		_device.DeviceApi.vkDestroyPipelineLayout(Layout);
	}
}