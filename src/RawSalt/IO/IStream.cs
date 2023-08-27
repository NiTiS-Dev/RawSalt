using System;

namespace RawSalt.IO;

public interface IStream : IDisposable
{
	bool EndOfStream { get; }
	nuint Position { get; }
	nuint Length { get; }
}
