using System.Collections.Immutable;
using CommunityToolkit.Diagnostics;
using RawSalt.Graphics;
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
		private readonly HashSet<GraphicsBackend> _backends = [];

		internal Builder()
		{ }

		public Builder AddPlatformCandidate(Platform platform)
		{
			Guard.IsNotNull(platform);

			_platforms.Add(platform);
			return this;
		}

		public Builder AddGraphicsBackendCandidate(GraphicsBackend backend)
		{
			Guard.IsNotNull(backend);

			_backends.Add(backend);
			return this;
		}

		public LaunchConfiguration Build()
		{
			return new LaunchConfiguration([.._platforms], [.._backends]);
		}
	}
}