using System.Collections.Immutable;
using RawSalt.Graphics;
using RawSalt.Platforms;

namespace RawSalt;

public sealed partial class LaunchConfiguration
{
	public ImmutableArray<Platform> Platforms { get; }
	public ImmutableArray<GraphicsBackend> GraphicsBackends { get; }

	private LaunchConfiguration(ImmutableArray<Platform> platforms, ImmutableArray<GraphicsBackend> graphicsBackends)
	{
		Platforms = platforms;
		GraphicsBackends = graphicsBackends;
	}
}