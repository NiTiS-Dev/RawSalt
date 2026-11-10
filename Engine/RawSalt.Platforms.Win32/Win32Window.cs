using System.Runtime.Versioning;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;
using NiTiS.Mathematics;

namespace RawSalt.Platforms.Win32;

public class Win32Window : Window, IWin32Window
{
	public override Win32Platform Platform { get; }

	internal HWND Handle { get; }
	private Vector2<int> _size, _surfaceSize;
	public override Vector2<int> Size => _size;
	public override Vector2<int> SurfaceSize => _surfaceSize;
	public override event Action<SizeChangedEventArgs>? SizeChanged;

	nint IWin32Window.Handle => Handle;
	nint IWin32Window.ModuleHandle => Platform.HInstance;

	private bool _alive = true;
	public override bool ShouldUpdate => _alive;
	public override event Action<CloseRequest>? CloseRequest;

	internal Win32Window(HWND handle, Win32Platform platform)
	{
		Handle = handle;
		Platform = platform;
	}

	[SupportedOSPlatform("windows5.0")]
	public override void PumpEvents()
	{
		while (PInvoke.PeekMessage(out MSG msg, Handle, 0, 0, PEEK_MESSAGE_REMOVE_TYPE.PM_REMOVE))
		{
			PInvoke.TranslateMessage(msg);
			PInvoke.DispatchMessage(msg);
		}
	}

	[SupportedOSPlatform("windows5.0")]
	public void Close()
	{
		PInvoke.DestroyWindow(Handle);
		_alive = false;
	}

	[SupportedOSPlatform("windows5.0")]
	public override void Hide()
	{
		PInvoke.ShowWindow(Handle, SHOW_WINDOW_CMD.SW_HIDE);
	}

	[SupportedOSPlatform("windows5.0")]
	public override void Show()
	{
		PInvoke.ShowWindow(Handle, SHOW_WINDOW_CMD.SW_SHOW);
	}

	internal void OnCloseRequested()
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

	[SupportedOSPlatform("windows10.0")]
	internal void UpdateSize()
	{
		PInvoke.GetWindowRect(Handle, out RECT windowRect);
		Vector2<int> newSize = new(windowRect.Width, windowRect.Height);

		PInvoke.GetClientRect(Handle, out RECT clientRect);
		// The process is per-monitor DPI aware (v2), so the client rect is already in physical pixels.
		Vector2<int> newSurfaceSize = new(clientRect.Width, clientRect.Height);

		if (_size == newSize && _surfaceSize == newSurfaceSize)
		{
			return;
		}

		_size = newSize;
		_surfaceSize = newSurfaceSize;
		SizeChanged?.Invoke(new SizeChangedEventArgs(_size, _surfaceSize));
	}
}