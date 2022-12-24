using RawSalt.Structs;
using System;
using System.Collections;

namespace RawSalt.Resources;

public readonly struct Identifier : IEquatable<Identifier>, IStructuralEquatable
{
	public static readonly char Separator = ':';
	private readonly string idSpace;
	private readonly string idName;
	public readonly string Space => idSpace;
	public readonly string Name => idName;
	internal Identifier(string idSpace, string idName, ZeroArgument _)
	{
		this.idSpace = idSpace;
		this.idName = idName;
	}
	public Identifier(string idSpace, string idName)
		: this(
			  Verify(idSpace) ? idSpace : throw new ArgumentException("Invalid idSpace format"),
			  Verify(idName) ? idName : throw new ArgumentException("Invalid idName format"),
			  default
			  )
	{ }
	public readonly bool Equals(Identifier other)
		=> this.idName  == other.idName
		&& this.idSpace == other.idSpace;
	public override readonly bool Equals(object? obj)
		=> obj is Identifier identifier && Equals(identifier);
	public readonly bool Equals(object? other, IEqualityComparer comparer)
		=> comparer.Equals(this, other);
	public readonly int GetHashCode(IEqualityComparer comparer)
		=> comparer.GetHashCode(this);
	public override readonly int GetHashCode()
		=> HashCode.Combine(idSpace, idName);

	public override readonly string ToString()
		=> $"{idSpace}{Separator}{idName}";

	public static bool Verify(string nameOrSpace)
	{
		static bool IsValidChar(char c) => c switch
		{
			_ when c is >= '0' and <= '9' => true,
			_ when c is >= 'A' and <= 'Z' => true,
			_ when c is >= 'a' and <= 'z' => true,
			'_' or '-' or '.' => true,
			_ => false
		};

		for (int i = 0; i < nameOrSpace.Length; i++)
		{
			if (!IsValidChar(nameOrSpace[i]))
				return false;
		}

		return true;
	}

	public static bool operator ==(Identifier left, Identifier right)
		=> left.Equals(right);
	public static bool operator !=(Identifier left, Identifier right)
		=> !left.Equals(right);
}