using RawSalt.Threading;

namespace RawSalt.Input;

/// <summary>
/// Keyboard keys that are currently held down.
/// </summary>
[ThreadSafety(AllowedThread.Main)]
public sealed class KeyboardState
{
	// TODO(low): Record time since last event to remove stale keys
	private readonly Dictionary<Key, KeyState> _states = [];
	private ulong _updateId;

	public void Update(ulong updateId, float deltaTime)
	{
		_updateId = updateId;

		// Increase held time
		foreach (var kvp in _states)
		{
			KeyState state = kvp.Value;
			_states[kvp.Key] = state with
			{
				HeldTimeInSeconds = state.HeldTimeInSeconds += deltaTime
			};
		}
	}

	public bool IsDown(Key key)
	{
		return _states.ContainsKey(key);
	}

	public bool IsDown(Key key, KeyModifiers mods)
	{
		return IsDown(key) && GetModifiersMask() == mods;
	}

	public float GetHeldTime(Key key)
	{
		KeyState state = _states.GetValueOrDefault(key);

		return state.HeldTimeInSeconds;
	}

	public bool IsAnyDown(params ReadOnlySpan<Key> keys)
	{
		foreach (Key key in keys)
		{
			if (_states.ContainsKey(key)) return true;
		}

		return false;
	}

	internal KeyModifiers GetModifiersMask()
	{
		KeyModifiers mask = 0;

		mask |= IsAnyDown(Key.ShiftLeft, Key.ShiftRight) ? KeyModifiers.Shift : KeyModifiers.None;
		mask |= IsAnyDown(Key.ControlLeft, Key.ControlRight) ? KeyModifiers.Control : KeyModifiers.None;
		mask |= IsAnyDown(Key.SuperLeft, Key.SuperRight) ? KeyModifiers.Super : KeyModifiers.None;
		mask |= IsAnyDown(Key.AltLeft, Key.AltRight) ? KeyModifiers.Alt : KeyModifiers.None;

		return mask;
	}

	internal void OnKeyPressed(Key key)
	{
		if (!_states.TryGetValue(key, out _))
		{
			_states[key] = new KeyState(_updateId);
		}
	}

	internal void OnKeyReleased(Key key)
	{
		_states.Remove(key);
	}

	internal void OnFocusLost()
	{
		_states.Clear();
	}

	private struct KeyState(ulong updateId)
	{
		public readonly ulong PressedUponUpdateId = updateId;
		public float HeldTimeInSeconds = 0f; // Time since first press
	}
}