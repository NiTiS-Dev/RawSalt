using System.Runtime.InteropServices;

namespace NiTiS.GLFW;
public unsafe partial class Glfw
{
	public static string VersionString => Marshal.PtrToStringUTF8((nint)GetVersionString());
	public static (int major, int minor, int rev) Version
	{
		get
		{
			int* vers = stackalloc int[3];

			GetVersion(vers, vers + 1, vers + 2);

			return (vers[0], vers[1], vers[2]);
		}
	}
}