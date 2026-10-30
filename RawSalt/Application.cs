using System.Collections.Immutable;
using RawSalt.Graphics;
using RawSalt.Graphics.Rendering;
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

	protected bool Launch(LaunchConfiguration config)
	{
		try
		{
			Log.Info("Launch application.");

			// TODO: allow user to configure force candidate (parse program cli arguments)
			Platform = SelectBestPlatform(config.Platforms.Select(t => t(this)).ToList());

			// TODO: config.GetConfiguration<ViewConfiguration>() ?? default;
			WindowConfiguration windowConfig = new()
			{
				Title = "Application",
				Position = null,
				Size = new(1280, 720)
			};
			View view = Platform.CreateView(in windowConfig);

			GraphicsBackend = SelectBestGraphicsBackend(config.GraphicsBackends.Select(t => t(this)).ToList());

			var adapters = GraphicsBackend.GetAdapters();
			if (adapters is not null && adapters.Count == 0)
			{
				// TODO: Add documentation how to fix that error on laptops with switchable graphics.
				Log.Error("Failed to get any viable adapter.");
			}

			RenderThread renderThread = new(GraphicsBackend, view);
			renderThread.Start();

			AppLoop(renderThread, view);

			renderThread.Stop();
			Platform.ReleaseView(view);
		}
		catch (Exception ex)
		{
			Log.Error(ex, "Error during application launch.");
			return false;
		}

		return true;
	}

	private void AppLoop(RenderThread renderThread, View view)
	{
		while (view.ShouldUpdate)
		{
			view.PumpEvents();
		}
	}

	private static GraphicsBackend SelectBestGraphicsBackend(params IList<GraphicsBackend> backends)
	{
		if (backends.Count == 0)
		{
			throw new ArgumentException("No graphics backend candidates provided.", nameof(backends));
		}

		return backends[0];
	}

	private static Platform SelectBestPlatform(params IList<Platform> platforms)
	{
		switch (platforms.Count)
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