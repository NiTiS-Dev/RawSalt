using RawSalt;
using RawSalt.Logging;

namespace NekoRotation;

internal sealed class App : Application
{
    public static void Main()
    {
	    Log.AddSink(new ConsoleSink());

        App app = new();

        var config = LaunchConfiguration.CreateBuilder()
	        .Build();

        app.Launch(config);
    }
}
