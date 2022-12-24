using RawSalt.App;
using RawSalt.Resources;

namespace Test3;

public sealed class Program : Application
{
	public static void Main(string[] args)
		=> _ = new Program();
	public Program()
	{
		Identifier id = new("actor", "decta");

		System.Console.WriteLine(id);
	}
}
