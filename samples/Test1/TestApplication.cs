using RawSalt.Resources;
using System;

namespace Test1;
public class TestApplication
{
	private static void Main(string[] args)
	{
		object lockO = new();
		Registry.RegisterRegistry<string, string>(lockO, new(4), RegistryOwnerRules.DisallowReplace);


	}
}
