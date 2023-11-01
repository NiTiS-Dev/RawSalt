using System.Runtime.InteropServices;

namespace RawSalt.Native.Windows;

public static partial class User32
{
	public const string LibName = "user32.dll";

	#region MessageBox
	[LibraryImport(LibName, EntryPoint = "MessageBoxW", StringMarshalling = StringMarshalling.Utf16)]
	public static partial MessageBoxResponse MessageBox(HWND hWND, string? lpText, string? lpCaption, MessageBoxStyle uType);


	public enum MessageBoxStyle
	{
		Ok = 0x00000000,
		OkCancel = 0x00000001,
		AbortIgnoreRetry = 0x00000002,
		YesNoCancel = 0x00000003,
		YesNo = 0x00000004,
		RetryCancel = 0x00000005,
		CancelRetry = 0x00000006,

		Help = 0x00004000,
	}
	public enum MessageBoxResponse
	{
		Ok = 1,
		Cancel = 2,
		Abort = 3,
		Retry = 4,
		Ignore = 5,
		Yes = 6,
		No = 7,
		TryAgain = 10,
		Continue = 11,
	}

	#endregion

	[DllImport(LibName, ExactSpelling = true)]
	public static extern HDC GetDC(HWND hWnd);
	[DllImport(LibName, ExactSpelling = true)]
	public static extern int ReleaseDC(HWND hWnd, HDC hDC);
}
