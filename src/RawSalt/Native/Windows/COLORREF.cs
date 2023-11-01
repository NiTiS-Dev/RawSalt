namespace RawSalt.Native.Windows;

public struct COLORREF
{
	public uint Value;

	public COLORREF(uint value)
	{
		Value = value;
	}
	public COLORREF(byte r, byte g, byte b)
	{
		Value = (uint)(r | g << 8 | b << 16);
	}
}
