namespace RawSalt.IO;

public interface ISeekable
{
	void Seek(SeekPosition position, nint offset);

	void Seek(SeekPosition position, int offset)
		=> Seek(position, (nint)offset);
}
