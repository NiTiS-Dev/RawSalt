using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace RawSalt.Resources;

#if DEBUG

[Obsolete]
public class Registry<KEY, VALUE> : Registry
	where KEY : notnull
{
	private readonly Dictionary<KEY, VALUE> registres;
	public Registry(int initSize)
	{
		registres = new Dictionary<KEY, VALUE>(initSize);
	}
	public bool Register(KEY key, VALUE value)
	{
		registres[key] = value;
		return true;
	}
	public VALUE? Get(KEY key)
	{
		if (registres.TryGetValue(key, out VALUE? value))
		{
			return value;
		}
		return default;
	}
	public bool Forget(KEY key)
	{
		if (registres.ContainsKey(key))
		{
			registres.Remove(key);
			return true;
		}
		return false;
	}
}

[Obsolete]
public class Registry
{
	private protected Registry() { }

	private static readonly Dictionary<(Type, Type), RegistryHost> registries;
	public static bool Register<KEY, VALUE>(KEY key, VALUE value)
		where KEY : notnull
	{
		if (registries.TryGetValue((typeof(KEY), typeof(VALUE)), out RegistryHost host))
		{
			return (host.Registry as Registry<KEY, VALUE>)!.Register(key, value);
		}
		return false;
	}
	public static VALUE? Get<KEY, VALUE>(KEY key)
		where KEY : notnull
	{
		if (registries.TryGetValue((typeof(KEY), typeof(VALUE)), out RegistryHost host))
			return (host.Registry as Registry<KEY, VALUE>)!.Get(key);
		else
			return default;
	}
	public static bool Forget<KEY, VALUE>(KEY key)
		where KEY : notnull
	{
		if (registries.TryGetValue((typeof(KEY), typeof(VALUE)), out RegistryHost host))
		{
			return (host.Registry as Registry<KEY, VALUE>)!.Forget(key);
		}
		return false;
	}
	public static RegistryCallback RegisterRegistry<KEY, VALUE>(object? host, Registry<KEY, VALUE> registry, RegistryOwnerRules rules, bool deleteContent = true)
		where KEY : notnull
	{
		if (registries.TryGetValue((typeof(KEY), typeof(VALUE)), out RegistryHost prevReg))
		{
			if (prevReg.Host is null || prevReg.Host == host)
				goto ALLOW;
			else
				switch (prevReg.Rules)
				{
					case RegistryOwnerRules.DisallowReplace:
						return RegistryCallback.HasNoRightToRemoveRegistry;

					case RegistryOwnerRules.AllowReplaceSaveContent:
						if (deleteContent)
							return RegistryCallback.HasNoRightToRemoveContent;
						else
							goto DUMP_CONTENT;
					case RegistryOwnerRules.AllowReplaceContentIgnore:
						if (deleteContent)
							goto ALLOW;
						else
							goto DUMP_CONTENT;
				}
		}
		else
			goto ALLOW;

		DUMP_CONTENT:
		//TODO move content
		goto ALLOW;
		ALLOW:
		registries[(typeof(KEY), typeof(VALUE))] = new(host, registry, rules);
		return 0;
	}

	static Registry()
	{
		registries = new(1);
	}

	[StructLayout(LayoutKind.Sequential)]
	private readonly struct RegistryHost
	{
		public readonly object? Host;
		public readonly	RegistryOwnerRules Rules;
		public readonly Registry Registry;
		public RegistryHost(object? host, Registry registry, RegistryOwnerRules rules)
		{
			Host = host;
			Rules = rules;
			Registry = registry;
		}
	}
}

public enum RegistryOwnerRules : byte
{
	DisallowReplace = 0,
	AllowReplaceSaveContent = 1,
	AllowReplaceContentIgnore = 2,
}
public enum RegistryCallback : byte
{
	Success = 0,
	HasNoRightToRemoveContent = 1,
	HasNoRightToRemoveRegistry = 2,
}

#endif