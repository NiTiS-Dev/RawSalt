using NiTiS.Mathematics;
using NWayland.Protocols.FractionalScaleV1;
using NWayland.Protocols.Viewporter;
using NWayland.Protocols.Wayland;
using NWayland.Protocols.XdgDecorationUnstableV1;
using NWayland.Protocols.XdgShell;
using RawSalt.Input;
using RawSalt.Platforms;

namespace RawSalt.Windowing.Wayland;

public sealed class WaylandWindow : Window, INativeWindow
{
	public override WaylandPlatform Platform { get; }

	internal WlSurface Surface { get; }
	internal XdgSurface XdgSurface { get; }
	internal XdgToplevel Toplevel { get; }

	private readonly WlSurface.Listener.Relay _surfaceListener;
	private readonly XdgSurface.Listener.Relay _xdgSurfaceListener;
	private readonly XdgToplevel.Listener.Relay _toplevelListener;
	private readonly ZxdgToplevelDecorationV1.Listener.Relay _decorationListener;
	private readonly ZxdgToplevelDecorationV1? _decoration;
	private readonly WpViewport.Listener.Relay _viewportListener = new();
	private readonly WpFractionalScaleV1.Listener.Relay _fractionalScaleListener;
	private readonly WpViewport? _viewport;
	private readonly WpFractionalScaleV1? _fractionalScale;
	private readonly HashSet<WlOutput> _outputs = [];

	private int _width;
	private int _height;
	private double _scaleFactor = 1;
	private int _preferredScale;
	private bool _useFractionalScale;
	private bool _configured;
	private bool _alive = true;
	private bool _destroyed;

	public override Vector2<int> Size => new(_width, _height);

	/// <summary>
	/// Wayland configures logical window sizes; the Vulkan surface is in physical pixels.
	/// </summary>
	public override Vector2<int> SurfaceSize =>
		new(Math.Max(1, (int)Math.Round(_width * _scaleFactor)),
			Math.Max(1, (int)Math.Round(_height * _scaleFactor)));

	public override event Action<SizeChangedEventArgs>? SizeChanged;
	public override event Action<CloseRequest>? CloseRequest;

	public override bool ShouldUpdate => _alive;

	NativeWindowHandle INativeWindow.NativeHandle =>
		new(WellKnownPlatformApi.Wayland, Platform.DisplayHandle, Surface.Handle);

	internal WaylandWindow(WaylandPlatform platform, Vector2<int> requestedSize, string title)
	{
		Platform = platform;
		_width = Math.Max(1, requestedSize.X);
		_height = Math.Max(1, requestedSize.Y);
		_scaleFactor = Math.Max(1, platform.GetMaximumOutputScale());

		_fractionalScaleListener = new WpFractionalScaleV1.Listener.Relay
		{
			OnPreferredScale = (_, scale) =>
			{
				if (scale == 0)
				{
					return;
				}

				Vector2<int> previousSize = Size;
				Vector2<int> previousSurfaceSize = SurfaceSize;
				_scaleFactor = scale / 120.0;
				ApplySurfaceScale();
				NotifySizeChanged(previousSize, previousSurfaceSize);
			}
		};

		_surfaceListener = new WlSurface.Listener.Relay
		{
			OnEnter = (_, output) =>
			{
				if (output != null && _outputs.Add(output))
				{
					UpdateScale();
				}
			},
			OnLeave = (_, output) =>
			{
				if (output != null && _outputs.Remove(output))
				{
					UpdateScale();
				}
			},
			OnPreferredBufferScale = (_, factor) =>
			{
				_preferredScale = Math.Max(0, factor);
				UpdateScale();
			}
		};

		_xdgSurfaceListener = new XdgSurface.Listener.Relay
		{
			OnConfigure = (xdgSurface, serial) =>
			{
				xdgSurface.AckConfigure(serial);

				if (!_configured)
				{
					// Initial configure must be applied before the first buffer is attached.
					_configured = true;
					Surface!.Commit();
				}
			}
		};

		_toplevelListener = new XdgToplevel.Listener.Relay
		{
			OnConfigure = (_, width, height, _) => UpdateLogicalSize(width, height),
			OnClose = _ => OnCloseRequested()
		};

		_decorationListener = new ZxdgToplevelDecorationV1.Listener.Relay
		{
			OnConfigure = (_, _) => { }
		};

		Surface = platform.Compositor.CreateSurface(_surfaceListener, null);

		if (platform.Viewporter != null && platform.FractionalScaleManager != null)
		{
			_viewport = platform.Viewporter.GetViewport(Surface, _viewportListener, null);
			_fractionalScale = platform.FractionalScaleManager.GetFractionalScale(Surface,
				_fractionalScaleListener, null);
			_useFractionalScale = true;
		}

		XdgSurface = platform.WmBase.GetXdgSurface(Surface, _xdgSurfaceListener, null);
		Toplevel = XdgSurface.GetToplevel(_toplevelListener, null);
		Toplevel.SetTitle(title);
		Toplevel.SetAppId("RawSalt");
		Toplevel.SetMinSize(1, 1);

		if (platform.DecorationManager != null)
		{
			_decoration = platform.DecorationManager.GetToplevelDecoration(Toplevel, _decorationListener, null);
			_decoration.SetMode(ZxdgToplevelDecorationV1.ModeEnum.ServerSide);
		}

		// Apply the initial buffer scale before the first commit.
		ApplySurfaceScale();

		// Ask the compositor for the initial configure.
		Surface.Commit();
	}

	public override void PumpEvents() => Platform.PumpEvents();

	public void Close()
	{
		if (_destroyed)
		{
			return;
		}

		_destroyed = true;
		_alive = false;
		Platform.RemoveWindow(this);

		_decoration?.Dispose();
		_fractionalScale?.Dispose();
		_viewport?.Dispose();
		Toplevel.Dispose();
		XdgSurface.Dispose();
		Surface.Dispose();
	}

	public override void Hide()
	{
		// Wayland has no unmap operation for xdg_toplevel. Hide is a no-op for now.
	}

	public override void Show()
	{
		// Wayland maps a window when the first buffer is attached by the Vulkan WSI.
	}

	internal void MarkDestroyed()
	{
		_alive = false;
		_destroyed = true;
	}

	internal void ProcessKeyboardFocus(bool focused)
	{
		OnFocusChanged(focused);
	}

	internal void ProcessKey(Key key, KeyModifiers modifiers, bool pressed, bool repeat)
	{
		if (pressed)
		{
			OnKeyDown(new KeyEventArgs(key, modifiers, repeat));
		}
		else
		{
			OnKeyUp(new KeyEventArgs(key, modifiers));
		}
	}

	internal void ProcessMouseButton(MouseButton button, bool pressed, Vector2<int> position,
		KeyModifiers modifiers)
	{
		MouseButtonEventArgs args = new(button, position, modifiers);

		if (pressed)
		{
			OnMouseDown(args);
		}
		else
		{
			OnMouseUp(args);
		}
	}

	internal void ProcessMouseMove(Vector2<int> position)
	{
		OnMouseMove(new MouseMoveEventArgs(position));
	}

	internal void ProcessMouseWheel(Vector2<int> delta)
	{
		OnMouseWheel(new MouseWheelEventArgs(delta));
	}

	internal void ProcessCursorEntered()
	{
		OnCursorEntered();
	}

	internal void ProcessCursorLeft()
	{
		OnCursorLeft();
	}

	private void UpdateLogicalSize(int width, int height)
	{
		Vector2<int> previousSize = Size;
		Vector2<int> previousSurfaceSize = SurfaceSize;

		if (width > 0)
		{
			_width = width;
		}

		if (height > 0)
		{
			_height = height;
		}

		if (Size != previousSize)
		{
			ApplySurfaceScale();
		}

		NotifySizeChanged(previousSize, previousSurfaceSize);
	}

	private void UpdateScale()
	{
		if (_useFractionalScale)
		{
			// wp_fractional_scale_v1 reports the effective scale; integer source scales are irrelevant.
			return;
		}

		Vector2<int> previousSize = Size;
		Vector2<int> previousSurfaceSize = SurfaceSize;

		int scale = _preferredScale;
		foreach (WlOutput output in _outputs)
		{
			scale = Math.Max(scale, Platform.GetOutputScale(output));
		}

		double factor = Math.Max(1, scale);
		if (Math.Abs(factor - _scaleFactor) < 0.0001)
		{
			return;
		}

		_scaleFactor = factor;
		ApplySurfaceScale();
		NotifySizeChanged(previousSize, previousSurfaceSize);
	}

	/// <summary>
	/// Applies the buffer scale or viewport destination so that the physical buffer matches
	/// <see cref="SurfaceSize"/> while the compositor displays <see cref="Size"/> logical pixels.
	/// </summary>
	private void ApplySurfaceScale()
	{
		if (_useFractionalScale)
		{
			// Viewporter requires buffer_scale 1 and maps the physical buffer onto the logical size.
			Surface.SetBufferScale(1);
			_viewport?.SetDestination(Size.X, Size.Y);
			return;
		}

		Surface.SetBufferScale(Math.Max(1, (int)Math.Round(_scaleFactor)));
	}

	private void NotifySizeChanged(Vector2<int> previousSize, Vector2<int> previousSurfaceSize)
	{
		if (Size != previousSize || SurfaceSize != previousSurfaceSize)
		{
			SizeChanged?.Invoke(new SizeChangedEventArgs(Size, SurfaceSize));
		}
	}

	private void OnCloseRequested()
	{
		if (CloseRequest is null)
		{
			_alive = false;
			return;
		}

		CloseRequest request = new();
		CloseRequest.Invoke(request);

		_alive = !request.ShouldClose;
	}
}
