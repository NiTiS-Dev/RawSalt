using System.Collections.Generic;

namespace RawSalt.Resources;

public interface IRegistry<Resource> : IRegistryWritter<Resource>, IRegistryReader<Resource>
{
	/// <summary>
	/// Is unregistration possible
	/// </summary>
	bool IsStrict { get; }
}

public interface IRegistryWritter<Resource>
{
	/// <summary>
	/// Registrate <see cref="Resource"/> by <paramref name="key"/>
	/// </summary>
	/// <param name="key">Resource key</param>
	/// <param name="resource">Resource itself</param>
	/// <returns><see langword="true"/> if registry is success, otherwise returns <see langword="false"/></returns>
	bool Registry(RegistryKey<Resource> key, Resource resource);
	/// <summary>
	/// Unregistrate <see cref="Resource"/> by <paramref name="key"/>
	/// </summary>
	/// <param name="key">Resource key</param>
	/// <returns><see langword="true"/> if unregistry is success, otherwise returns <see langword="false"/></returns>
	bool Unregistry(Identifier key);
	/// <summary>
	/// Unregistrate all entries
	/// </summary>
	/// <returns><see langword="true"/> if unregistry is success, otherwise returns <see langword="false"/></returns>
	bool UnregistryAll();
}
public interface IRegistryReader<Resource>
{
	/// <summary>
	/// Checks <paramref name="key"/> is registred or not
	/// </summary>
	/// <param name="key">Key to found</param>
	/// <returns><see langword="true"/> if founded, otherwise returns <see langword="false"/></returns>
	bool IsRegistred(Identifier key);
	/// <summary>
	/// Returns registred resource by <paramref name="key"/>
	/// </summary>
	/// <param name="key">Key to found</param>
	/// <returns>Resource linked to <paramref name="key"/><br/>Returns default if key not found</returns>
	RegistryKey<Resource>? Get(Identifier key);
	IEnumerable<RegistryKey<Resource>> GetAll();
}