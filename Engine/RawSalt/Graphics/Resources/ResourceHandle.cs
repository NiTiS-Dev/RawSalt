using RawSalt.Threading;

namespace RawSalt.Graphics.Resources;

/// <summary>
/// Generational handle to a pooled GPU resource.
/// </summary>
/// <remarks>
/// The generation changes whenever the slot is reused, so a handle to a released resource never resolves.
/// </remarks>
[ThreadSafety(AllowedThread.Main | AllowedThread.Render)]
public readonly record struct ResourceHandle(int Index, int Generation)
{
	public bool IsValid => Index >= 0 && Generation != 0;
}