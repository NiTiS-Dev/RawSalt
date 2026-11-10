namespace RawSalt.Threading;

/// <summary>
/// Threads that are allowed to use a type or member marked with <see cref="ThreadSafetyAttribute"/>.
/// </summary>
[Flags]
public enum AllowedThread
{
	/// <summary>
	/// Not usable on any thread.
	/// </summary>
	None = 0,

	/// <summary>
	/// The application thread: runs the update loop and pumps window events.
	/// </summary>
	Main = 1 << 0,

	/// <summary>
	/// The dedicated render thread.
	/// </summary>
	Render = 1 << 1,
}