namespace RawSalt.Logging;

public interface ILoggerSink
{
	/// <summary>
	/// Write message to the sink.
	/// </summary>
	/// <remarks>
	/// This method very likely is not called from main thread.
	/// </remarks>
	/// <param name="message">The message.</param>
	void Write(LogMessage message);

	void Flush();
}