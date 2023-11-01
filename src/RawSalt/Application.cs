using RawSalt.Graphics;
using RawSalt.IO.Resources;
using RawSalt.Native.Vulkan;
using RawSalt.Native.Windows;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using VkVersion = RawSalt.Native.Vulkan.Version;

namespace RawSalt;

public abstract unsafe partial class Application : IDisposable
{
	protected ResourceManager resources;
	public readonly RenderOptions RenderOptions;

	private Instance instance;
	private PhysicalDevice physicalDevice;
	private Queue queue;
	private Device device;

	private Surface surface;

	protected Application(RenderOptions renderOptions)
	{
		RenderOptions = renderOptions;

		InitializeResourceManager();
		InitializeVulkan();
	}
	public virtual void Dispose() { }
	public virtual void Run()
	{
	}

	/// <summary>
	/// Initialize
	/// </summary>
	protected virtual void InitializeResourceManager()
	{
		resources = new LocalResourceManager();
	}
	protected virtual void InitializeVulkan()
	{
		CreateApplication();
		CreateSurface();
		CreateDevice();
	}
	private void CreateApplication()
	{
		var applicationInfo = new ApplicationInfo
		{
			StructureType = StructureType.ApplicationInfo,
			EngineVersion = 0,
			ApiVersion = Vulkan.ApiVersion
		};

		var enabledLayerNames = new[]
		{
			Marshal.StringToHGlobalAnsi("VK_LAYER_LUNARG_standard_validation"),
		};

		var enabledExtensionNames = new[]
		{
			Marshal.StringToHGlobalAnsi("VK_KHR_surface"),
			Marshal.StringToHGlobalAnsi("VK_KHR_win32_surface"),
			Marshal.StringToHGlobalAnsi("VK_EXT_debug_report"),
		};


		try
		{
			fixed (void* enabledLayerNamesPointer = &enabledLayerNames[0])
			fixed (void* enabledExtensionNamesPointer = &enabledExtensionNames[0])
			{
				var instanceCreateInfo = new InstanceCreateInfo
				{
					StructureType = StructureType.InstanceCreateInfo,
					ApplicationInfo = new IntPtr(&applicationInfo),
					EnabledExtensionCount = (uint)enabledExtensionNames.Length,
					EnabledExtensionNames = new IntPtr(enabledExtensionNamesPointer),
				};

				//if (validate)
				//{
				//instanceCreateInfo.EnabledLayerCount = (uint)enabledLayerNames.Length;
				//instanceCreateInfo.EnabledLayerNames = new IntPtr(enabledLayerNamesPointer);
				//}

				instance = Vulkan.CreateInstance(ref instanceCreateInfo);
			}
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine(ex);
		}
		finally
		{
			foreach (var enabledExtensionName in enabledExtensionNames)
				Marshal.FreeHGlobal(enabledExtensionName);

			foreach (var enabledLayerName in enabledLayerNames)
				Marshal.FreeHGlobal(enabledLayerName);
		}

		physicalDevice = instance.PhysicalDevices[0];
	}
	protected virtual void CreateSurface()
	{
		var surfaceCreateInfo = new Win32SurfaceCreateInfo
		{
			StructureType = StructureType.Win32SurfaceCreateInfo,
			InstanceHandle = Process.GetCurrentProcess().Handle,
			WindowHandle = Kernel32.GetConsoleWindow().Handle,
		};
		surface = instance.CreateWin32Surface(surfaceCreateInfo);
	}
	private void CreateDevice()
	{
		uint queuePriorities = 0;
		var queueCreateInfo = new DeviceQueueCreateInfo
		{
			StructureType = StructureType.DeviceQueueCreateInfo,
			QueueFamilyIndex = 0,
			QueueCount = 1,
			QueuePriorities = new IntPtr(&queuePriorities)
		};

		var enabledLayerNames = new[]
		{
				Marshal.StringToHGlobalAnsi("VK_LAYER_LUNARG_standard_validation"),
			};

		var enabledExtensionNames = new[]
		{
				Marshal.StringToHGlobalAnsi("VK_KHR_swapchain"),
			};

		try
		{
			fixed (void* enabledLayerNamesPointer = &enabledLayerNames[0])
			fixed (void* enabledExtensionNamesPointer = &enabledExtensionNames[0])
			{
				var enabledFeatures = new PhysicalDeviceFeatures
				{
					ShaderClipDistance = true,
				};

				var deviceCreateInfo = new DeviceCreateInfo
				{
					StructureType = StructureType.DeviceCreateInfo,
					QueueCreateInfoCount = 1,
					QueueCreateInfos = new IntPtr(&queueCreateInfo),
					EnabledExtensionCount = (uint)enabledExtensionNames.Length,
					EnabledExtensionNames = new IntPtr(enabledExtensionNamesPointer),
					EnabledFeatures = new IntPtr(&enabledFeatures)
				};

				//if (validate)
				//{
				//	deviceCreateInfo.EnabledLayerCount = (uint)enabledLayerNames.Length;
				//	deviceCreateInfo.EnabledLayerNames = new IntPtr(enabledLayerNamesPointer);
				//}

				device = physicalDevice.CreateDevice(ref deviceCreateInfo);
			}
		}
		finally
		{
			foreach (var enabledExtensionName in enabledExtensionNames)
				Marshal.FreeHGlobal(enabledExtensionName);

			foreach (var enabledLayerName in enabledLayerNames)
				Marshal.FreeHGlobal(enabledLayerName);
		}

		int queueNodeIndex = physicalDevice.QueueFamilyProperties
				.Where((properties, index) => (properties.QueueFlags & QueueFlags.Graphics) != 0 && physicalDevice.GetSurfaceSupport((uint)index, surface))
				.Select((properties, index) => index).First();

		queue = device.GetQueue(0, (uint)queueNodeIndex);
	}

}