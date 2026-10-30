namespace RawSalt.Platforms;

public abstract class Platform : IDisposable
{
	public virtual WellKnownPlatformApi Api => WellKnownPlatformApi.Unknown;
	public Application Application { get; }

	protected Platform(Application application)
	{
		Application = application;
	}

	public virtual Display GetPrimaryDisplay()
	{
		return GetDisplays()[0];
	}

	public abstract IReadOnlyList<Display> GetDisplays();

	public abstract View CreateView(in WindowConfiguration configuration);
	public abstract bool ReleaseView(View view);

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected abstract void Dispose(bool disposing);

	~Platform()
	{
		Dispose(false);
	}
}