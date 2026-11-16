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

	private readonly Lock _sizeLock = new();
	private Vector2<int> _size, _surfaceSize;

	public override Vector2<int> Size
	{
		get
		{
			lock (_sizeLock)
			{
				return _size;
			}
		}
	}

	public override Vector2<int> SurfaceSize
	{
		get
		{
			lock (_sizeLock)
			{
				return _surfaceSize;
			}
		}
	}

	nint IWin32Window.Handle => Handle;
	nint IWin32Window.ModuleHandle => Platform.HInstance;

	private volatile bool _alive = true;
	public override bool ShouldUpdate => _alive;

	internal Win32Window(HWND handle, Win32Platform platform)
	{
		Handle = handle;
		Platform = platform;
	}

	[SupportedOSPlatform("windows5.0")]
	public void Close()
	{
		_alive = false;
		Platform.Post(() => PInvoke.DestroyWindow(Handle));
	}

	[SupportedOSPlatform("windows5.0")]
	public override void Hide()
	{
		Platform.Post(() => PInvoke.ShowWindow(Handle, SHOW_WINDOW_CMD.SW_HIDE));
	}

	[SupportedOSPlatform("windows5.0")]
	public override void Show()
	{
		Platform.Post(() => PInvoke.ShowWindow(Handle, SHOW_WINDOW_CMD.SW_SHOW));
	}

	internal void RequestClose()
	{
		_alive = false;
	}

	[SupportedOSPlatform("windows10.0")]
	internal void UpdateSize()
	{
		PInvoke.GetWindowRect(Handle, out RECT windowRect);
		Vector2<int> newSize = new(windowRect.Width, windowRect.Height);

		PInvoke.GetClientRect(Handle, out RECT clientRect);
		// The process is per-monitor DPI aware (v2), so the client rect is already in physical pixels.
		Vector2<int> newSurfaceSize = new(clientRect.Width, clientRect.Height);

		lock (_sizeLock)
		{
			_size = newSize;
			_surfaceSize = newSurfaceSize;
		}
	}
}