namespace RawSalt;

public sealed class LaunchConfiguration
{
	public static Builder CreateBuilder()
	{
		return new Builder();
	}

	public class Builder
	{
		internal Builder()
		{ }

		public LaunchConfiguration Build()
		{
			return new LaunchConfiguration();
		}
	}
}