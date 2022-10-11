namespace RawSalt.Resources;

public sealed class SimpleResource<T> : DedicatedResource<T>
	where T : IResourceType
{
	private readonly T value;
	public SimpleResource(T value)
	{
		this.value = value;
	}
	public override T GetValue()
	{
		return value;
	}
	public override void Unuse() { }
}
