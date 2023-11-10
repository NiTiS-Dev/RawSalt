namespace RawSalt.Memory;

public static class CStringHelper
{
	public static unsafe int ZeroIndex(void* ptr)
		=> ZeroIndex((byte*) ptr);

	public static unsafe int ZeroIndex(byte* ptr)
	{
		int index = 0;
		while (ptr[index] != 0)
			index++;

		return index;
	}
}
