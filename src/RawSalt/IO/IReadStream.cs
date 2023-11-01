using System;
using System.Buffers.Binary;
using System.Text;

namespace RawSalt.IO;

public interface IReadStream : IStream
{
	/// <summary>
	/// When overridden in a derived class, reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
	/// </summary>
	/// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between <paramref name="offset"/> and (<paramref name="offset"/> + <paramref name="count"/> - 1) replaced by the bytes read from the current source.</param>
	/// <param name="offset">The zero-based byte offset in <paramref name="buffer"/> at which to begin storing the data read from the current stream.</param>
	/// <param name="count">The maximum number of bytes to be read from the current stream.</param>
	/// <returns>The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if <paramref name="count"/> is 0 or the end of the stream has been reached.</returns>
	nuint Read(byte[] buffer, nint offset, nint count);

	/// <inheritdoc cref="IReadStream.Read(byte[], nint, nint)"/>
	nuint Read(byte[] buffer, nint offset)
		=> Read(buffer, offset, buffer.Length - offset);

	/// <inheritdoc cref="IReadStream.Read(byte[], nint, nint)"/>
	nuint Read(byte[] buffer)
		=> Read(buffer, 0, buffer.Length);

	/// <inheritdoc cref="IReadStream.Read(byte[], nint, nint)"/>
	/// <param name="buffer">A region of memory. When this method returns, the contents of this region are replaced by the bytes read from the current source.</param>
	/// <returns>The total number of bytes read into the buffer. This can be less than the size of the buffer if that many bytes are not currently available, or zero (0) if the buffer's length is zero or the end of the stream has been reached.</returns>
	nuint Read(Span<byte> buffer);
	virtual byte ReadByte()
	{
		Span<byte> buffer = stackalloc byte[1];
		Read(buffer);
		return buffer[0];
	}
	virtual sbyte ReadSByte()
	{
		Span<sbyte> buffer = stackalloc sbyte[1];
		Read(SpanMarshal.FastCast<sbyte, byte>(buffer));
		return buffer[0];
	}
	virtual ushort ReadUInt16()
	{
		Span<byte> buffer = stackalloc byte[2];
		Read(buffer);
		return BinaryPrimitives.ReadUInt16LittleEndian(buffer);
	}
	virtual short ReadInt16()
	{
		Span<byte> buffer = stackalloc byte[2];
		Read(buffer);
		return BinaryPrimitives.ReadInt16LittleEndian(buffer);
	}
	virtual uint ReadUInt32()
	{
		Span<byte> buffer = stackalloc byte[4];
		Read(buffer);
		return BinaryPrimitives.ReadUInt32LittleEndian(buffer);
	}
	virtual int ReadInt32()
	{
		Span<byte> buffer = stackalloc byte[4];
		Read(buffer);
		return BinaryPrimitives.ReadInt32LittleEndian(buffer);
	}
	virtual ulong ReadUInt64()
	{
		Span<byte> buffer = stackalloc byte[8];
		Read(buffer);
		return BinaryPrimitives.ReadUInt64LittleEndian(buffer);
	}
	virtual long ReadInt64()
	{
		Span<byte> buffer = stackalloc byte[8];
		Read(buffer);
		return BinaryPrimitives.ReadInt64LittleEndian(buffer);
	}
	virtual string? ReadString(Encoding encoding)
	{
		nuint pos = this.Position;

		int len = ReadInt32();
		Span<byte> buffer = stackalloc byte[len];
		
		if (Read(buffer) == (nuint)len)
		{
			return encoding.GetString(buffer);
		}
		else
		{
			if (this is ISeekable seekable)
			{
				seekable.Seek(SeekPosition.Begin, (nint)pos);
			}
			return null;
		}
	}
}