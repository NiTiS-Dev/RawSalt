namespace RawSalt.Input;

[Flags]
public enum KeyModifiers : byte
{
	None = 0,
	Shift = 1 << 0,
	Control = 1 << 1,
	Super = 1 << 2,
	Alt = 1 << 3,
}
