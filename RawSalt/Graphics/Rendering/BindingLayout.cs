namespace RawSalt.Graphics.Rendering;

/// <remarks>
/// Must outlive every pipeline created from it.
/// </remarks>
public abstract class BindingLayout : Disposable
{
	// Binding numbers of all dynamic-offset bindings, sorted ascending.
	// Vulkan expects the dynamic offsets array in exactly this order.
	private readonly uint[] _dynamicBindings;

	public BindingEntry[] Bindings { get; }

	public int DynamicOffsetCount => _dynamicBindings.Length;

	protected BindingLayout(ReadOnlySpan<BindingEntry> entries)
	{
		Bindings = [..entries];
		Validate(Bindings);

		_dynamicBindings = Bindings
			.Where(static e => IsDynamic(in e))
			.Select(static e => e.Binding)
			.Order().ToArray();
	}

	public int GetDynamicOffsetIndex(uint binding)
	{
		int index = Array.BinarySearch(_dynamicBindings, binding);
		if (index < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(binding),
				$"Binding {binding} is not a uniform buffer in this layout.");
		}
		return index;
	}

	private static bool IsDynamic(in BindingEntry entry)
	{
		return entry.Type == BindingType.UniformBuffer;
	}

	private static void Validate(BindingEntry[] entries)
	{
		HashSet<uint> seen = [];
		foreach (BindingEntry e in entries)
		{
			if (!seen.Add(e.Binding))
			{
				throw new ArgumentException($"Duplicate binding {e.Binding}.", nameof(entries));
			}
			if (e.Stages == 0)
			{
				throw new ArgumentException($"Binding {e.Binding} is not visible to any shader stage.", nameof(entries));
			}
			if (e.Type == BindingType.UniformBuffer && e.Size == 0)
			{
				throw new ArgumentException($"Binding {e.Binding}: uniform buffer size must be non-zero.", nameof(entries));
			}
		}
	}
}