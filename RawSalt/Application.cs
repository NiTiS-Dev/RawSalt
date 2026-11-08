using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using RawSalt.Diagnostics;
using RawSalt.Graphics;
using RawSalt.Graphics.Drawing;
using RawSalt.Graphics.Rendering;
using RawSalt.Logging;
using RawSalt.Platforms;

namespace RawSalt;

public class Application
{
	public bool Headless { get; }

	[MemberNotNullWhen(false, nameof(Headless))]
	[NotNullIfNotNull(nameof(MainView))]
	[NotNullIfNotNull(nameof(MainSurface))]
	[NotNullIfNotNull(nameof(GraphicsBackend))]
	public Platform? Platform { get; private set; }

	[MemberNotNullWhen(false, nameof(Headless))]
	[NotNullIfNotNull(nameof(Platform))]
	[NotNullIfNotNull(nameof(MainSurface))]
	[NotNullIfNotNull(nameof(GraphicsBackend))]
	public View? MainView { get; private set; }

	[MemberNotNullWhen(false, nameof(Headless))]
	[NotNullIfNotNull(nameof(Platform))]
	[NotNullIfNotNull(nameof(MainView))]
	[NotNullIfNotNull(nameof(GraphicsBackend))]
	public RenderSurface? MainSurface { get; private set; }

	[MemberNotNullWhen(false, nameof(Headless))]
	[NotNullIfNotNull(nameof(Platform))]
	[NotNullIfNotNull(nameof(MainView))]
	[NotNullIfNotNull(nameof(MainSurface))]
	public GraphicsBackend? GraphicsBackend { get; private set; }

	protected Application(bool headless = false)
	{
		Headless = headless;
	}

	protected virtual void Update(ulong updateId, float deltaTime)
	{ }

	protected bool Launch(LaunchConfiguration config)
	{
		if (Headless)
		{
			throw new NotImplementedException("Headless applications is not implemented yet.");
		}

		try
		{
			Log.Info("Launch application.");

			// TODO: Add headless launch configuration
			// TODO: allow user to configure force candidate (parse program cli arguments)
			Platform = SelectBestPlatform(config.Platforms.Select(t => t(this)).ToList());

			// TODO: config.GetConfiguration<ViewConfiguration>() ?? default;
			WindowConfiguration windowConfig = new()
			{
				Title = "Application",
				Position = null,
				Size = new(1280, 720)
			};
			MainView = Platform.CreateView(in windowConfig);

			GraphicsBackend = SelectBestGraphicsBackend(config.GraphicsBackends.Select(t => t(this)).ToList());
			MainSurface = GraphicsBackend.CreateSurface(MainView);

			FramePacer pacer = new();
			RenderThread renderThread = new(pacer, GraphicsBackend, MainSurface);
			renderThread.Start();

			AppLoop(pacer, renderThread, MainView);

			renderThread.Stop();
			Platform.ReleaseView(MainView);
			MainView = null!;
		}
		catch (Exception ex)
		{
			Log.Error(ex, "Error during application launch.");
			return false;
		}

		return true;
	}

	private void AppLoop(FramePacer pacer, RenderThread renderThread, View view)
	{
		long last = Stopwatch.GetTimestamp();

		RateStatistics statistics = new();
		statistics.Start();

		bool firstFrame = true;
		ulong updateId = 0;
		while (view.ShouldUpdate && renderThread.IsValid)
		{
			view.PumpEvents();
			if (firstFrame && renderThread.FirstFramePresented)
			{
				firstFrame = false;
				(view as Window)?.Show();
			}

			// At least update @10Hz
			RenderSnapshot? snapshot = pacer.AcquireForUpdate(TimeSpan.FromMilliseconds(100));

			long now = Stopwatch.GetTimestamp();
			float deltaTime = (now - last) / (float)Stopwatch.Frequency;
			Update(updateId++, deltaTime);
			statistics.Update();
			last = now;

			if (snapshot != null)
			{
				snapshot.Size = view.SurfaceSize;
				pacer.Submit(snapshot);
			}
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