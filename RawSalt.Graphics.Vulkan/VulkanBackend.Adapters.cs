namespace RawSalt.Graphics.Vulkan;

public partial class VulkanBackend
{
	private List<VulkanAdapter>? _adapters;

	public override IReadOnlyList<VulkanAdapter> GetAdapters()
	{
		if (_adapters == null)
		{
			var adapters = CreateAdapters();
			Interlocked.CompareExchange(ref _adapters, adapters, null);
		}

		return _adapters;

		List<VulkanAdapter> CreateAdapters()
		{
			throw new NotImplementedException();
		}
	}
}