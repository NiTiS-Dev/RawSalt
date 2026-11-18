using RawSalt.Graphics.Rendering;
using RawSalt.Platforms;

namespace RawSalt.Graphics.Vulkan;

public sealed class VulkanSurface : RenderSurface, IDisposable
{
	public VkSurfaceKHR Surface { get; private set; }

	private readonly VkInstanceApi _instanceApi;
	private bool _disposed;

	internal VulkanSurface(View view, VkInstanceApi instanceApi, VkSurfaceKHR surface) : base(view)
	{
		_instanceApi = instanceApi;
		Surface = surface;
	}

	public void Dispose()
	{
		if (_disposed)
		{
			return;
		}

		_disposed = true;

		if (Surface != VkSurfaceKHR.Null)
		{
			_instanceApi.vkDestroySurfaceKHR(Surface);
			Surface = VkSurfaceKHR.Null;
		}
	}
}
