using System;

namespace RawSalt.IO;

public interface IReadStream : IStream
{
	void Read(byte[] buffer, int offset, int count);
	void Read(Span<byte> buffer);
	virtual byte ReadByte()
	{
		Span<byte> buffer = stackalloc byte[1];
		buffer[0] = ReadByte();
		return buffer[0];
	}
	
	// TODO: Add more read methods
}