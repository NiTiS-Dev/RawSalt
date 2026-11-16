namespace RawSalt.Graphics;

public enum PresentMode
{
	/// <summary>
	/// Frame goes to the display immediately after finish rendering, can cause tearing.
	/// </summary>
	Immediate,
	/// <summary>
	/// First in, first out.
	/// </summary>
	Fifo,
	RelaxedFifo,
	Mailbox,
}