using RawSalt;
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
	        .Build();

        app.Launch(config);
    }
}
