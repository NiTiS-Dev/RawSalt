using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using RawSalt.Diagnostics;
using RawSalt.Graphics;
using RawSalt.Graphics.Rendering;
using RawSalt.Input;
using RawSalt.Logging;
using RawSalt.Platforms;
using RawSalt.Threading;

namespace RawSalt;

public class Application
{
	/// <summary>
	/// Describe how application handles graphics.<br/>
	/// When returns <see langword="true"/> application does not use any graphics.
	/// </summary>
	public bool Headless { get; }

	[MemberNotNullWhen(false, nameof(Headless))]
	public Platform? Platform { get; private set; }

	[MemberNotNullWhen(false, nameof(Headless))]
	internal View? MainView { get; private set; }

	[MemberNotNullWhen(false, nameof(Headless))]
	internal RenderSurface? MainSurface { get; private set; }

	[MemberNotNullWhen(false, nameof(Headless))]
	internal GraphicsBackend? GraphicsBackend { get; private set; }

	[MemberNotNullWhen(false, nameof(Headless))]
	private RenderThread? RenderThread { get; set; }

	/// <summary>
	/// The graphics device, available once the render thread has created it. Usable from the
	/// update/draw loop to create graphics objects, which are uploaded by the render thread later.
	/// </summary>
	/// <remarks>
	/// Can be null, if application is not include graphics.
	/// </remarks>
	[ThreadSafety(AllowedThread.Main)]
	[MemberNotNullWhen(false, nameof(Headless))]
	protected GraphicsDevice? Device => RenderThread?.Device;

	protected Application(bool headless = false)
	{
		Headless = headless;
	}

	/// <summary>
	/// Logical update event.
	/// <br/><br/>
	/// The update happens approximately every frame or more frequently, when rendering is too slow.
	/// Headless application takes a fixed update.
	/// </summary>
	/// <param name="updateId">Incremental index of update.</param>
	/// <param name="deltaTime">Time since previous update in seconds.</param>
	/// <remarks>
	/// Do no call manually.
	/// </remarks>
	[ThreadSafety(AllowedThread.Main)]
	protected virtual void Update(ulong updateId, float deltaTime)
	{ }

	[ThreadSafety(AllowedThread.Main)]
	protected virtual void Draw(ref CameraData camera, Vec2i viewport, ulong updateId, float deltaTime)
	{ }

	/// <summary>
	/// Called on the render thread once the device and renderer exist. Create render content here.
	/// </summary>
	[ThreadSafety(AllowedThread.Render)]
	protected internal virtual void LoadGraphics(GraphicsDevice device, Renderer renderer)
	{ }

	/// <summary>
	/// Called on the render thread to record one frame, after it was begun and before it is submitted.
	/// </summary>
	[ThreadSafety(AllowedThread.Render)]
	protected internal virtual void Render(GraphicsDevice device, RenderFrame frame, in CameraData camera)
	{ }

	/// <summary>
	/// Called on the render thread before the device is disposed. Release the content created in
	/// <see cref="LoadGraphics"/> here.
	/// </summary>
	[ThreadSafety(AllowedThread.Render)]
	protected internal virtual void UnloadGraphics()
	{ }

	/// <summary>
	/// Initialize and starts application lifetime cycle.
	/// </summary>
	/// <param name="config">Application configuration.</param>
	/// <returns>Exit state: <see langword="true"/> when successfully exit; <see langword="false"/> when crashed.</returns>
	protected bool Launch(LaunchConfiguration config)
	{
		// TODO(high): Move platform and backend to the application constructor
		// TODO(high): Dedicate platform/backend selection to the builder, not application!
		Log.Info("Launch application.");

		try
		{
			if (Headless)
			{
				MainLoop();
			}

			// TODO: allow user to configure force candidate (parse program cli arguments)
			Platform = SelectBestPlatform(config.Platforms.Select(t => t(this)).ToList());
			Log.Info("Selected Platform: {0}", Platform.Api);

			// TODO: config.GetConfiguration<ViewConfiguration>() ?? default;
			WindowConfiguration windowConfig = new()
			{
				Title = "Application",
				Position = null,
				Size = new(1280, 720)
			};
			MainView = Platform.CreateView(in windowConfig);

			GraphicsBackend = SelectBestGraphicsBackend(config.GraphicsBackends.Select(t => t(this)).ToList());
			Log.Info("Selected Graphics Backend: {0}", GraphicsBackend.Api);
			MainSurface = GraphicsBackend.CreateSurface(MainView);

			FramePacer pacer = new();
			RenderThread = new RenderThread(this, pacer);
			RenderThread.Start();

			MainLoopWithGraphics(pacer);

			RenderThread.Stop();
			RenderThread = null;
			Platform.ReleaseView(MainView);
			MainView = null!;
		}
		catch (Exception ex)
		{
			Log.Error(ex, "Error during application launch.");
			Platform?.TryCreateErrorMessage(ex.Message + "\n" + ex.StackTrace);
			return false;
		}

		return true;
	}

	private void MainLoop()
	{
		TimeSpan mhz10 = TimeSpan.FromMilliseconds(100);
		long last = Stopwatch.GetTimestamp();

		RateStatistics statistics = new();
		statistics.Start();

		ulong updateId = 0;
		while (true) // TODO: Add Close Methods to application
		{
			long now = Stopwatch.GetTimestamp();
			float deltaTime = (now - last) / (float)Stopwatch.Frequency;
			if (deltaTime < mhz10.Seconds)
			{
				Thread.Sleep(TimeSpan.FromSeconds(mhz10.Seconds - deltaTime));
				continue;
			}

			Update(updateId, deltaTime);
			updateId++;
			statistics.Update();
			last = now;
		}
	}

	private void MainLoopWithGraphics(FramePacer pacer)
	{
		long last = Stopwatch.GetTimestamp();

		Debug.Assert(RenderThread != null);
		Debug.Assert(MainView != null);
		var renderThread = RenderThread!;
		var view = MainView!;
		RateStatistics statistics = new();
		statistics.Start();

		bool firstFrame = true;
		ulong updateId = 0;
		while (view.IsAlive && renderThread.IsValid)
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
			CameraData cameraData = default;
			if (snapshot != null && (view.SurfaceSize.X != 0 && view.SurfaceSize.Y != 0))
			{
				Draw(ref cameraData, view.SurfaceSize, updateId, deltaTime);
			}
			statistics.Update();
			last = now;

			if (snapshot != null)
			{
				snapshot.Size = view.SurfaceSize;
				snapshot.Camera = cameraData;
				pacer.Submit(snapshot);
			}
		}
	}

	// Input events are called during PumpEvents on MAIN thread
	// If platform dedicate a separate thread it should NOT invoke these method from that thread.
	[ThreadSafety(AllowedThread.Main)]
	public virtual void OnKeyDown(Key key)
	{ }

	[ThreadSafety(AllowedThread.Main)]
	public virtual void OnKeyUp(Key key)
	{ }

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