using NiTiS.Mathematics;
using RawSalt.Graphics.Rendering;

namespace RawSalt.Graphics.Vulkan.Rendering;

internal sealed class VulkanRenderTarget : RenderTarget
{
	public VkImage Image;
	public VkImageView View;
	public VkExtent2D Extent;
	public VkImageLayout Layout;
	public VkFormat Format;

	public VkImage DepthImage;
	public VkImageView DepthView;
	public VkImageLayout DepthLayout;

	public override Vector2<int> Size => new((int)Extent.width, (int)Extent.height);
}