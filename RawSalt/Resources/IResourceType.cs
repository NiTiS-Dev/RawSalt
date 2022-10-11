namespace RawSalt.Resources;

public interface IResourceType
{
	/// <summary>
	/// Called when resource is unused
	/// </summary>
	void DisposeResoruce();
	static abstract string ResourceType { get; }
}
