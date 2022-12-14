using NiTiS.GLFW.Enums;
using NiTiS.Native;
using System;
using System.Text;

namespace NiTiS.GLFW;
public unsafe partial class Glfw
{
	public static string VersionString => GetVersionString().ToString();
	public static (int major, int minor, int rev) Version
	{
		get
		{
			int* vers = stackalloc int[3];

			GetVersion(vers, vers + 1, vers + 2);

			return (vers[0], vers[1], vers[2]);
		}
	}
	public static void SetWindowHint(WindowHintBool attr, bool value)
		=> SetWindowHint(attr, value ? GlfwBool.True : GlfwBool.False);
	public static void SetWindowHint(WindowHintBool attr, GlfwBool value)
	{
		Glfw.WindowHint((int)attr, (int)value);
	}
	public static void SetWindowHint(WindowHintClientAPI attr, ClientAPI value)
	{
		Glfw.WindowHint((int)attr, (int)value);
	}
	public static void SetWindowHint(WindowHintContextAPI attr, ContextAPI value)
	{
		Glfw.WindowHint((int)attr, (int)value);
	}
	public static void SetWindowHint(WindowHintInt attr, int value)
	{
		Glfw.WindowHint((int)attr, value);
	}
	public static void SetWindowHint(WindowHintOpenGLProfile attr, OpenGLProfile value)
	{
		Glfw.WindowHint((int)attr, (int)value);
	}
	public static void SetWindowHint(WindowHintReleaseBehavior attr, ReleaseBehavior value)
	{
		Glfw.WindowHint((int)attr, (int)value);
	}
	public static void SetWindowHint(WindowHintRobustness attr, Robustness value)
	{
		Glfw.WindowHint((int)attr, (int)value);
	}
	public static void SetWindowHint(WindowHintString attr, string value)
	{
		int bufferSize = Encoding.UTF8.GetByteCount(value) + 1;
		Span<byte> chars = stackalloc byte[bufferSize];

		//MOOT: Maybe its not required cause all stack zero-based?!
		chars[bufferSize - 1] = 0;

		Encoding.UTF8.GetBytes(value, chars);

		fixed (byte* pChars = chars)
		{
			Glfw.WindowHintString(attr, pChars);
		}
	}
	public static void SetWindowHint(WindowHintString attr, CString value)
	{
		Glfw.WindowHintString(attr, value);
	}
	/// <summary>
	/// Prepare GLFW for OpenGL calls
	/// </summary>
	public static void PrepareForOpenGL()
	{
		Internal.ContextualAPI.ContextualStorage.openGL = GetProcAddress;
	}
}