using System.Numerics;
using System.Runtime.CompilerServices;

namespace RawSalt.Input;

public sealed class MouseState
{
	private ulong _updateId;
	private States _states = new();

	public Vector2 Position { get; private set; }
	public Vector2 Scroll { get; private set; }

	public void Update(ulong updateId, float deltaTime)
	{
		_updateId = updateId;

		// Increase held time
		foreach (ref ButtonState state in _states)
		{
			state.HeldTimeInSeconds += deltaTime;
		}

		Scroll = default;
	}

	public bool IsDown(MouseButton button)
	{
		return _states[(int)button].Pressed;
	}

	internal void OnButtonPressed(MouseButton button)
	{
		ref ButtonState state = ref _states[(int)button];
		state = new ButtonState(_updateId)
		{
			Pressed = true
		};
	}

	internal void OnButtonReleased(MouseButton button)
	{
		ref ButtonState state = ref _states[(int)button];
		state.Pressed = false;
	}

	internal void OnMouseMove(Vector2 position)
	{
		Position = position;
	}

	internal void OnMouseScroll(Vector2 scroll)
	{
		Scroll += scroll;
	}

	internal void OnFocusLost()
	{
		// A window that loses focus does not receive button releases.
		foreach (ref ButtonState state in _states)
		{
			state.Pressed = false;
		}
	}

	[InlineArray(5)]
	private struct States
	{
		private ButtonState _element0;
	}

	private struct ButtonState(ulong updateId)
	{
		public readonly ulong PressedUponUpdateId = updateId;
		public float HeldTimeInSeconds = 0f; // Time since first press
		public bool Pressed;
	}
}