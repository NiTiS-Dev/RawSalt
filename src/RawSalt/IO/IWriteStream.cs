using System;

namespace RawSalt.IO;

public interface IWriteStream : IStream
{
	void Write(byte[] buffer, int offset, int count);
	void Write(ReadOnlySpan<byte> buffer);
	virtual void Write(byte value)
	{
		Write(stackalloc byte[] { value });
	}

	// TODO: Add more write methods
}