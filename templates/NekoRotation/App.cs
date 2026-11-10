using RawSalt;
using RawSalt.Graphics.Vulkan;
using RawSalt.Logging;
#if RAWSALT_WIN32
using RawSalt.Windowing.Win32;
#endif
#if RAWSALT_WAYLAND
using RawSalt.Windowing.Wayland;
#endif
#if RAWSALT_XLIB
using RawSalt.Windowing.Xlib;
#endif

namespace NekoRotation;

internal sealed class App : Application
{
    public static void Main()
    {
	    Log.AddSink(new ConsoleSink());

        App app = new();

        var config = LaunchConfiguration.CreateBuilder()
#if RAWSALT_WIN32
	        .AddWin32Platform()
#endif
#if RAWSALT_WAYLAND
	        .AddWaylandPlatform()
#endif
#if RAWSALT_XLIB
	        .AddXlibPlatform()
#endif
	        .AddVulkanBackend()
	        .Build();

        bool success = app.Launch(config);

        Environment.Exit(success ? 0 : -1);
    }
}
