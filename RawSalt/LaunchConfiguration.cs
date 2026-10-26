using System.Collections.Immutable;
using RawSalt.Graphics;

namespace RawSalt;

public sealed partial class LaunchConfiguration
{
	public ImmutableArray<GraphicsBackend> GraphicsBackends { get; }

	private LaunchConfiguration()
	{
		GraphicsBackends = [];
	}
}