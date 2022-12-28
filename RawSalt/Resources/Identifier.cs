using System;
using System.Diagnostics.CodeAnalysis;

namespace RawSalt.Resources;

/// <summary>
/// NiTiS identifier type
/// </summary>
public readonly struct Identifier : IEquatable<Identifier>, IParsable<Identifier>
{
	private readonly string source;
	private readonly string name;

	private Identifier(string source, string name)
	{
		this.name = name;
		this.source = source;
	}
	private static bool ValidateString(string str)
	{
		if (str is null)
			return false;

		static bool ValidateChar(char a)
			=> a switch
			{
				_ when a is >= 'A' and <= 'Z' => true,
				_ when a is >= 'a' and <= 'z' => true,
				_ when a is >= '0' and <= '9' => true,
				'-' or '_' => true,
				_ => false
			};

		for (int i = 0; i < str.Length; i++)
		{
			if (!ValidateChar(str[i]))
				return false;
		}

		return true;
	}
	public static Identifier Parse(string s, IFormatProvider? provider)
	{
		if (TryParse(s, provider, out Identifier a))
		{
			return a;
		}

		throw new ArgumentException("Unable to parse identifier");
	}
	public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Identifier result)
	{
		if (s is null)
		{
			result = default;
			return false;
		}

		int spliterator = s.IndexOf(':');

		if (spliterator is -1)
		{
			result = default;
			return false;
		}

		if (s.LastIndexOf(':') != spliterator)
		{
			result = default;
			return false;
		}

		string source = s.Substring(0, spliterator);

		string name = s.Substring(spliterator);

		return TryCreate(source, name, out result);
	}
	public static Identifier Create(string source, string name)
	{
		if (TryCreate(source, name, out Identifier result))
		{
			return result;
		}

		throw new ArgumentException("Invalid source or name");
	}
	public static bool TryCreate(string source, string name, [NotNullWhen(true)]out Identifier result)
	{
		if (!ValidateString(source) || !ValidateString(name))
		{
			result = default;

			return false;
		}

		result = new(source, name);
		return true;
	}

	public override readonly string ToString()
	{
		if (source is null || name is null)
			return string.Empty;

		int lenSource = source.Length;
		int lenName = name.Length;
		Span<char> abas = stackalloc char[lenName + lenSource + 1];


		source.CopyTo(abas);
		name.CopyTo(abas.Slice(lenSource + 1));

		abas[lenSource] = ':';

		return new(abas);
	}
	public readonly bool Equals(Identifier other)
		=> other.source == this.source
		&& other.name == this.name;
	public override bool Equals(object? obj)
		=> obj is Identifier id && Equals(id);
	public override int GetHashCode()
		=> HashCode.Combine(source, name);

	public static bool operator ==(Identifier left, Identifier right)
		=> left.Equals(right);
	public static bool operator !=(Identifier left, Identifier right)
		=> !left.Equals(right);
}