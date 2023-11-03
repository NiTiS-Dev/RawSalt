using RawSalt.Native.SDL;

namespace RawSalt;

public class DesktopApplication : Application
{
	public DesktopApplication(string applicationId) : base(applicationId)
	{
		title = applicationId;
	}

	#region Properties

	private nint windowHandle;
	public nint WindowHandle => windowHandle;

	private string title;
	public string Title
	{
		get
		{
			if (windowHandle == nint.Zero)
				return title;

			return SDL2.SDL_GetWindowTitle(windowHandle);
		}
		set
		{
			title = value;
			
			if (windowHandle != nint.Zero)
				SDL2.SDL_SetWindowTitle(windowHandle, value);
		}
	}

	#endregion

	public override void Run()
	{
		while (true)
		{
			SDL2.SDL_PollEvent(out SDL2.SDL_Event evnt);


		}
	}

	public override void Setup()
	{
		if (SDL2.SDL_Init(SDL2.SDL_WasInit(SDL2.SDL_INIT_VIDEO | SDL2.SDL_INIT_AUDIO)) != 0)
		{
			logger.Fatal("Unable to initialize SDL_INIT_VIDEO and/or SDL_INIT_AUDIO");
		}

		windowHandle = SDL2.SDL_CreateWindow(Title,
				SDL2.SDL_WINDOWPOS_CENTERED, SDL2.SDL_WINDOWPOS_CENTERED,
				512, 512,
				SDL2.SDL_WindowFlags.SDL_WINDOW_VULKAN
				);
	}
}
