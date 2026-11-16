namespace RawSalt.Graphics.Vulkan;

public static class LaunchConfigurationExtensions
{
	extension(LaunchConfiguration.Builder configBuilder)
	{
		public LaunchConfiguration.Builder AddVulkanBackend()
		{
			configBuilder.AddGraphicsBackendCandidate(app => new VulkanBackend(app));
			return configBuilder;
		}
	}
}