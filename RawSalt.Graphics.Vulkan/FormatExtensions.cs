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
}