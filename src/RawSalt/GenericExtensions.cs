namespace RawSalt;

public static class GenericExtensions
{
	public static T Default<T>(this T? value, T ifNull)
		where T : class
	{
		return value ?? ifNull;
	}
	public static T Default<T>(this T? value, T ifNull)
		where T : struct
	{
		return value ?? ifNull;
	}
}