using RawSalt.Input;

namespace RawSalt.Platforms;

public abstract class Window : View
{
	private readonly HashSet<Key> _pressedKeys = [];
	private readonly HashSet<MouseButton> _pressedButtons = [];
	private Vec2i _cursorPosition;
	private bool _isFocused;

	// Windows MUST define surface size
	public abstract override Vec2i SurfaceSize { get; }

	public bool IsFocused => _isFocused;
	public Vec2i CursorPosition => _cursorPosition;

	public abstract void Hide();
	public abstract void Show();

	public bool IsKeyDown(Key key) => _pressedKeys.Contains(key);

	public bool IsMouseButtonDown(MouseButton button) => _pressedButtons.Contains(button);

	public event Action<KeyEventArgs>? KeyDown;
	public event Action<KeyEventArgs>? KeyUp;
	public event Action<MouseButtonEventArgs>? MouseDown;
	public event Action<MouseButtonEventArgs>? MouseUp;
	public event Action<MouseMoveEventArgs>? MouseMove;
	public event Action<MouseWheelEventArgs>? MouseWheel;
	public event Action<FocusChangedEventArgs>? FocusChanged;
	public event Action? CursorEntered;
	public event Action? CursorLeft;

	protected void OnKeyDown(KeyEventArgs args)
	{
		_pressedKeys.Add(args.Key);
		KeyDown?.Invoke(args);
	}

	protected void OnKeyUp(KeyEventArgs args)
	{
		_pressedKeys.Remove(args.Key);
		KeyUp?.Invoke(args);
	}

	protected void OnMouseDown(MouseButtonEventArgs args)
	{
		_pressedButtons.Add(args.Button);
		MouseDown?.Invoke(args);
	}

	protected void OnMouseUp(MouseButtonEventArgs args)
	{
		_pressedButtons.Remove(args.Button);
		MouseUp?.Invoke(args);
	}

	protected void OnMouseMove(MouseMoveEventArgs args)
	{
		_cursorPosition = args.Position;
		MouseMove?.Invoke(args);
	}

	protected void OnMouseWheel(MouseWheelEventArgs args)
	{
		MouseWheel?.Invoke(args);
	}

	protected void OnCursorEntered()
	{
		CursorEntered?.Invoke();
	}

	protected void OnCursorLeft()
	{
		CursorLeft?.Invoke();
	}

	protected void OnFocusChanged(bool focused)
	{
		if (_isFocused == focused)
		{
			return;
		}

		_isFocused = focused;

		if (!focused)
		{
			// A window that loses focus does not receive key or button releases.
			_pressedKeys.Clear();
			_pressedButtons.Clear();
		}

		FocusChanged?.Invoke(new FocusChangedEventArgs(focused));
	}
}
