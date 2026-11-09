namespace RawSalt.Platforms;

public abstract class Platform : Disposable
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

	/// <summary>
	/// Try to create a window with error message.
	/// </summary>
	/// <param name="message">Error message.</param>
	/// <returns><see langword="true"/> if succeed to create an error window; otherwise <see langword="false"/>.</returns>
	public virtual bool TryCreateErrorMessage(string message) => false;
}