using System.Collections.Generic;

namespace RawSalt.Resources;

public class ResourceManager
{
	private static Dictionary<string, Dictionary<string, IDedicatedResource>> resources = new(5); 

	public static void Add<T>(IDedicatedResource<T> resource)
		where T : IResourceType
	{

	}
}
