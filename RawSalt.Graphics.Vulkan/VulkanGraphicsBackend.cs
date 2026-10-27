namespace RawSalt.Graphics.Vulkan;

public class VulkanGraphicsBackend : GraphicsBackend
{
	public override IReadOnlyList<Adapter> GetAdapters()
	{
		throw new NotImplementedException();
	}

	public override GraphicsContext CreateContext(Adapter device)
	{
		throw new NotImplementedException();
	}
}