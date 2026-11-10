using RawSalt;
using RawSalt.Graphics.Vulkan;
using RawSalt.Logging;
using RawSalt.Platforms.Win32;

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

        bool success = app.Launch(config);

        Environment.Exit(success ? 0 : -1);
    }
}
