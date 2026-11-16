namespace RawSalt;

public static class EnvironmentExtensions
{
	extension(Environment)
	{
		public static ThreeState GetEnvironmentVariableBool(string name)
		{
			string? value = Environment.GetEnvironmentVariable(name);

			if (value == null) return ThreeState.Unknown;

			return value.ToLowerInvariant() switch
			{
				"0" or "false" or "disable" => ThreeState.False,
				_ => ThreeState.True
			};
		}
	}
}