using System.Runtime.InteropServices;

namespace RawSalt.Graphics.Vulkan;

public sealed class VulkanShaderModule : ShaderModule
{
	private readonly VulkanDevice _device;
	public VkShaderModule Module { get; }
	public override ShaderStages Stage { get; }

	public unsafe VulkanShaderModule(VulkanDevice device, ShaderStages stage, ReadOnlySpan<byte> code)
	{
		if (code.Length == 0 || code.Length % 4 != 0)
		{
			throw new ArgumentException("SPIR-V must be a non-empty multiple of 4 bytes.", nameof(code));
		}

		_device = device;
		Stage = stage;

		uint[] words = new uint[code.Length / 4]; // copy: pCode must be 4-byte aligned
		code.CopyTo(MemoryMarshal.AsBytes(words.AsSpan()));

		fixed (uint* p = words)
		{
			VkShaderModuleCreateInfo info = new()
			{
				codeSize = (nuint)code.Length,
				pCode = p
			};
			device.DeviceApi.vkCreateShaderModule(&info, out VkShaderModule module);
			Module = module;
		}
	}

	protected override void Dispose(bool disposing)
	{
		_device.DeviceApi.vkDestroyShaderModule(Module);
	}
}