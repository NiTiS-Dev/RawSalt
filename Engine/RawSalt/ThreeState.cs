namespace RawSalt;

/// <summary>
/// Represents an optional bool as a single byte.
/// </summary>
public enum ThreeState : byte
{
	Unknown = 0,
	False = 1,
	True = 2,
}

public static class ThreeStateExtensions
{
	extension(ThreeState threeState)
	{
		public bool IsTrue => threeState == ThreeState.True;
		public bool IsTrueOrUnknown => threeState != ThreeState.False;
		public bool IsFalse => threeState == ThreeState.False;
		public bool IsFalseOrUnknown => threeState != ThreeState.True;

		public static bool operator ==(ThreeState first, bool second)
		{
			return first == (second ? ThreeState.True : ThreeState.False);
		}

		public static bool operator !=(ThreeState first, bool second)
		{
			return !(first == second);
		}
	}
}