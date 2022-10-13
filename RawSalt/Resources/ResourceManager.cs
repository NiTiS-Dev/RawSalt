using System.Collections.Generic;

namespace RawSalt.Resources;

public class ResourceManager
{
	private Dictionary<string, Dictionary<string, IDedicatedResource>> resources = new(16); 
	public void Register<T>(T resource)
		where T : IIdentity, IResourceType
		=> Register(resource.Identity, resource);
	public void Register<T>(string key, T resource)
		where T : IResourceType
		=> Register(key, new SimpleResource<T>(resource));
	public void Register<T>(string key, IDedicatedResource<T> resource)
		where T : IResourceType
	{
		Validate(T.ResourceType);
		resources[T.ResourceType][key] = resource;
	}
	public bool Unregister(string resourceKey, string key)
	{
		Validate(resourceKey);
		return resources[resourceKey].Remove(key);
	}
	public bool Unregister<T>(string key)
		where T : IResourceType
	{
		Validate(T.ResourceType);
		return resources[T.ResourceType].Remove(key);
	}
	public IDedicatedResource<T>? Get<T>(string key)
		where T : IResourceType
	{
		if (!resources.ContainsKey(T.ResourceType))
			return default!;

		Dictionary<string, IDedicatedResource> localResources = resources[T.ResourceType];

		if (localResources.TryGetValue(key, out IDedicatedResource resource))
		{
			return (IDedicatedResource<T>)resource;
		}

		return default!;
	}
	public T? GetValue<T>(string key)
		where T : IResourceType
	{
		if (!resources.ContainsKey(T.ResourceType))
			return default!;

		Dictionary<string, IDedicatedResource> localResources = resources[T.ResourceType];

		if (localResources.TryGetValue(key, out IDedicatedResource resource))
		{
			return ((IDedicatedResource<T>)resource).GetValue();
		}

		return default!;
	}
	private void Validate(string resourceCode)
	{
		if (!resources.ContainsKey(resourceCode))
			resources[resourceCode] = new(16);
	}
	/// <summary>
	/// <b>WARNING</b>: This action is irreversible, all resources of type <typeparamref name="T"/> will be deleted
	/// </summary>
	public void Clear(string resourceKey)
	{
		if (!resources.ContainsKey(resourceKey))
			return;

		foreach (Dictionary<string, IDedicatedResource> resourceType in resources.Values)
		{
			foreach (IDedicatedResource resource in resourceType.Values)
			{
				resource.DisposeResource();
			}
		}
		resources.Clear();
	}
	/// <summary>
	/// <b>WARNING</b>: This action is irreversible, all resources of type <typeparamref name="T"/> will be deleted
	/// </summary>
	public void Clear<T>()
		where T : IResourceType
	{
		if (!resources.ContainsKey(T.ResourceType))
			return;

		foreach (Dictionary<string, IDedicatedResource> resourceType in resources.Values)
		{
			foreach (IDedicatedResource resource in resourceType.Values)
			{
				resource.DisposeResource();
			}
		}
		resources.Clear();
	}
	/// <summary>
	/// <b>WARNING</b>: This action is irreversible, all resources will be deleted
	/// </summary>
	public void Clear()
	{
		foreach (Dictionary<string, IDedicatedResource> resourceType in resources.Values)
		{
			foreach (IDedicatedResource resource in resourceType.Values)
			{
				resource.DisposeResource();
			}
		}
		resources.Clear();
	}
}
