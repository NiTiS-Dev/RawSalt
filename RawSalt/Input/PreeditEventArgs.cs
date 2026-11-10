namespace RawSalt.Input;

/// <summary>
/// The current input method composition string.
/// </summary>
/// <param name="Text">Composition text. An empty string means composition ended.</param>
/// <param name="CursorBegin">Cursor position inside the composition text in bytes.</param>
/// <param name="CursorEnd">Selection end inside the composition text in bytes.</param>
public sealed record PreeditEventArgs(string Text, int CursorBegin, int CursorEnd);
