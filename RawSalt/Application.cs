using System.Collections.Immutable;
using RawSalt.Logging;
using RawSalt.Platforms;

namespace RawSalt;

public class Application
{
	public Platform Platform { get; private set; }

	protected Application()
	{

	}

	protected void Launch(LaunchConfiguration config)
	{
		try
		{
			Log.Info("Launch application.");

			Platform = SelectBestPlatform(config.Platforms);

			WindowConfiguration windowConfig = new()
			{
				Title = "Application",
				Position = null,
				Size = new(1280, 720)
			};
			View view = Platform.CreateView(in windowConfig);
			AppLoop(view);
		}
		catch (Exception ex)
		{
			Log.Error(ex, "Error during application launch.");
		}
	}

	private void AppLoop(View view)
	{
		while (true)
		{
			view.PumpEvents();
		}
	}

	private static Platform SelectBestPlatform(params ImmutableArray<Platform> platforms)
	{
		if (platforms.Length == 0)
		{
			throw new ArgumentException("No platform candidates provided.", nameof(platforms));
		}

		if (platforms.IsEmpty)
		{
			return platforms[0];
		}

		// Prefer wayland over Xlib
		Platform? best;
		if (OperatingSystem.IsLinux() &&
		    (best = platforms.FirstOrDefault(t => t.Api == WellKnownPlatformApi.Wayland)) != null)
		{
			return best;
		}

		return platforms[0];
	}
}