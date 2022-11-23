using NiTiS.IO;
using RawSalt;
using RawSalt.App.Desktop;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.SDL;
using Silk.NET.Windowing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;

namespace Test1;

public class TestApplication : DesktopApplication
{
	public static void Main(string[] args)
	{
		WindowOptions opts = new WindowOptions() with
		{
			Size = new(1200, 700),
			API = new(ContextAPI.OpenGL, new(3, 3)),
			FramesPerSecond = 144,
			UpdatesPerSecond = 100,
			ShouldSwapAutomatically = true,
			Title = "Hello",
			IsVisible = true,
		};
		TestApplication apl = new(opts);

		Console.WriteLine(SaltMath.DivRem(20, 10));

		//apl.Run();
	}
	public TestApplication(WindowOptions opts) : base(opts) { }
	public override void Initialize()
	{
		base.Initialize();

		gl.ClearColor(System.Drawing.Color.AliceBlue);
	}
	public override void Resize(vec2i size)
	{
		base.Resize(size);

		gl.Viewport(size);
	}
} 