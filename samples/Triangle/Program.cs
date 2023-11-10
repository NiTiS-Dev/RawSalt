using RawSalt;
using System;

namespace Triangle;

public class Program : DesktopApplication
{
	public const string TriangleAppId = "triangle";
	public Program(string appId) : base(appId)
	{
	}

	static void Main(string[] args)
	{
		try
		{
			Program app = new(TriangleAppId);
			
			app.Initialize();
			
			app.Run();
		}
		catch (Exception e)
		{
			Console.Error.WriteLine(e);
		}
	}
}