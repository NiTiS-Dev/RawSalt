namespace RawSalt.Platforms;

public abstract class Platform
{
	public virtual WellKnownPlatformApi Api => WellKnownPlatformApi.Unknown;

	public virtual Display GetPrimaryDisplay()
	{
		return GetDisplays()[0];
	}

	public abstract IReadOnlyList<Display> GetDisplays();

	public abstract View CreateView(in WindowConfiguration configuration);
	public abstract bool ReleaseView(View view);
}