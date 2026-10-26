using System.Collections.Immutable;
using CommunityToolkit.Diagnostics;
using RawSalt.Platforms;

namespace RawSalt;

public partial class LaunchConfiguration
{
	public static Builder CreateBuilder()
	{
		return new Builder();
	}

	public class Builder
	{
		private readonly HashSet<Platform> _platforms = [];

		internal Builder()
		{ }

		public Builder AddPlatformCandidate(Platform platform)
		{
			Guard.IsNotNull(platform);

			_platforms.Add(platform);
			return this;
		}

		public LaunchConfiguration Build()
		{
			return new LaunchConfiguration([.._platforms], []);
		}
	}
}