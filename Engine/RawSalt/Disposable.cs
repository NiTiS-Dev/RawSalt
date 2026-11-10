namespace RawSalt;

/// <summary>
/// Base class that implements the disposable pattern.
/// </summary>
public abstract class Disposable : IDisposable
{
	/// <summary>
	/// Returns a boolean value indicating whether this instance has been disposed or not.
	/// </summary>
	public bool IsDisposed { get; private set; }

	/// <summary>
	/// Throws <see cref="ObjectDisposedException"/> when this instance has been disposed.
	/// </summary>
	/// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
	protected void ThrowIfDisposed()
	{
		ObjectDisposedException.ThrowIf(IsDisposed, this);
	}

	/// <inheritdoc/>
	public void Dispose()
	{
		if (IsDisposed)
		{
			return;
		}

		IsDisposed = true;

		GC.SuppressFinalize(this);
		Dispose(true);
	}

	/// <summary>
	/// Releases the resources used by this instance.
	/// </summary>
	/// <param name="disposing">
	/// <see langword="true"/> when called from <see cref="Dispose()"/>;
	/// <see langword="false"/> when called from the finalizer.
	/// </param>
	protected virtual void Dispose(bool disposing)
	{
	}
}