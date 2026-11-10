using NiTiS.Mathematics;
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
	private readonly HashSet<WlOutput> _outputs = [];

	private int _width;
	private int _height;
	private int _scale = 1;
	private int _preferredScale;
	private bool _configured;
	private bool _alive = true;
	private bool _destroyed;

	public override Vector2<int> Size => new(_width, _height);

	/// <summary>
	/// Wayland configures logical window sizes; the Vulkan surface is in physical pixels.
	/// </summary>
	public override Vector2<int> SurfaceSize =>
		new(Math.Max(1, (int)MathF.Round(_width * _scale)),
			Math.Max(1, (int)MathF.Round(_height * _scale)));

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

		if (Size != previousSize || SurfaceSize != previousSurfaceSize)
		{
			SizeChanged?.Invoke(new SizeChangedEventArgs(Size, SurfaceSize));
		}
	}

	private void UpdateScale()
	{
		Vector2<int> previousSize = Size;
		Vector2<int> previousSurfaceSize = SurfaceSize;

		int scale = _preferredScale;
		foreach (WlOutput output in _outputs)
		{
			scale = Math.Max(scale, Platform.GetOutputScale(output));
		}

		_scale = Math.Max(1, scale);

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
