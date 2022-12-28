using System;
using System.Collections.Generic;

namespace RawSalt.Resources;

public abstract class StandardRegistry<Resource> : IRegistry<Resource>
{
	protected Dictionary<Identifier, RegistryKey<Resource>> entries;
	public abstract bool IsStrict { get; }
	protected StandardRegistry(Dictionary<Identifier, RegistryKey<Resource>> entries)
	{
		this.entries = entries;
	}

	public virtual RegistryKey<Resource>? Get(Identifier key)
	{
		if (entries.TryGetValue(key, out RegistryKey<Resource> retusa))
		{
			return retusa;
		}

		return null;
	}
	public virtual IEnumerable<RegistryKey<Resource>> GetAll()
		=> entries.Values;
	public virtual bool IsRegistred(Identifier key)
		=> entries.ContainsKey(key);
	public virtual bool Registry(RegistryKey<Resource> key, Resource resource)
	{
		if (entries.ContainsKey(key))
			return false;

		key.Value = resource;
		entries[key] = key;
		return true;
	}
	public virtual bool Unregistry(Identifier key)
	{
		if (IsStrict)
			return false;

		return entries.Remove(key);
	}
	public virtual bool UnregistryAll()
	{
		if (IsStrict)
			return false;

		entries.Clear();
		return true;
	}
}
