using RawSalt.Logging;

namespace RawSalt.Windowing.Xlib;

public static class LaunchConfigurationExtensions
{
	extension(LaunchConfiguration.Builder builder)
	{
		public LaunchConfiguration.Builder AddXlibPlatform()
		{
			if (!OperatingSystem.IsLinux() && !OperatingSystem.IsFreeBSD())
			{
				return builder;
			}

			if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DISPLAY")))
			{
				return builder;
			}

			if (!XlibPlatform.IsAvailable())
			{
				Log.Warning("Failed to add Xlib platform: unable to open X display.");
				return builder;
			}

			return builder.AddPlatformCandidate(app => new XlibPlatform(app));
		}
	}
}
