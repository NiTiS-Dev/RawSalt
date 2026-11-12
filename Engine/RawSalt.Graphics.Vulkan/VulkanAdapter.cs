using System.Diagnostics;
using System.Runtime.InteropServices;
using RawSalt.Graphics.Devices;

namespace RawSalt.Graphics.Vulkan;

public sealed class VulkanAdapter : Adapter
{
	public override DeviceLimits Limits { get; }
	public VkPhysicalDevice PhysicalDevice { get; }
	public VkPhysicalDeviceProperties Properties { get; }
	public override string Name { get; }

	internal unsafe VulkanAdapter(VkPhysicalDevice physicalDevice, VkPhysicalDeviceProperties properties)
	{
		PhysicalDevice = physicalDevice;
		Properties = properties;

		Debug.Assert(properties.limits.minUniformBufferOffsetAlignment <= uint.MaxValue);
		Limits = new()
		{
			MaxImageDimension1D = properties.limits.maxImageDimension1D,
			MaxImageDimension2D = properties.limits.maxImageDimension2D,
			MaxImageDimension3D = properties.limits.maxImageDimension3D,
			MinUniformBufferOffsetAlignment = (uint)properties.limits.minUniformBufferOffsetAlignment,
		};
		Name = Marshal.PtrToStringUTF8((IntPtr)properties.deviceName) ?? "Unknown";
	}
}