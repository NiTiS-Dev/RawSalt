using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.InteropServices;
using RawSalt.Graphics.Drawing;
using RawSalt.Graphics.Pipelines;
using RawSalt.Graphics.Rendering;
using RawSalt.Graphics.Vulkan.Rendering;

namespace RawSalt.Graphics.Vulkan;

public sealed class VulkanPipeline : Pipeline
{
	private readonly VulkanDevice _device;
	private readonly Dictionary<string, ShaderResource> _resources;

	internal VulkanBindGroupLayout[] BindGroupLayouts { get; }
	public VkPipelineLayout Layout { get; }

	public VkPipeline Handle { get; }
	public PixelFormat ColorFormat { get; }

	public unsafe VulkanPipeline(VulkanDevice device, PipelineDescriptor descriptor)
	{
		_device = device;
		ColorFormat = descriptor.ColorFormat;
		VkDeviceApi api = device.DeviceApi;

		CompiledShader shader = descriptor.Anatomy.Compiler.Compile(descriptor.Anatomy);
		_resources = shader.Resources.ToDictionary(static r => r.Name);

		BindGroupLayouts = CreateBindGroupLayouts(device, shader.Resources);

		VkPipelineLayoutCreateInfo layoutInfo = new();
		VkDescriptorSetLayout* pDescriptorSetLayouts = stackalloc VkDescriptorSetLayout[BindGroupLayouts.Length];
		for (int i = 0; i < BindGroupLayouts.Length; i++)
		{
			pDescriptorSetLayouts[i] = BindGroupLayouts[i].Handle;
		}
		layoutInfo.setLayoutCount = (uint)BindGroupLayouts.Length;
		layoutInfo.pSetLayouts = pDescriptorSetLayouts;
		api.vkCreatePipelineLayout(&layoutInfo, out VkPipelineLayout layout).CheckResult();
		Layout = layout;

		VkVertexInputAttributeDescription[] attributes = shader.VertexInputs.Select(static a =>
			new VkVertexInputAttributeDescription
			{
				location = a.Location,
				binding = 0,
				format = a.Format.ToVkFormat(),
				offset = a.Offset,
			}).ToArray();

		VkVertexInputBindingDescription binding = new()
		{
			binding = 0,
			stride = shader.VertexStride,
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
			VkPipelineShaderStageCreateInfo* stages = stackalloc VkPipelineShaderStageCreateInfo[shader.Stages.Length];
			VkShaderModule* modules = stackalloc VkShaderModule[shader.Stages.Length];
			for (int i = 0; i < shader.Stages.Length; i++)
			{
				modules[i] = CreateShaderModule(api, shader.Stages[i].Code.Span);
				stages[i] = new()
				{
					stage = shader.Stages[i].Stage.ToVk(),
					module = modules[i],
					pName = pEntryName
				};
			}

			try
			{
				VkPipelineVertexInputStateCreateInfo vertexInput = new()
				{
					vertexBindingDescriptionCount = attributes.Length > 0 ? 1u : 0u,
					pVertexBindingDescriptions = &binding,
					vertexAttributeDescriptionCount = (uint)attributes.Length,
					pVertexAttributeDescriptions = pAttributes,
				};

				VkGraphicsPipelineCreateInfo info = new()
				{
					pNext = &rendering,
					stageCount = (uint)shader.Stages.Length,
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
					DisposeLayouts();
					throw new VkException(result, "Failed to create pipeline");
				}

				Handle = pipeline;
			}
			finally
			{
				for (int i = 0; i < shader.Stages.Length; i++)
				{
					api.vkDestroyShaderModule(modules[i]);
				}
			}
		}
	}

	internal ShaderResource GetResource(string name, BindingType expected)
	{
		if (!_resources.TryGetValue(name, out ShaderResource resource))
		{
			throw new ArgumentException($"The pipeline has no shader resource named '{name}'.", nameof(name));
		}

		if (resource.Type != expected)
		{
			throw new ArgumentException($"Shader resource '{name}' is {resource.Type}, not {expected}.", nameof(name));
		}

		return resource;
	}

	protected override void Dispose(bool disposing)
	{
		_device.DeviceApi.vkDestroyPipeline(Handle);
		_device.DeviceApi.vkDestroyPipelineLayout(Layout);
		DisposeLayouts();
	}

	private void DisposeLayouts()
	{
		foreach (VulkanBindGroupLayout layout in BindGroupLayouts)
		{
			layout.Dispose();
		}
	}

	private static VulkanBindGroupLayout[] CreateBindGroupLayouts(VulkanDevice device,
		ImmutableArray<ShaderResource> resources)
	{
		if (resources.IsDefaultOrEmpty)
		{
			return [];
		}

		// Descriptor sets must be continuous, so empty sets in between get an empty layout.
		uint setCount = resources.Max(static r => r.Set) + 1;
		VulkanBindGroupLayout[] layouts = new VulkanBindGroupLayout[setCount];
		IGrouping<uint, ShaderResource>[] groups = resources.GroupBy(static r => r.Set).ToArray();
		for (uint set = 0; set < setCount; set++)
		{
			ReadOnlySpan<ShaderResource> setResources = groups.FirstOrDefault(g => g.Key == set)?.ToArray() ?? [];
			layouts[set] = new VulkanBindGroupLayout(device, setResources);
		}

		return layouts;
	}

	private static unsafe VkShaderModule CreateShaderModule(VkDeviceApi api, ReadOnlySpan<byte> code)
	{
		Debug.Assert(code.Length > 0 && code.Length % 4 == 0, "SPIR-V must be a non-empty multiple of 4 bytes.");

		fixed (byte* pCode = code)
		{
			VkShaderModuleCreateInfo info = new()
			{
				codeSize = (nuint)code.Length,
				pCode = (uint*)pCode
			};
			api.vkCreateShaderModule(&info, out VkShaderModule module).CheckResult();
			return module;
		}
	}
}
