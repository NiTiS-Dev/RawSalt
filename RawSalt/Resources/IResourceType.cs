namespace RawSalt.Resources;

public interface IResourceType
{
	/// <summary>
	/// Called when resource is unused
	/// </summary>
	void DisposeResource();
	static abstract string ResourceType { get; }
}
