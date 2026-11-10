using NiTiS.Mathematics;
using RawSalt.Platforms;

namespace RawSalt.Windowing.Xlib;

public sealed class XlibWindow : Window, INativeWindow
{
	public override XlibPlatform Platform { get; }

	internal ulong Handle { get; }

	private Vector2<int> _size;
	private bool _alive = true;
	private bool _destroyed;

	public override Vector2<int> Size => _size;

	/// <summary>
	/// X11 has no per-window scale factor, so the surface size matches the window size.
	/// </summary>
	public override Vector2<int> SurfaceSize => _size;

	public override event Action<SizeChangedEventArgs>? SizeChanged;
	public override event Action<CloseRequest>? CloseRequest;

	public override bool ShouldUpdate => _alive;

	NativeWindowHandle INativeWindow.NativeHandle =>
		new(WellKnownPlatformApi.Xlib, Platform.DisplayHandle, unchecked((nint)Handle));

	internal XlibWindow(ulong handle, XlibPlatform platform)
	{
		Handle = handle;
		Platform = platform;
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
		XlibNative.XDestroyWindow(Platform.DisplayHandle, Handle);
		XlibNative.XFlush(Platform.DisplayHandle);
	}

	public override void Hide()
	{
		XlibNative.XUnmapWindow(Platform.DisplayHandle, Handle);
		XlibNative.XFlush(Platform.DisplayHandle);
	}

	public override void Show()
	{
		XlibNative.XMapWindow(Platform.DisplayHandle, Handle);
		XlibNative.XFlush(Platform.DisplayHandle);
	}

	internal void MarkDestroyed()
	{
		_alive = false;
		_destroyed = true;
	}

	internal void ProcessEvent(in XEvent xevent)
	{
		switch (xevent.type)
		{
			case XlibNative.ConfigureNotify:
				UpdateSize();
				break;

			case XlibNative.ClientMessage:
				if (xevent.client.message_type == Platform.WmProtocolsAtom &&
				    xevent.client.format == 32 &&
				    unchecked((ulong)xevent.client.data.l0) == Platform.WmDeleteWindowAtom)
				{
					OnCloseRequested();
				}

				break;

			case XlibNative.DestroyNotify:
				_alive = false;
				break;
		}
	}

	internal void UpdateSize()
	{
		if (XlibNative.XGetWindowAttributes(Platform.DisplayHandle, Handle, out XWindowAttributes attributes) == 0)
		{
			return;
		}

		Vector2<int> size = new(attributes.width, attributes.height);
		if (_size == size)
		{
			return;
		}

		_size = size;
		SizeChanged?.Invoke(new SizeChangedEventArgs(size, size));
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
