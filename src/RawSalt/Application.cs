using RawSalt.Logging;
using RawSalt.Mathematics.Geometry;
using System;

namespace RawSalt;

public abstract unsafe partial class Application : IDisposable
{
	/// <summary>
	/// Application logger.
	/// </summary>
	protected static readonly Logger logger = Logger.CreateLogger<Application>();
	
	/// <summary>
	/// Application identificator.
	/// </summary>
	public readonly string Id;

	/// <summary>
	/// Defines Size of view/window.
	/// </summary>
	public abstract UVec2 Size { get; set; }

	/// <summary>
	/// Determines if it is possible to resize the view/window.
	/// </summary>
	public abstract bool IsResizeAllow { get; }

	/// <summary>
	/// Determines whether the user can resize the view/window.
	/// </summary>
	public abstract bool IsResizable { get; set; }

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
	public abstract void Initialize();

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