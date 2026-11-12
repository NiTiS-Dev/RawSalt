namespace RawSalt.Graphics;

/// <summary>
/// Dedicated memory buffer.
/// </summary>
public abstract class Buffer : Disposable
{
	/// <summary>
	/// Usable size of buffer.
	/// </summary>
	public abstract ulong Size { get; }
}