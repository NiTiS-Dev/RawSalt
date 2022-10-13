namespace RawSalt.Resources;

public abstract class DedicatedResource<T> : IDedicatedResource<T>
	where T : IResourceType
{
	object IDedicatedResource.GetValue() => GetValue();
	public abstract T GetValue();
	public abstract void DisposeResource();

	public static implicit operator T(DedicatedResource<T> resource)
		=> resource.GetValue();
}
