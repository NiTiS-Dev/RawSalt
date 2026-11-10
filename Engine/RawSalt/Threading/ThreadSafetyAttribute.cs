namespace RawSalt.Threading;

/// <summary>
/// Documents on which threads the marked type or member may safely be used.
/// A member marked with this attribute overrides the value of its containing type.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event)]
public sealed class ThreadSafetyAttribute(AllowedThread threads) : Attribute
{
	/// <summary>
	/// Threads allowed to use the marked type or member.
	/// </summary>
	public AllowedThread Threads { get; } = threads;
}