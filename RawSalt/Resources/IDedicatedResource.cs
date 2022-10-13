namespace RawSalt.Resources;

public interface IDedicatedResource
{
	void DisposeResource();
	object GetValue();
}
public interface IDedicatedResource<out T> : IDedicatedResource
	where T : IResourceType
{
	new T GetValue();
}