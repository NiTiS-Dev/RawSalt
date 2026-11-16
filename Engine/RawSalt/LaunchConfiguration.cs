using System.Collections.Immutable;
using RawSalt.Graphics;
using RawSalt.Platforms;

namespace RawSalt;

public sealed partial class LaunchConfiguration
{
	public ImmutableArray<Func<Application, Platform>> Platforms { get; }
	public ImmutableArray<Func<Application, GraphicsBackend>> GraphicsBackends { get; }

	private LaunchConfiguration(ImmutableArray<Func<Application, Platform>> platforms,
		ImmutableArray<Func<Application, GraphicsBackend>> graphicsBackends)
	{
		Platforms = platforms;
		GraphicsBackends = graphicsBackends;
	}
}