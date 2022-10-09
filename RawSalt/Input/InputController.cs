using Silk.NET.Input;

namespace RawSalt.Core.Input;

public class InputController
{
	private readonly IInputContext context;
	public InputController(IInputContext context)
	{
		this.context = context;
	}
	public string? Copyboard
	{
		get => null;
		set
		{

		}
	}
}
