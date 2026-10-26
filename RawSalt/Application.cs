using RawSalt.Logging;
using RawSalt.Platforms;

namespace RawSalt;

public class Application
{
	protected Application()
	{

	}

	protected void Launch(LaunchConfiguration config)
	{
		Log.Info("Launch application.");

		Platform platform = config.Platforms.First();

		foreach (Display display in platform.GetDisplays())
		{
			Log.Info(display.Name);
		}

		while (true)
		{

		}
	}
}