using RawSalt;
using RawSalt.Graphics.Vulkan;
using RawSalt.Logging;
#if RAWSALT_WIN32
using RawSalt.Windowing.Win32;
#elif RAWSALT_XLIB
using RawSalt.Windowing.Xlib;
#elif RAWSALT_WAYLAND
using RawSalt.Windowing.Wayland;
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
#elif RAWSALT_XLIB
	        .AddXlibPlatform()
#elif RAWSALT_WAYLAND
	        .AddWaylandPlatform()
#endif
	        .AddVulkanBackend()
	        .Build();

        bool success = app.Launch(config);

        Environment.Exit(success ? 0 : -1);
    }
}
