using RawSalt;
using RawSalt.Graphics.Vulkan;
using RawSalt.Logging;
using RawSalt.Windowing.Win32;

namespace NekoRotation;

internal sealed class App : Application
{
    public static void Main()
    {
	    Log.AddSink(new ConsoleSink());

        App app = new();

        var config = LaunchConfiguration.CreateBuilder()
	        .AddWin32Platform()
	        .AddVulkanBackend()
	        .Build();

        app.Launch(config);
    }
}
