using NiTiS.GLFW;
using NiTiS.GLFW.Enums;
using NiTiS.Math;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace NiTiS.Windowing.GLFW;

public unsafe class GlfwWindow : Window
{
	private GlfwWindowHandle* handle;
	private GCHandle gcHandle;

	#region Callbacks
	private GlfwCallbacks.WindowPos onMove;
	private GlfwCallbacks.WindowSize onResize;
	private GlfwCallbacks.FramebufferSize onFramebufferResize;
	private GlfwCallbacks.Drop onFileDrop;
	private GlfwCallbacks.WindowClose onClosing;
	private GlfwCallbacks.WindowFocus onFocusChanged;
	private GlfwCallbacks.WindowIconify onMinimized;
	private GlfwCallbacks.WindowMaximize onMaximized;
	private GlfwCallbacks.WindowRefresh onRefresh;
	#endregion

	public event Action Refresh;
	public event Action<Vector2D<int>> Move;

	public GlfwWindow(WindowOptions options) : base(options)
	{

	}
	public override void Initialize()
	{
		if (options.Title?.Length <= 0)
			throw new ArgumentException("The window name must be set and non empty");

		gcHandle = GCHandle.Alloc(this);

		//Border
		switch (options.Border)
		{
			case WindowBorder.Resizable:
				Glfw.SetWindowHint(WindowHintBool.Resizable, true);
				Glfw.SetWindowHint(WindowHintBool.Decorated, true);
				break;
			case WindowBorder.Hidden:
				Glfw.SetWindowHint(WindowHintBool.Resizable, false);
				Glfw.SetWindowHint(WindowHintBool.Decorated, false);
				break;
			case WindowBorder.FixedSize:
				Glfw.SetWindowHint(WindowHintBool.Resizable, false);
				Glfw.SetWindowHint(WindowHintBool.Decorated, true);
				break;
		}

		// API
		Glfw.SetWindowHint(WindowHintContextAPI.ContextCreationApi, ContextAPI.NativeContext);
		Glfw.SetWindowHint(WindowHintClientAPI.ClientApi, options.Graphics.API switch
		{
			ContextAPIType.OpenGL => ClientAPI.OpenGL,
			_ => ClientAPI.NoAPI,
		});
		Glfw.SetWindowHint(WindowHintOpenGLProfile.OpenGlProfile, options.Graphics.Profile switch
		{
			ContextAPIProfile.Core => OpenGLProfile.Core,
			ContextAPIProfile.Compatability => OpenGLProfile.Compatibility,
			_ => OpenGLProfile.Any,
		});

		if (options.IsVisible)
			Glfw.SetWindowHint(WindowHintBool.Focused, true);

		Glfw.SetWindowHint(WindowHintInt.ContextVersionMajor, options.Graphics.Version.Major);
		Glfw.SetWindowHint(WindowHintInt.ContextVersionMinor, options.Graphics.Version.Minor);

		Glfw.SetWindowHint(WindowHintString.X11ClassName, options.ClassName);

		// Video mode
		Glfw.SetWindowHint(WindowHintInt.RefreshRate, options.RefreshRate ?? Glfw.DontCare);
		Glfw.SetWindowHint(WindowHintInt.DepthBits,   options.PreferredDepthBufferBits ?? Glfw.DontCare);
		Glfw.SetWindowHint(WindowHintInt.StencilBits, options.PreferredStencilBufferBits ?? Glfw.DontCare);

		Glfw.SetWindowHint(WindowHintInt.RedBits,   options.PreferredBitDepth?.X ?? Glfw.DontCare);
		Glfw.SetWindowHint(WindowHintInt.GreenBits, options.PreferredBitDepth?.Y ?? Glfw.DontCare);
		Glfw.SetWindowHint(WindowHintInt.BlueBits,  options.PreferredBitDepth?.Z ?? Glfw.DontCare);
		Glfw.SetWindowHint(WindowHintInt.AlphaBits, options.PreferredBitDepth?.W ?? Glfw.DontCare);

		Glfw.SetWindowHint(WindowHintBool.TransparentFramebuffer, options.TransparentFramebuffer);

		Glfw.SetWindowHint(WindowHintInt.Samples, options.Samples ?? Glfw.DontCare);

		Glfw.SetWindowHint(WindowHintBool.Visible, options.IsVisible);


		// Title
		int bufferSize = Encoding.UTF8.GetByteCount(options.Title) + 1;
		Span<byte> chars = stackalloc byte[bufferSize];

		Encoding.UTF8.GetBytes(options.Title, chars);

		chars[bufferSize - 1] = 0;

		// Handle
		fixed (byte* pTitle = chars)
		{
			handle = Glfw.CreateWindow(options.Size.X, options.Size.Y, pTitle, null, null);
		}

		if (options.Graphics.API is ContextAPIType.OpenGL)
		{
			Glfw.MakeContextCurrent(handle);
			Glfw.PrepareForOpenGL();
		}


		GlfwError error = Glfw.GetError(null);
		if (error != GlfwError.NoError)
		{
			throw new Exception($"Glfw error: " + error);
		}

		InitializeCallbacks();

		base.Initialize();
	}
	private void InitializeCallbacks()
	{
		onRefresh = window =>
		{
			Refresh();
		};
		onMove = (window, x, y) =>
		{
			Move(new(x, y));
		};

		Refresh = new(() => { });
		Move = new((_) => { });

		delegate* unmanaged[Stdcall]<GlfwWindowHandle*, void>
			pOnRefresh = (delegate* unmanaged[Stdcall]<GlfwWindowHandle*, void>)
		Marshal.GetFunctionPointerForDelegate(onRefresh);

		delegate* unmanaged[Stdcall]<GlfwWindowHandle*, int, int, void>
			pOnMove = (delegate* unmanaged[Stdcall]<GlfwWindowHandle*, int, int, void>)
		Marshal.GetFunctionPointerForDelegate(onMove);

		Glfw.SetWindowRefreshCallback(handle, pOnRefresh);
		Glfw.SetWindowPosCallback(handle, pOnMove);
	}

	public override IWindow CreateWindow(WindowOptions options)
	{
		return new GlfwWindow(options);
	}
	public override void Destroy()
	{
		if (!isInitialized)
			return;

		gcHandle.Free();
		gcHandle = default;

		Glfw.DestroyWindow(handle);
	}
	public override void SwapBuffers()
	{
		if (!isInitialized)
			return;

		Glfw.SwapBuffers(handle);
	}
	public override string Title
	{
		set
		{
			options.Title = value;

			if (!isInitialized)
				return;

			Span<byte> chars = stackalloc byte[value.Length];

			Encoding.UTF8.GetBytes(value, chars);

			fixed (byte* pChars = chars)
			{
				Glfw.SetWindowTitle(handle, pChars);
			}
		}
		get
			=> !isInitialized
			? options.Title
			: Glfw.GetWindowTitle(handle).ToString()
			;
	}
	public override bool IsVisible
	{
		set
		{
			options.IsVisible = value;

			if (!isInitialized)
				return;

			Glfw.WindowHint((int)WindowHintBool.Visible, (int)(value ? GlfwBool.True : GlfwBool.False));
		}
		get
			=> !isInitialized
			? options.IsVisible
			: Glfw.GetWindowAttrib(handle, WindowHintGetter.Visible) == GlfwBool.True
			;
	}
	public override bool AlwaysOnTop
	{
		set
		{
			options.AlwaysOnTop = value;

			if (!isInitialized)
				return;

			Glfw.SetWindowAttrib(handle, WindowHintSetter.Floating, value ? GlfwBool.True : GlfwBool.False);
		}
		get => !isInitialized
			? options.AlwaysOnTop
			: Glfw.GetWindowAttrib(handle, WindowHintGetter.Floating) == GlfwBool.True
			;
	}
	public override bool TransperentFramebuffer
	{
		set
		{
			options.TransparentFramebuffer = value;

			if (!isInitialized)
				return;

			Glfw.SetWindowHint(WindowHintBool.TransparentFramebuffer, value);
		}
		get => !isInitialized
			? options.TransparentFramebuffer
			: Glfw.GetWindowAttrib(handle, WindowHintGetter.TransparentFramebuffer) == GlfwBool.True
			;
	}
	public override Vector2D<int> Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	public override Vector2D<int> Size { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	public override WindowState State { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	public override WindowBorder Border { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

	public override string WindowClassName => throw new NotImplementedException();

	public static explicit operator GlfwWindowHandle*(GlfwWindow window)
		=> window.handle;
}