using RawSalt.Logging;

namespace RawSalt.Windowing.Wayland;

public static class LaunchConfigurationExtensions
{
	extension(LaunchConfiguration.Builder builder)
	{
		public LaunchConfiguration.Builder AddWaylandPlatform()
		{
			if (!OperatingSystem.IsLinux() && !OperatingSystem.IsFreeBSD())
			{
				return builder;
			}

			if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WAYLAND_DISPLAY")))
			{
				return builder;
			}

			if (!WaylandPlatform.IsAvailable())
			{
				Log.Warning("Failed to add Wayland platform: unable to connect to the compositor.");
				return builder;
			}

			return builder.AddPlatformCandidate(app => new WaylandPlatform(app));
		}
	}
}
