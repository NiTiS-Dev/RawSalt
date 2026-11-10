namespace RawSalt.Platforms;

/// <summary>
/// View backed by a native window that can be used by graphics backends to create a render surface.
/// </summary>
public interface INativeWindow
{
	NativeWindowHandle NativeHandle { get; }
}
