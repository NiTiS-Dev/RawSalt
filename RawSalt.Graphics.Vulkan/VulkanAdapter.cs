using System.Runtime.InteropServices;
using System.Text;

namespace RawSalt.Graphics.Vulkan;

public sealed class VulkanAdapter : Adapter
{
	public VkPhysicalDevice PhysicalDevice { get; }
	public VkPhysicalDeviceProperties Properties { get; }
	public override string Name { get; }

	internal unsafe VulkanAdapter(VkPhysicalDevice physicalDevice, VkPhysicalDeviceProperties properties)
	{
		PhysicalDevice = physicalDevice;
		Properties = properties;
		Name = Marshal.PtrToStringUTF8((IntPtr)properties.deviceName) ?? "Unknown";
	}
}