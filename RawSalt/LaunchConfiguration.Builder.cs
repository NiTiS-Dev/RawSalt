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
		private readonly HashSet<Func<Application, Platform>> _platforms = [];
		private readonly HashSet<Func<Application, GraphicsBackend>> _backends = [];

		internal Builder()
		{ }

		public Builder AddPlatformCandidate(Func<Application, Platform> platform)
		{
			Guard.IsNotNull(platform);

			_platforms.Add(platform);
			return this;
		}

		public Builder AddGraphicsBackendCandidate(Func<Application, GraphicsBackend> backend)
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