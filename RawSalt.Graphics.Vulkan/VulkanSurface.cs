using RawSalt.Graphics.Rendering;
using RawSalt.Platforms;

namespace RawSalt.Graphics.Vulkan;

public sealed class VulkanSurface : RenderSurface
{
	public VkSurfaceKHR Surface { get; }

	internal VulkanSurface(View view, VkSurfaceKHR surface) : base(view)
	{
		Surface = surface;
	}
}