using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace RawSalt.Resources;

public class Registry<KEY, VALUE> : Registry
	where KEY : notnull
{
	private readonly Dictionary<KEY, VALUE> registres;
	public Registry(int initSize)
	{
		registres = new Dictionary<KEY, VALUE>(initSize);
	}
	public void Register(KEY key, VALUE value)
	{

	}
	public void Forget(KEY key)
	{

	}
}
public class Registry
{
	private protected Registry() { }

	private static readonly Dictionary<(Type, Type), RegistryHost> registries;
	public static VALUE Get<KEY, VALUE>(KEY key) //TODO: Impl
		where KEY : notnull
	{
		return default;
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