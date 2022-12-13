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
	public static void SetWindowHint(WindowBoolAttribute attr, bool value)
		=> SetWindowHint(attr, value ? GlfwBool.True : GlfwBool.False);
	public static void SetWindowHint(WindowBoolAttribute attr, GlfwBool value)
	{
		Glfw.WindowHint((int)attr, (int)value);
	}
	public static void SetWindowHint(WindowClientAPIAttribute attr, ClientAPI value)
	{
		Glfw.WindowHint((int)attr, (int)value);
	}
	public static void SetWindowHint(WindowContextAPIAttribute attr, ContextAPI value)
	{
		Glfw.WindowHint((int)attr, (int)value);
	}
	public static void SetWindowHint(WindowIntAttribute attr, int value)
	{
		Glfw.WindowHint((int)attr, (int)value);
	}
	public static void SetWindowHint(WindowOpenGLProfileAttribute attr, OpenGLProfile value)
	{
		Glfw.WindowHint((int)attr, (int)value);
	}
	public static void SetWindowHint(WindowReleaseBehaviorAttribute attr, ReleaseBehavior value)
	{
		Glfw.WindowHint((int)attr, (int)value);
	}
	public static void SetWindowHint(WindowRobustnessAttribute attr, Robustness value)
	{
		Glfw.WindowHint((int)attr, (int)value);
	}
	public static void SetWindowHint(WindowStringAttirubte attr, string value)
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
	public static void SetWindowHint(WindowStringAttirubte attr, CString value)
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