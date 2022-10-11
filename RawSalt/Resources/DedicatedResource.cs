namespace RawSalt.Resources;

public abstract class DedicatedResource<T> : IDedicatedResource<T>
	where T : IResourceType
{
	object IDedicatedResource.GetValue() => GetValue();
	public virtual T GetValue()
		=> default!;
	public abstract void Unuse();

	public static implicit operator T(DedicatedResource<T> resource)
		=> resource.GetValue();
}
