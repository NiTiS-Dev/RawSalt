using RawSalt.Logging;

namespace RawSalt.Windowing.Win32;

public static class LaunchConfigurationExtensions
{
	extension(LaunchConfiguration.Builder builder)
	{
		public LaunchConfiguration.Builder AddWin32Platform()
		{
			if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 15063))
			{
				return builder.AddPlatformCandidate(new Win32Platform());
			}

			if (OperatingSystem.IsWindows())
			{
				Log.Error("Failed to add Windows platform. Required minimum Windows version: 10.0.15063");
			}

			return builder;
		}
	}
}