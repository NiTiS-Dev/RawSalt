using RawSalt.Logging;

namespace RawSalt.Platforms.Win32;

public static class LaunchConfigurationExtensions
{
	extension(LaunchConfiguration.Builder builder)
	{
		public LaunchConfiguration.Builder AddWin32Platform()
		{
			if (OperatingSystem.IsWindowsVersionAtLeast(10, 0))
			{
				// Roslyn, you are baka 💢
#pragma warning disable CA1416
				return builder.AddPlatformCandidate(app => new Win32Platform(app));
#pragma warning restore CA1416
			}

			if (OperatingSystem.IsWindows())
			{
				Log.Error("Failed to add Windows platform. Required Windows version: 10 or newer.");
			}

			return builder;
		}
	}
}