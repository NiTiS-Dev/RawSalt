using NiTiS.Collections.Generic;
using System.Collections.Generic;

namespace RawSalt.Resources;

public sealed class Registry<Resource> : StandardRegistry<Resource>
{
	private readonly bool isStrict;
	public override bool IsStrict
		=> isStrict;
	public Registry() : this(new(), false) { }
	public Registry(bool isStrict) : this(new(), isStrict) { }
	public Registry(Dictionary<Identifier, RegistryKey<Resource>> entries, bool isStrict) : base(entries)
	{
		this.isStrict = isStrict;
	}
}

public static class Registry
{
	public static bool Reg<Resource>(IRegistryWritter<Resource> registry, RegistryKey<Resource> key, Resource resource)
		=> registry?.Registry(key, resource) ?? false;
	public static bool Exists<Resource>(IRegistryReader<Resource> registry, Identifier key)
		=> registry?.IsRegistred(key) ?? false;
	public static bool Unreg<Resource>(IRegistryWritter<Resource> registry, Identifier key)
		=> registry?.Unregistry(key) ?? false;
	public static bool UnregAll<Resource>(IRegistryWritter<Resource> registry)
		=> registry?.UnregistryAll() ?? false;
	public static RegistryKey<Resource>? Get<Resource>(IRegistryReader<Resource> registry, Identifier key)
		=> registry?.Get(key) ?? default;
	public static IEnumerable<RegistryKey<Resource>> GetAll<Resource>(IRegistryReader<Resource> registry)
		=> registry?.GetAll() ?? new Empty<RegistryKey<Resource>>();
}