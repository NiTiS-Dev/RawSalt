using RawSalt.Input;

namespace RawSalt.Platforms.Win32;

internal struct Win32KeyEvent
{
	public Key Key;
	public bool Pressed;
}

internal struct Win32MouseEvent
{
	public MouseButton Button;
	public bool Pressed;
}