namespace RawSalt.Resources;

public interface IDedicatedResource
{
	object GetValue();
}
public interface IDedicatedResource<out T> : IDedicatedResource
	where T : IResourceType
{
	new T GetValue();
}