namespace RawSalt.Graphics.Drawing;

public class PixelFormatNotSupportedException : Exception
{
	public PixelFormat RequestedFormat { get; }

	public PixelFormatNotSupportedException()
	{
		RequestedFormat = PixelFormat.Unknown;
	}

	public PixelFormatNotSupportedException(PixelFormat requestedFormat)
		: base($"Requested pixel format {requestedFormat} is not supported by this operation.")
	{
		RequestedFormat = requestedFormat;
	}

	public PixelFormatNotSupportedException(PixelFormat requestedFormat, string? message) :
		base(message)
	{
		RequestedFormat = requestedFormat;
	}

	public PixelFormatNotSupportedException(PixelFormat requestedFormat, string? message, Exception? innerException) :
		base(message, innerException)
	{
		RequestedFormat = requestedFormat;
	}
}