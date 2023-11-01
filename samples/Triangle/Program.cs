using RawSalt;
using RawSalt.Graphics;
using RawSalt.Native.Vulkan;
using RawSalt.Native.Windows;
using System;

namespace Triangle;

public class Program : Application
{
	public Program(RenderOptions renderOptions) : base(renderOptions)
	{
	}

	static void Main(string[] args)
	{
		try
		{
			Program app = new(new RenderOptions(false, 60));
			app.Run();
		}
		catch (Exception e)
		{
			Console.Error.WriteLine(e);
		}
	}
}
