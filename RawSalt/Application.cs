using System.Collections.Immutable;
using RawSalt.Graphics;
using RawSalt.Logging;
using RawSalt.Platforms;

namespace RawSalt;

public class Application
{
	public Platform Platform { get; private set; }
	public GraphicsBackend GraphicsBackend { get; private set; }

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

			GraphicsBackend = SelectBestGraphicsBackend(config.GraphicsBackends);
			AppLoop(view);
		}
		catch (Exception ex)
		{
			Log.Error(ex, "Error during application launch.");
		}
	}

	private void AppLoop(View view)
	{
		while (view.ShouldUpdate)
		{
			view.PumpEvents();
		}
	}

	private static GraphicsBackend SelectBestGraphicsBackend(params ImmutableArray<GraphicsBackend> backends)
	{
		if (backends.Length == 0)
		{
			throw new ArgumentException("No graphics backend candidates provided.", nameof(backends));
		}

		return backends[0];
	}

	private static Platform SelectBestPlatform(params ImmutableArray<Platform> platforms)
	{
		switch (platforms.Length)
		{
			case 0:
				throw new ArgumentException("No platform candidates provided.", nameof(platforms));
			case 1:
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