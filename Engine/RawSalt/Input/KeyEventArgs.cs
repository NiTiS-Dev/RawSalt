namespace RawSalt.Input;

/// <param name="IsRepeat">Whether the key is repeated because it is held down.</param>
public sealed record KeyEventArgs(Key Key, KeyModifiers Modifiers, bool IsRepeat = false);
