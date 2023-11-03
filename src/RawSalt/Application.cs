using RawSalt.Logging;
using System;

namespace RawSalt;

public abstract unsafe partial class Application : IDisposable
{
	/// <summary>
	/// Application logger.
	/// </summary>
	protected static readonly Logger logger = Logger.CreateLogger<Application>();
	
	public readonly string Id;

	protected Application(string applicationId)
	{
		if (string.IsNullOrWhiteSpace(applicationId))
			throw new ArgumentException("Application id must have at least one non-space character.", nameof(applicationId));

		if (applicationId.IndexOf(' ') != -1)
			throw new ArgumentException("Application id must have no spaces.", nameof(applicationId));
		Id = applicationId;
	}

	/// <summary>
	/// Initializes application.
	/// </summary>
	public abstract void Setup();

	/// <summary>
	/// Launch application life cycle.
	/// </summary>
	public abstract void Run();

	/// <summary>
	/// Disposes application resources.
	/// </summary>
	/// <remarks>
	/// Call this only by application, during termination.
	/// </remarks>
	public virtual void Dispose() { }
}