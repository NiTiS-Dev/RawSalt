using System.IO.Compression;
using System.Numerics;
using System.Reflection.Metadata;
using System.Runtime.Versioning;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Dwm;
using Windows.Win32.UI.WindowsAndMessaging;
using NiTiS.Mathematics;
using RawSalt.Platforms;

namespace RawSalt.Windowing.Win32;

public class Win32Window : Window, IWin32Window
{
	private readonly Win32Platform _platform;

	internal HWND Handle { get; }
	private Vector2<int> _size, _surfaceSize;
	public override Vector2<int> Size => _size;
	public override Vector2<int> SurfaceSize => _surfaceSize;
	public override event Action<SizeChangedEventArgs>? SizeChanged;

	nint IWin32Window.Handle => Handle;
	nint IWin32Window.ModuleHandle => _platform.HInstance;

	private bool _alive = true;
	public override bool ShouldUpdate => _alive;
	public override event Action<CloseRequest>? CloseRequest;

	internal Win32Window(HWND handle, Win32Platform platform)
	{
		Handle = handle;
		_platform = platform;
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

	internal void OnResize(Vector2<int> size, Vector2<int> surfaceSize)
	{
		_size = size;
		_surfaceSize = surfaceSize;
		SizeChanged?.Invoke(new SizeChangedEventArgs(size, surfaceSize));
	}
}