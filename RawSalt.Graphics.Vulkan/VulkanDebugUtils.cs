using System.Runtime.InteropServices;
using RawSalt.Logging;

namespace RawSalt.Graphics.Vulkan;

internal sealed unsafe class VulkanDebugUtils : IDisposable
{
	private readonly VkInstanceApi _instanceApi;
	private VkDebugUtilsMessengerEXT _messenger;
	private bool _disposed;

	private VulkanDebugUtils(VkInstanceApi instanceApi, VkDebugUtilsMessengerEXT messenger)
	{
		_instanceApi = instanceApi;
		_messenger = messenger;
	}

	public static VulkanDebugUtils? TryCreate(VkInstanceApi instanceApi)
	{
		VkDebugUtilsMessengerCreateInfoEXT createInfo = new()
		{
			messageSeverity = VkDebugUtilsMessageSeverityFlagsEXT.Verbose
			                  | VkDebugUtilsMessageSeverityFlagsEXT.Info
			                  | VkDebugUtilsMessageSeverityFlagsEXT.Warning
			                  | VkDebugUtilsMessageSeverityFlagsEXT.Error,
			messageType = VkDebugUtilsMessageTypeFlagsEXT.General
			              | VkDebugUtilsMessageTypeFlagsEXT.Validation
			              | VkDebugUtilsMessageTypeFlagsEXT.Performance,
			pfnUserCallback = &OnDebugUtilsMessage // ain't sure if UnmanagedCallersOnly preventing from GC
		};

		VkResult result = instanceApi.vkCreateDebugUtilsMessengerEXT(in createInfo, out VkDebugUtilsMessengerEXT messenger);
		if (result != VkResult.Success)
		{
			Log.Warning("Failed to create Vulkan debug utils messenger, reason: {0}", result);
			return null;
		}

		return new VulkanDebugUtils(instanceApi, messenger);
	}

	[UnmanagedCallersOnly]
	private static uint OnDebugUtilsMessage(VkDebugUtilsMessageSeverityFlagsEXT messageSeverity,
		VkDebugUtilsMessageTypeFlagsEXT messageTypes, VkDebugUtilsMessengerCallbackDataEXT* callbackData, void* userData)
	{
		try
		{
			if (callbackData == null)
			{
				return 0;
			}

			LogSeverity severity = (messageSeverity & VkDebugUtilsMessageSeverityFlagsEXT.Error) != 0
				? LogSeverity.Error
				: (messageSeverity & VkDebugUtilsMessageSeverityFlagsEXT.Warning) != 0
					? LogSeverity.Warning
					: (messageSeverity & VkDebugUtilsMessageSeverityFlagsEXT.Info) != 0
						? LogSeverity.Info
						: LogSeverity.Debug;

			string? messageIdName = Marshal.PtrToStringUTF8((nint)callbackData->pMessageIdName);
			string? message = Marshal.PtrToStringUTF8((nint)callbackData->pMessage);

			Log.Write(severity, null, null, "[Vulkan {0}] {1} ({2}): {3}",
				messageTypes,
				string.IsNullOrEmpty(messageIdName) ? "Vulkan" : messageIdName,
				callbackData->messageIdNumber,
				message ?? string.Empty);
		}
		catch (Exception ex)
		{
			try
			{
				Log.Error(ex, "Failed to process Vulkan debug utils message.");
			}
			catch
			{
				// Never let an exception cross the unmanaged callback boundary.
			}
		}

		return 0;
	}

	public void Dispose()
	{
		if (_disposed)
		{
			return;
		}

		_disposed = true;

		if (_messenger.IsNotNull)
		{
			_instanceApi.vkDestroyDebugUtilsMessengerEXT(_messenger);
			_messenger = VkDebugUtilsMessengerEXT.Null;
		}
	}
}
