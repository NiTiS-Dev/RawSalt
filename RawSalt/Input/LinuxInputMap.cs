namespace RawSalt.Input;

/// <summary>
/// Maps Linux evdev key codes to <see cref="Key"/> values.
/// </summary>
/// <remarks>
/// The values match GLFW key codes, which <see cref="Key"/> is based on.
/// X11 key codes on Linux are evdev codes plus 8, so X11 platforms should subtract 8 before mapping.
/// </remarks>
public static class LinuxKeyMap
{
	public static bool TryGetKey(int evdevCode, out Key key)
	{
		switch (evdevCode)
		{
			case 1: key = Key.Escape; return true;
			case 2: key = Key.Digit1; return true;
			case 3: key = Key.Digit2; return true;
			case 4: key = Key.Digit3; return true;
			case 5: key = Key.Digit4; return true;
			case 6: key = Key.Digit5; return true;
			case 7: key = Key.Digit6; return true;
			case 8: key = Key.Digit7; return true;
			case 9: key = Key.Digit8; return true;
			case 10: key = Key.Digit9; return true;
			case 11: key = Key.Digit0; return true;
			case 12: key = Key.Minus; return true;
			case 13: key = Key.Equal; return true;
			case 14: key = Key.Backspace; return true;
			case 15: key = Key.Tab; return true;
			case 16: key = Key.Q; return true;
			case 17: key = Key.W; return true;
			case 18: key = Key.E; return true;
			case 19: key = Key.R; return true;
			case 20: key = Key.T; return true;
			case 21: key = Key.Y; return true;
			case 22: key = Key.U; return true;
			case 23: key = Key.I; return true;
			case 24: key = Key.O; return true;
			case 25: key = Key.P; return true;
			case 26: key = Key.LeftBracket; return true;
			case 27: key = Key.RightBracket; return true;
			case 28: key = Key.Enter; return true;
			case 29: key = Key.ControlLeft; return true;
			case 30: key = Key.A; return true;
			case 31: key = Key.S; return true;
			case 32: key = Key.D; return true;
			case 33: key = Key.F; return true;
			case 34: key = Key.G; return true;
			case 35: key = Key.H; return true;
			case 36: key = Key.J; return true;
			case 37: key = Key.K; return true;
			case 38: key = Key.L; return true;
			case 39: key = Key.Semicolon; return true;
			case 40: key = Key.Apostrophe; return true;
			case 41: key = Key.GraveAccent; return true;
			case 42: key = Key.ShiftLeft; return true;
			case 43: key = Key.BackSlash; return true;
			case 44: key = Key.Z; return true;
			case 45: key = Key.X; return true;
			case 46: key = Key.C; return true;
			case 47: key = Key.V; return true;
			case 48: key = Key.B; return true;
			case 49: key = Key.N; return true;
			case 50: key = Key.M; return true;
			case 51: key = Key.Comma; return true;
			case 52: key = Key.Period; return true;
			case 53: key = Key.Slash; return true;
			case 54: key = Key.ShiftRight; return true;
			case 55: key = Key.KeypadMultiply; return true;
			case 56: key = Key.AltLeft; return true;
			case 57: key = Key.Space; return true;
			case 58: key = Key.CapsLock; return true;
			case 59: key = Key.F1; return true;
			case 60: key = Key.F2; return true;
			case 61: key = Key.F3; return true;
			case 62: key = Key.F4; return true;
			case 63: key = Key.F5; return true;
			case 64: key = Key.F6; return true;
			case 65: key = Key.F7; return true;
			case 66: key = Key.F8; return true;
			case 67: key = Key.F9; return true;
			case 68: key = Key.F10; return true;
			case 69: key = Key.NumLock; return true;
			case 70: key = Key.ScrollLock; return true;
			case 71: key = Key.Keypad7; return true;
			case 72: key = Key.Keypad8; return true;
			case 73: key = Key.Keypad9; return true;
			case 74: key = Key.KeypadSubtract; return true;
			case 75: key = Key.Keypad4; return true;
			case 76: key = Key.Keypad5; return true;
			case 77: key = Key.Keypad6; return true;
			case 78: key = Key.KeypadAdd; return true;
			case 79: key = Key.Keypad1; return true;
			case 80: key = Key.Keypad2; return true;
			case 81: key = Key.Keypad3; return true;
			case 82: key = Key.Keypad0; return true;
			case 83: key = Key.KeypadPeriod; return true;
			case 86: key = Key.World2; return true;
			case 87: key = Key.F11; return true;
			case 88: key = Key.F12; return true;
			case 89: key = Key.World1; return true;
			case 96: key = Key.KeypadEnter; return true;
			case 97: key = Key.ControlRight; return true;
			case 98: key = Key.KeypadDivide; return true;
			case 99: key = Key.PrintScreen; return true;
			case 100: key = Key.AltRight; return true;
			case 102: key = Key.Home; return true;
			case 103: key = Key.Up; return true;
			case 104: key = Key.PageUp; return true;
			case 105: key = Key.Left; return true;
			case 106: key = Key.Right; return true;
			case 107: key = Key.End; return true;
			case 108: key = Key.Down; return true;
			case 109: key = Key.PageDown; return true;
			case 110: key = Key.Insert; return true;
			case 111: key = Key.Delete; return true;
			case 117: key = Key.KeypadEqual; return true;
			case 119: key = Key.Pause; return true;
			case 125: key = Key.SuperLeft; return true;
			case 126: key = Key.SuperRight; return true;
			case 127: key = Key.Menu; return true;
			case 183: key = Key.F13; return true;
			case 184: key = Key.F14; return true;
			case 185: key = Key.F15; return true;
			case 186: key = Key.F16; return true;
			case 187: key = Key.F17; return true;
			case 188: key = Key.F18; return true;
			case 189: key = Key.F19; return true;
			case 190: key = Key.F20; return true;
			case 191: key = Key.F21; return true;
			case 192: key = Key.F22; return true;
			case 193: key = Key.F23; return true;
			case 194: key = Key.F24; return true;
			default: key = default; return false;
		}
	}
}

/// <summary>
/// Maps Linux evdev button codes (BTN_*) to <see cref="MouseButton"/> values.
/// </summary>
public static class LinuxButtonMap
{
	private const int BtnLeft = 0x110;
	private const int BtnRight = 0x111;
	private const int BtnMiddle = 0x112;
	private const int BtnSide = 0x113;
	private const int BtnExtra = 0x114;

	public static bool TryGetButton(int evdevButton, out MouseButton button)
	{
		switch (evdevButton)
		{
			case BtnLeft: button = MouseButton.Left; return true;
			case BtnRight: button = MouseButton.Right; return true;
			case BtnMiddle: button = MouseButton.Middle; return true;
			case BtnSide: button = MouseButton.Back; return true;
			case BtnExtra: button = MouseButton.Forward; return true;
			default: button = default; return false;
		}
	}
}
