namespace RawSalt;

public interface IApproximatelyEquatable<T, in TTolerance> : IEquatable<T>
	where T : allows ref struct
	where TTolerance : allows ref struct
{
	bool Equals(T? other, TTolerance tolerance);
}