using RawSalt.Graphics.Drawing;

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
}