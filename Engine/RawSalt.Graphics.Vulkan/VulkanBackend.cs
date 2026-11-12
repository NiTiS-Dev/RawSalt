using System.Diagnostics;
using RawSalt.Graphics.Rendering;
using RawSalt.Logging;
using RawSalt.Platforms;

namespace RawSalt.Graphics.Vulkan;

/// <summary>
/// Vulkan 1.3 API graphics backend.
/// </summary>
public sealed partial class VulkanBackend : GraphicsBackend
{
	private static volatile int _instanceCount;

	private Platform Platform { get; }
	private VkInstanceApi? _instanceApi;
	private VulkanDebugUtils? _debugUtils;

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
			apiVersion = VkVersion.Version_1_3
		};

		bool enableDebugLayers = AreDebugLayersEnabled();
		bool enableValidationLayer = enableDebugLayers && IsInstanceLayerSupported(Vk.VK_LAYER_KHRONOS_VALIDATION_EXTENSION_NAME);
		bool enableDebugUtils = enableDebugLayers && IsInstanceExtensionSupported(Vk.VK_EXT_DEBUG_UTILS_EXTENSION_NAME);

		using VkStringArray layers = GetRequiredLayers(enableValidationLayer);
		using VkStringArray extensions = GetRequiredExtensions(Platform.Api, enableDebugUtils);
		VkInstanceCreateInfo createInfo = new()
		{
			pApplicationInfo = &applicationInfo,
			enabledLayerCount = layers.Length,
			ppEnabledLayerNames = layers,
			enabledExtensionCount = extensions.Length,
			ppEnabledExtensionNames = extensions
		};

		VkResult result = Vk.vkCreateInstance(in createInfo, null, out VkInstance instance);
		if (result != VkResult.Success)
		{
			throw new InvalidOperationException($"Failed to create Vulkan instance, reason: {result}.");
		}

		VkInstanceApi instanceApi = Vk.GetApi(instance);

		if (enableDebugUtils)
		{
			try
			{
				_debugUtils = VulkanDebugUtils.TryCreate(instanceApi);
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Failed to initialize Vulkan debug utils.");
			}
		}

		return instanceApi;
	}

	private static bool AreDebugLayersEnabled()
	{
#if DEBUG
		return true;
#else
		string? value = Environment.GetEnvironmentVariable("RAWSALT_ENABLE_DEBUG_LAYERS_VULKAN");
		return !string.IsNullOrEmpty(value) && value.ToLowerInvariant() is not ("0" or "false");
#endif
	}

	private void DestroyInstance()
	{
		lock (_instanceLock)
		{
			_debugUtils?.Dispose();
			_debugUtils = null;

			_instanceApi?.vkDestroyInstance();
			_instanceApi = null;
		}
	}

	public override VulkanSurface CreateSurface(View view)
	{
		if (view is IWin32Window win32Window)
		{
			VkSurfaceKHR surface;
			VkWin32SurfaceCreateInfoKHR createInfo = new()
			{
				hwnd = win32Window.Handle,
				hinstance = win32Window.ModuleHandle
			};

			VkResult result = InstanceApi.vkCreateWin32SurfaceKHR(in createInfo, out surface);

			if (result != VkResult.Success)
			{
				Log.Error("Failed to create Vulkan surface for Win32 window, reason: {0}", result);
				throw new Exception("Failed to create Vulkan surface for Win32 window");
			}

			return new VulkanSurface(view, surface);
		}

		throw new NotImplementedException("VulkanBackend.CreateSurface only supports Win32 windows for now.");
	}


	/// <remarks>
	/// Do not forget to dispose array safely.
	/// </remarks>
	private static VkStringArray GetRequiredLayers(bool enableValidationLayer)
	{
		List<VkUtf8String> strings = [];

		if (enableValidationLayer)
		{
			strings.Add(Vk.VK_LAYER_KHRONOS_VALIDATION_EXTENSION_NAME);
		}

		return new VkStringArray(strings);
	}

	/// <remarks>
	/// Do not forget to dispose array safely.
	/// </remarks>
	private static VkStringArray GetRequiredExtensions(WellKnownPlatformApi platformApi, bool enableDebugUtils)
	{
		List<VkUtf8String> strings = platformApi switch
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

		if (enableDebugUtils)
		{
			strings.Add(Vk.VK_EXT_DEBUG_UTILS_EXTENSION_NAME);
		}

		return new VkStringArray(strings);
	}

	private static unsafe bool IsInstanceExtensionSupported(VkUtf8String extensionName)
	{
		VkResult result = Vk.vkEnumerateInstanceExtensionProperties(out uint extensionCount);
		if (result != VkResult.Success && result != VkResult.Incomplete)
		{
			Log.Warning("Failed to enumerate Vulkan instance extensions, reason: {0}", result);
			return false;
		}

		if (extensionCount == 0)
		{
			return false;
		}

		VkExtensionProperties[] extensions = new VkExtensionProperties[extensionCount];
		result = Vk.vkEnumerateInstanceExtensionProperties(extensions);
		if (result != VkResult.Success && result != VkResult.Incomplete)
		{
			Log.Warning("Failed to retrieve Vulkan instance extensions, reason: {0}", result);
			return false;
		}

		return ContainsExtension(extensions, extensionName);
	}

	private static unsafe bool ContainsExtension(ReadOnlySpan<VkExtensionProperties> extensions, VkUtf8String extensionName)
	{
		foreach (VkExtensionProperties properties in extensions)
		{
			byte* name = properties.extensionName;
			if (new VkUtf8String(name).Span.SequenceEqual(extensionName.Span))
			{
				return true;
			}
		}

		return false;
	}

	private static unsafe bool IsInstanceLayerSupported(VkUtf8String layerName)
	{
		VkResult result = Vk.vkEnumerateInstanceLayerProperties(out uint layerCount);
		if (result != VkResult.Success && result != VkResult.Incomplete)
		{
			Log.Warning("Failed to enumerate Vulkan instance layers, reason: {0}", result);
			return false;
		}

		if (layerCount == 0)
		{
			return false;
		}

		VkLayerProperties[] layers = new VkLayerProperties[layerCount];
		result = Vk.vkEnumerateInstanceLayerProperties(layers);
		if (result != VkResult.Success && result != VkResult.Incomplete)
		{
			Log.Warning("Failed to retrieve Vulkan instance layers, reason: {0}", result);
			return false;
		}

		return ContainsLayer(layers, layerName);
	}

	private static unsafe bool ContainsLayer(ReadOnlySpan<VkLayerProperties> layers, VkUtf8String layerName)
	{
		foreach (VkLayerProperties properties in layers)
		{
			byte* name = properties.layerName;
			if (new VkUtf8String(name).Span.SequenceEqual(layerName.Span))
			{
				return true;
			}
		}

		return false;
	}
}