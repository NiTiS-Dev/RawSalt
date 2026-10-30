using System.Diagnostics;
using RawSalt.Logging;
using RawSalt.Platforms;

namespace RawSalt.Graphics.Vulkan;

public sealed partial class VulkanBackend : GraphicsBackend
{
	private static volatile int _instanceCount;

	private Platform Platform { get; }
	private VkInstanceApi? _instanceApi;

	public VulkanBackend(Application app) : base(app)
	{
		Platform = app.Platform;
		Debug.Assert(Platform != null);

		if (Interlocked.Increment(ref _instanceCount) == 1)
		{
			VkResult result = Vk.vkInitialize();

			if (result != VkResult.Success)
			{
				Log.Error("Failed to initialize library, reason: {0}", result);
			}
		}
	}

	protected override void Dispose(bool disposing)
	{
		Interlocked.Decrement(ref _instanceCount);

		if (disposing)
		{
			DestroyInstance();
		}
	}

	private VkInstanceApi InstanceApi
	{
		get
		{
			if (_instanceApi == null)
			{
				lock (_instanceLock)
				{
					_instanceApi ??= CreateInstanceApi();
				}
			}

			return _instanceApi;
		}
	}

	private unsafe VkInstanceApi CreateInstanceApi()
	{
		VkUtf8String applicationName = "RawSalt"u8;
		VkApplicationInfo applicationInfo = new()
		{
			pApplicationName = applicationName,
			applicationVersion = new VkVersion(1, 0, 0),
			pEngineName = applicationName,
			engineVersion = new VkVersion(1, 0, 0),
			apiVersion = VkVersion.Version_1_0
		};

		using VkStringArray extensions = GetRequiredExtensions(Platform.Api);
		VkInstanceCreateInfo createInfo = new()
		{
			pApplicationInfo = &applicationInfo,
			enabledExtensionCount = extensions.Length,
			ppEnabledExtensionNames = extensions
		};

		VkResult result = Vk.vkCreateInstance(in createInfo, null, out VkInstance instance);
		if (result != VkResult.Success)
		{
			throw new InvalidOperationException($"Failed to create Vulkan instance, reason: {result}.");
		}

		return Vk.GetApi(instance);
	}

	private void DestroyInstance()
	{
		lock (_instanceLock)
		{
			_instanceApi?.vkDestroyInstance();
			_instanceApi = null;
		}
	}

	public override GraphicsDevice CreateDevice(Adapter? device)
	{
		if (device is not null && device is not VulkanAdapter)
		{
			throw new ArgumentException("Provided adapter device is not of Vulkan.");
		}

		throw new NotImplementedException();
	}

	/// <remarks>
	/// Do not forget to dispose array safely.
	/// </remarks>
	private static VkStringArray GetRequiredExtensions(WellKnownPlatformApi platformApi)
	{
		VkUtf8String[] strings = platformApi switch
		{
			WellKnownPlatformApi.Win32
				=> [Vk.VK_KHR_SURFACE_EXTENSION_NAME, Vk.VK_KHR_WIN32_SURFACE_EXTENSION_NAME],
			WellKnownPlatformApi.Wayland
				=> [Vk.VK_KHR_SURFACE_EXTENSION_NAME, Vk.VK_KHR_WAYLAND_SURFACE_EXTENSION_NAME],
			WellKnownPlatformApi.Android
				=> [Vk.VK_KHR_SURFACE_EXTENSION_NAME, Vk.VK_KHR_ANDROID_SURFACE_EXTENSION_NAME],
			WellKnownPlatformApi.Xlib
				=> [Vk.VK_KHR_SURFACE_EXTENSION_NAME, Vk.VK_KHR_XLIB_SURFACE_EXTENSION_NAME],
			_
				=> [Vk.VK_KHR_SURFACE_EXTENSION_NAME]
		};

		return new VkStringArray(strings);
	}
}