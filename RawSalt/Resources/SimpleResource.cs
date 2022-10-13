namespace RawSalt.Resources;

public sealed class SimpleResource<T> : DedicatedResource<T>
	where T : IResourceType
{
	private T? value;
	public SimpleResource(T value)
	{
		this.value = value;
	}
	public override void DisposeResource()
	{
		value?.DisposeResource();
		value = default;
	}

	public override T GetValue()
	{
		return value;
	}
}
