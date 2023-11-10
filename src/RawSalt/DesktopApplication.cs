using RawSalt.Mathematics.Geometry;
using RawSalt.Memory;
using RawSalt.Native.SDL;
using RawSalt.Native.Vulkan;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using WindowFlags = RawSalt.Native.SDL.SDL2.SDL_WindowFlags;

namespace RawSalt;

public class DesktopApplication : Application
{
	public DesktopApplication(string applicationId) : base(applicationId)
	{
		title = applicationId;
		isResizable = true;
		size = new(512, 512);
	}

	#region Unique Properties

	private nint windowHandle;
	public nint WindowHandle => windowHandle;

	protected Instance vkInstance;
	protected Surface vkSurface;

	private string title;
	public string Title
	{
		get
		{
			if (windowHandle == nint.Zero)
				return title;

			return SDL2.SDL_GetWindowTitle(windowHandle);
		}
		set
		{
			title = value;
			
			if (windowHandle != nint.Zero)
				SDL2.SDL_SetWindowTitle(windowHandle, value);
		}
	}

	public bool FullScreen => throw null!;

	#endregion

	#region Properties

	private UVec2 size;
	public override UVec2 Size
	{
		get
		{
			if (windowHandle != nint.Zero)
			{
				SDL2.SDL_GetWindowSize(windowHandle, out int w, out int h);
				size = new((uint)w, (uint)h);
			}

			return size;
		}
		set
		{
			if (!IsResizeAllow)
				return;

			size = value;

			if (windowHandle != nint.Zero)
			{
				SDL2.SDL_SetWindowSize(windowHandle, (int)size.x, (int)size.y);
			}
		}
	}

	public override bool IsResizeAllow => true;

	private bool isResizable;
	public override bool IsResizable
	{
		get
		{
			return ((SDL2.SDL_WindowFlags)SDL2.SDL_GetWindowFlags(WindowHandle))
				.HasFlag(SDL2.SDL_WindowFlags.SDL_WINDOW_RESIZABLE);
		}
		set
		{
			isResizable = value;

			if (windowHandle != nint.Zero)
				SDL2.SDL_SetWindowResizable(windowHandle, isResizable ? SDL2.SDL_bool.SDL_TRUE : SDL2.SDL_bool.SDL_FALSE);
		}
	}

	#endregion

	public override void Run()
	{
		ulong tickCount = 0;
		while (true)
		{
			tickCount++;
			SDL2.SDL_PollEvent(out SDL2.SDL_Event evnt);

			switch (evnt.type)
			{
				case SDL2.SDL_EventType.SDL_QUIT:
					goto QUIT;
				case SDL2.SDL_EventType.SDL_WINDOWEVENT:
					switch (evnt.window.windowEvent)
					{
						case SDL2.SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED:
							break;
					}
					break;
			}
		}

	QUIT:
		logger.Info($"Application '{Id}' termination");
		logger.Info($"Tick count: {tickCount}");
	}

	public override unsafe void Initialize()
	{
		if (SDL2.SDL_Init(SDL2.SDL_WasInit(SDL2.SDL_INIT_VIDEO | SDL2.SDL_INIT_EVENTS)) != 0)
		{
			logger.Fatal("Unable to initialize SDL_INIT_VIDEO and/or SDL_INIT_AUDIO");
		}

		WindowFlags flags = WindowFlags.SDL_WINDOW_VULKAN;

		if (isResizable)
			flags |= WindowFlags.SDL_WINDOW_RESIZABLE;

		windowHandle = SDL2.SDL_CreateWindow(Title,
				SDL2.SDL_WINDOWPOS_CENTERED, SDL2.SDL_WINDOWPOS_CENTERED,
				(int)size.x, (int)size.y,
				flags
				);

		InitializeVkInstance();
		InitializeVkSurface();
	}
	private unsafe void InitializeVkInstance()
	{
		nint pEngineName = 0;
		nint pApplicationName = 0;

		try
		{
			pEngineName = AllocatorHelper.AllocString("RawSaltEngine"u8);
			pApplicationName = AllocatorHelper.AllocString(Id);

			ApplicationInfo vkApplication = new()
			{
				StructureType = StructureType.ApplicationInfo,
				ApplicationName = pApplicationName,
				ApplicationVersion = new Native.Vulkan.Version(1, 0, 0),
				EngineName = pEngineName,
				EngineVersion = new Native.Vulkan.Version(0, 0, 1),
				ApiVersion = new Native.Vulkan.Version(1, 0, 0),
			};

			SDL2.SDL_Vulkan_GetInstanceExtensions(windowHandle, out uint vulkanExtCount, nint.Zero);

			Span<nint> vkExts = stackalloc nint[(int)vulkanExtCount]; // C#, WTF, why i need to cast unsigned int to signed in allocator????!?!///1/

			fixed (nint* vkExtsPtr = vkExts)
			{
				SDL2.SDL_Vulkan_GetInstanceExtensions(windowHandle, out vulkanExtCount, (nint)vkExtsPtr);

				InstanceCreateInfo instanceCreateInfo = new()
				{
					StructureType = StructureType.InstanceCreateInfo,
					ApplicationInfo = (nint)(&vkApplication),
					EnabledExtensionCount = vulkanExtCount,
					EnabledExtensionNames = (nint)vkExtsPtr,
					EnabledLayerCount = 0,
				};

				vkInstance = Vulkan.CreateInstance(ref instanceCreateInfo);
			}

			#region DEBUG INFO
			StringBuilder sb = new();
			string msg = "Required vulkan extension for window view creation:";
			sb.AppendLine(msg);
			for (int i = 0; i < vulkanExtCount; i++)
			{
				int zeroIndex = CStringHelper.ZeroIndex((byte*)vkExts[i]);
				string vulkanExt = Encoding.UTF8.GetString((byte*)vkExts[i], zeroIndex);
				sb.Append('\t');
				sb.AppendLine(vulkanExt);
			}
			logger.Info(sb.ToString());
			#endregion
		}
		finally
		{
			AllocatorHelper.FreeString(pEngineName);
			AllocatorHelper.FreeString(pApplicationName);
		}

		logger.Info($"VkInstance created: {Unsafe.As<Instance, nint>(ref vkInstance):X16}");
	}
	private unsafe void InitializeVkSurface()
	{
		SDL2.SDL_Vulkan_CreateSurface(windowHandle, vkInstance.NativeHandle, out ulong surfaceHandle);

		vkSurface = new(surfaceHandle);
	}
}
