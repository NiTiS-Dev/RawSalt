using System;

namespace RawSalt.App;

public abstract class Application : IEquatable<Application>
{

	//public abstract FileSystem FS { get; protected set; } 


	public override bool Equals(object? obj)
		=> obj is Application other && Equals(other);
	public bool Equals(Application? other)
		=> ReferenceEquals(this, other);

	public override string ToString()
		=> "Application";
	public override int GetHashCode()
		=> 0;
}