using System;
using System.Diagnostics.CodeAnalysis;

namespace RawSalt.Resources;

public struct RegistryKey<Resource> : IEquatable<RegistryKey<Resource>>, IEquatable<Identifier>
{
	public readonly Identifier Key;
	public Resource? Value;

	public RegistryKey(Identifier id)
	{
		Key = id;
		Value = default;
	}
	public override readonly int GetHashCode()
		=> Key.GetHashCode();
	public override readonly bool Equals([NotNullWhen(true)] object? obj)
		=> obj is RegistryKey<Resource> regkey && Equals(regkey)
		|| obj is Identifier id && Equals(id);
	public readonly bool Equals(Identifier other)
		=> Key.Equals(other);
	public readonly bool Equals(RegistryKey<Resource> other)
		=> Key.Equals(other);
	public override readonly string ToString()
		=> $"[{Key} == {Value?.ToString() ?? "null"}]";

	public static implicit operator Identifier(RegistryKey<Resource> key)
		=> key.Key;
}
