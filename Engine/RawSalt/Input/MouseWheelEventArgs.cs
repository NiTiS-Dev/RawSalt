namespace RawSalt.Input;

/// <param name="Delta">
/// Scroll delta in wheel notches. Positive X scrolls right, positive Y scrolls up.
/// </param>
public sealed record MouseWheelEventArgs(Vec2i Delta);
