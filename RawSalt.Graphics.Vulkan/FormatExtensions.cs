using RawSalt.Graphics.Drawing;
using RawSalt.Graphics.Pipelines;
using RawSalt.Graphics.Rendering;

namespace RawSalt.Graphics.Vulkan;

public static class FormatExtensions
{
	extension(VkFormat format)
	{
		public PixelFormat ToPixelFormat()
		{
			return format switch
			{
				VkFormat.R8G8B8A8Unorm => PixelFormat.Rgba32UNorm,
				VkFormat.R8G8B8A8Srgb => PixelFormat.Rgba32Srgb,
				VkFormat.B8G8R8A8Unorm => PixelFormat.Bgra32UNorm,
				VkFormat.B8G8R8A8Srgb => PixelFormat.Bgra32Srgb,
				_ => PixelFormat.Unknown,
			};
		}
	}

	extension(PixelFormat format)
	{
		public VkFormat ToVkFormat()
		{
			return format switch
			{
				PixelFormat.Rgba32UNorm => VkFormat.R8G8B8A8Unorm,
				PixelFormat.Rgba32Srgb => VkFormat.R8G8B8A8Srgb,
				PixelFormat.Bgra32UNorm => VkFormat.B8G8R8A8Unorm,
				PixelFormat.Bgra32Srgb => VkFormat.B8G8R8A8Srgb,
				_ => VkFormat.Undefined,
			};
		}
	}

	extension(VertexFormat format)
	{
		public VkFormat ToVkFormat()
		{
			return format switch
			{
				VertexFormat.Float32 => VkFormat.R32Sfloat,
				VertexFormat.Float32x2 => VkFormat.R32G32Sfloat,
				VertexFormat.Float32x3 => VkFormat.R32G32B32Sfloat,
				VertexFormat.Float32x4 => VkFormat.R32G32B32A32Sfloat,
				_ => throw new NotSupportedException($"Vertex format {format} not supported."),
			};
		}
	}

	extension(ShaderStages stages)
	{
		public VkShaderStageFlags ToVkFlags()
		{
			VkShaderStageFlags result = 0;

			if (stages.HasFlag(ShaderStages.Vertex)) result |= VkShaderStageFlags.Vertex;
			if (stages.HasFlag(ShaderStages.Fragment)) result |= VkShaderStageFlags.Fragment;

			return result;
		}

		public VkShaderStageFlags ToVk()
		{
			return stages switch
			{
				ShaderStages.Vertex => VkShaderStageFlags.Vertex,
				_ => VkShaderStageFlags.Fragment
			};
		}
	}

	extension(BindingType bindingType)
	{
		public VkDescriptorType ToVk()
		{
			return bindingType switch
			{
				BindingType.CombinedImageSampler => VkDescriptorType.CombinedImageSampler,
				_ => VkDescriptorType.UniformBufferDynamic,
			};
		}
	}

	extension(Sampling sampling)
	{
		public VkSamplerAddressMode ToVk()
		{
			return sampling switch
			{
				Sampling.ClampToBorder => VkSamplerAddressMode.ClampToBorder,
				Sampling.MirrorRepeat => VkSamplerAddressMode.MirroredRepeat,
				_ => VkSamplerAddressMode.Repeat,
			};
		}
	}
}