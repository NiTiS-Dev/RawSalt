namespace RawSalt.Graphics.Vulkan;

public static class LaunchConfigurationExtensions
{
	extension(LaunchConfiguration.Builder configBuilder)
	{
		public LaunchConfiguration.Builder AddVulkanBackend()
		{
			//configBuilder.AddPlatformCandidate();
			return configBuilder;
		}
	}
}