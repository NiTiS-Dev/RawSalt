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
	/// <summary>
	/// Prepare GLFW for OpenGL calls
	/// </summary>
	public static void PrepareForOpenGL()
	{
		NiTiS.Internal.ContextualAPI.ContextualStorage.openGL = GetProcAddress;
	}
}