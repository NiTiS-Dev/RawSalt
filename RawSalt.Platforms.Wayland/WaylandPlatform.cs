using NWayland;
using NWayland.Interop;
using NWayland.Protocols.Wayland;
using NWayland.Protocols.XdgDecorationUnstableV1;
using NWayland.Protocols.XdgShell;
using NiTiS.Mathematics;
using RawSalt.Logging;
using RawSalt.Platforms;

namespace RawSalt.Windowing.Wayland;

public sealed class WaylandPlatform : Platform
{
	public override WellKnownPlatformApi Api => WellKnownPlatformApi.Wayland;

	internal IntPtr DisplayHandle => _display.Handle;
	internal WlCompositor Compositor { get; private set; } = null!;
	internal XdgWmBase WmBase { get; private set; } = null!;
	internal ZxdgDecorationManagerV1? DecorationManager { get; private set; }

	private readonly WlDisplay _display;
	private readonly WlRegistry _registry;
	private readonly WlRegistry.Listener.Relay _registryListener;
	private readonly WlCompositor.Listener.Relay _compositorListener = new();
	private readonly XdgWmBase.Listener.Relay _wmBaseListener;
	private readonly ZxdgDecorationManagerV1.Listener.Relay _decorationListener = new();
	private readonly Dictionary<WlOutput, WaylandDisplay> _outputs = [];
	private readonly Dictionary<nint, WaylandWindow> _windows = [];
	private readonly int _displayFd;

	public WaylandPlatform(Application application) : base(application)
	{
		_display = WlDisplay.Connect(null!, new WlDisplayOptions());
		_displayFd = _display.GetFd();

		_wmBaseListener = new XdgWmBase.Listener.Relay
		{
			OnPing = (wmBase, serial) => wmBase.Pong(serial)
		};

		_registryListener = new WlRegistry.Listener.Relay
		{
			OnGlobal = OnGlobal,
			OnGlobalRemove = OnGlobalRemove
		};

		_registry = _display.GetRegistry(_registryListener, null!);
		_display.Roundtrip();

		if (Compositor == null || WmBase == null)
		{
			throw new NotSupportedException("Wayland compositor does not expose wl_compositor and xdg_wm_base.");
		}

		// Collect the initial state of the bound outputs.
		_display.Roundtrip();
	}

	private void OnGlobal(WlRegistry registry, uint name, string @interface, uint version)
	{
		switch (@interface)
		{
			case "wl_compositor":
				Compositor = WlCompositor.Bind(registry, name, Math.Min(version, 4), _compositorListener, null!);
				break;

			case "xdg_wm_base":
				WmBase = XdgWmBase.Bind(registry, name, Math.Min(version, 1), _wmBaseListener, null!);
				break;

			case "wl_output":
				WaylandDisplay display = new(name);
				display.Output = WlOutput.Bind(registry, name, Math.Min(version, 4), display.Listener, null!);
				_outputs.Add(display.Output, display);
				break;

			case "zxdg_decoration_manager_v1":
				DecorationManager = ZxdgDecorationManagerV1.Bind(registry, name, Math.Min(version, 1),
					_decorationListener, null!);
				break;
		}
	}

	private void OnGlobalRemove(WlRegistry registry, uint name)
	{
		foreach (WaylandDisplay display in _outputs.Values.ToArray())
		{
			if (display.RegistryName != name || display.Output is not WlOutput output)
			{
				continue;
			}

			_outputs.Remove(output);
			display.Output = null;
			output.Dispose();
			break;
		}
	}

	public static bool IsAvailable()
	{
		if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WAYLAND_DISPLAY")))
		{
			return false;
		}

		try
		{
			using WlDisplay display = WlDisplay.Connect(null!, new WlDisplayOptions());
			return true;
		}
		catch (Exception ex) when (ex is DllNotFoundException or EntryPointNotFoundException or NWaylandException)
		{
			return false;
		}
	}

	internal int GetOutputScale(WlOutput output)
	{
		return _outputs.TryGetValue(output, out WaylandDisplay? display) ? display.Scale : 1;
	}

	protected override void Dispose(bool disposing)
	{
		foreach (WaylandWindow window in _windows.Values.ToArray())
		{
			window.Close();
		}

		_windows.Clear();

		foreach (WaylandDisplay display in _outputs.Values)
		{
			display.Output?.Dispose();
		}

		_outputs.Clear();

		DecorationManager?.Dispose();
		DecorationManager = null;
		WmBase?.Dispose();
		WmBase = null!;
		Compositor?.Dispose();
		Compositor = null!;
		_registry.Dispose();
		_display.Dispose();
	}

	public override IReadOnlyList<Display> GetDisplays()
	{
		return [.. _outputs.Values];
	}

	public override WaylandWindow CreateView(in WindowConfiguration configuration)
	{
		WaylandWindow window = new(this, configuration.Size, configuration.Title);
		_windows.Add(window.Surface.Handle, window);
		return window;
	}

	public override bool ReleaseView(View view)
	{
		if (view is not WaylandWindow window)
		{
			return false;
		}

		_windows.Remove(window.Surface.Handle);
		window.Close();
		return true;
	}

	public override void PumpEvents()
	{
		try
		{
			// Non-blocking read cycle: prepare, poll, read (or cancel), then dispatch.
			if (_display.PrepareRead() == 0)
			{
				PollFd descriptor = new()
				{
					fd = _displayFd,
					events = LibC.PollIn
				};

				if (LibC.poll([descriptor], 1, 0) > 0)
				{
					_display.ReadEvents();
				}
				else
				{
					_display.CancelRead();
				}
			}

			_display.DispatchPending();
			_display.Flush();
		}
		catch (Exception ex) when (ex is NWaylandException or ObjectDisposedException)
		{
			Log.Error(ex, "Wayland event pump failed.");
		}
	}

	public override bool TryCreateErrorMessage(string message)
	{
		Console.Error.WriteLine(message);
		return true;
	}

	internal void RemoveWindow(WaylandWindow window)
	{
		_windows.Remove(window.Surface.Handle);
	}
}
