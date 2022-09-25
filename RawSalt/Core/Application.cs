namespace RawSalt.Core;

public class Application
{
	public PlatformType Platform { get; }

	public Application(PlatformType platform)
	{
		Platform = platform;
	}
}
