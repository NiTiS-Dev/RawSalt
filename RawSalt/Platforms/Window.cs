using RawSalt.Input;

namespace RawSalt.Platforms;

public abstract class Window : View
{
	private readonly HashSet<Key> _pressedKeys = [];
	private readonly HashSet<MouseButton> _pressedButtons = [];
	private Vec2i _cursorPosition;
	private bool _isFocused;
	private bool _isTextInputEnabled = true;
	private CursorShape _cursorShape = CursorShape.Default;

	// Windows MUST define surface size
	public abstract override Vec2i SurfaceSize { get; }

	public bool IsFocused => _isFocused;
	public Vec2i CursorPosition => _cursorPosition;

	/// <summary>
	/// Whether the platform should attach an input method to this window while it is focused.
	/// </summary>
	public bool IsTextInputEnabled
	{
		get => _isTextInputEnabled;
		set
		{
			if (_isTextInputEnabled == value)
			{
				return;
			}

			_isTextInputEnabled = value;
			TextInputEnabledChanged?.Invoke(value);
		}
	}

	public CursorShape CursorShape
	{
		get => _cursorShape;
		set
		{
			if (_cursorShape == value)
			{
				return;
			}

			_cursorShape = value;
			CursorShapeChanged?.Invoke(value);
		}
	}

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
	public event Action<TextInputEventArgs>? TextInput;
	public event Action<PreeditEventArgs>? PreeditChanged;
	public event Action<bool>? TextInputEnabledChanged;
	public event Action<CursorShape>? CursorShapeChanged;

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

	protected void OnTextInput(TextInputEventArgs args)
	{
		TextInput?.Invoke(args);
	}

	protected void OnPreeditChanged(PreeditEventArgs args)
	{
		PreeditChanged?.Invoke(args);
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
