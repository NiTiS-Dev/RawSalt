using RawSalt;
using RawSalt.Graphics;
using RawSalt.Native.SDL;
using RawSalt.Native.Windows;

namespace Triangle;

public class Program : Application
{
	public Program(RenderOptions renderOptions) : base(renderOptions)
	{
	}

	static void Main(string[] args)
	{
		SDL2.SDL_ShowSimpleMessageBox(SDL2.SDL_MessageBoxFlags.SDL_MESSAGEBOX_WARNING, "Hi", "Anaans", 0);
		//try
		//{
		//	Program app = new(new RenderOptions(false, 60));
		//	app.Run();
		//}
		//catch (Exception e)
		//{
		//	Console.Error.WriteLine(e);
		//}
	}
}