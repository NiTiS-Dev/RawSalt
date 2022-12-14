using NiTiS.GLFW;
using NiTiS.GLFW.Enums;
using NiTiS.Math;
using System;
using System.Text;

namespace NiTiS.Windowing.GLFW;

public unsafe class GlfwWindow : Window
{
	private GlfwWindowHandle* handle;
	public GlfwWindow(WindowOptions options) : base(options)
	{

	}
	public override void Initialize()
	{
		if (options.Title?.Length <= 0)
			throw new ArgumentException("The window name must be set and non empty");

		// Title
		int bufferSize = Encoding.UTF8.GetByteCount(options.Title) + 1;
		Span<byte> chars = stackalloc byte[bufferSize];

		Encoding.UTF8.GetBytes(options.Title, chars);

		chars[bufferSize -1] = 0;

		// Handle
		fixed (byte* pTitle = chars)
		{
			handle = Glfw.CreateWindow(options.Size.X, options.Size.Y, pTitle, null, null);
		}

		Glfw.MakeContextCurrent(handle);

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
		Glfw.SetWindowAttrib(handle, WindowHintSetter.Decorated, options.HasBorder.ToGlfw());
		Glfw.SetWindowHint(WindowHintBool.Focused, true);

		Glfw.SetWindowHint(WindowHintInt.ContextVersionMajor, options.Graphics.Version.Major);
		Glfw.SetWindowHint(WindowHintInt.ContextVersionMinor, options.Graphics.Version.Minor);

		Glfw.PrepareForOpenGL();

		// Window props
		Glfw.SetWindowAttrib(handle, WindowHintSetter.Floating, options.AlwaysOnTop.ToGlfw());

		switch (options.Border)
		{
			case WindowBorder.Resizable:
				Glfw.SetWindowAttrib(handle, WindowHintSetter.Resizable, GlfwBool.True);
				break;
			case WindowBorder.FixedSize:
				Glfw.SetWindowAttrib(handle, WindowHintSetter.Resizable, GlfwBool.False);
				break;
				
		}

		Glfw.SetWindowHint(WindowHintString.X11ClassName, options.ClassName);

		base.Initialize();
	}
	public override IWindow CreateWindow(WindowOptions options)
	{
		return new GlfwWindow(options);
	}
	public override void Destroy()
	{
		if (!isInitialized)
			return;

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
	public override bool TransperentFramebuffer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	public override Vector2D<int> Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	public override Vector2D<int> Size { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	public override WindowState State { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	public override WindowBorder Border { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

	public override string WindowClassName => throw new NotImplementedException();

	public static explicit operator GlfwWindowHandle*(GlfwWindow window)
		=> window.handle;
}