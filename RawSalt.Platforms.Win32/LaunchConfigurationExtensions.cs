namespace RawSalt.Windowing.Win32;

public static class LaunchConfigurationExtensions
{
	extension(LaunchConfiguration.Builder builder)
	{
		public LaunchConfiguration.Builder AddWin32Platform()
		{
			return builder.AddPlatformCandidate(new Win32Platform());
		}
	}
}