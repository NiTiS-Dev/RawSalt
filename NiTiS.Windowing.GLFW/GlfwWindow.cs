using NiTiS.GLFW;
using NiTiS.GLFW.Enums;
using NiTiS.Math;
using NiTiS.Native;
using System;
using System.Text;

namespace NiTiS.Windowing.GLFW;

public unsafe class GlfwWindow : Window
{
	private GlfwWindowHandle* handle;
	private WindowOptions options;
	public GlfwWindow(WindowOptions options) : base(options)
	{

	}
	public override void Initialize()
	{


		base.Initialize();
	}
	public override string Title
	{
		set
		{
			Span<byte> chars = stackalloc byte[value.Length];

			Encoding.UTF8.GetBytes(value, chars);

			fixed (byte* pChars = chars)
			{
				Glfw.SetWindowTitle(handle, pChars);
			}
		}
		get
			=> Glfw.GetWindowTitle(handle).ToString();
	}

	public override string Title { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

	public bool IsVisible
	{
		set
		{
			Glfw.WindowHint((int)WindowBoolAttribute.Visible, (int)(value ? GlfwBool.True : GlfwBool.False));
		}
		get
			=> Glfw.GetWindowAttrib(handle, WindowAttributeGetter.Visible) == GlfwBool.True;
	}

	public override bool IsVisible { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	public override bool AlwaysOnTop { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	public override bool TransperentFramebuffer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	public override Vector2D<int> Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	public override Vector2D<int> Size { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	public override WindowState State { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	public override WindowBorder Border { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

	public override string WindowClassName => throw new NotImplementedException();

	public override IWindow CreateWindow() => throw new NotImplementedException();
}
