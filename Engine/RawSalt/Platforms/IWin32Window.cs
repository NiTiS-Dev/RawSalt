namespace RawSalt.Platforms;

public interface IWin32Window
{
	/// <summary>
	/// Window handle.
	/// </summary>
	nint Handle { get; }
	/// <summary>
	/// HInstance.
	/// </summary>
	nint ModuleHandle { get; }
}