using System;

namespace RawSalt.IO;

public interface IStream : IDisposable
{
	bool IsEndOfStream { get; }
	nuint Position { get; }
	nuint Length { get; }
}
