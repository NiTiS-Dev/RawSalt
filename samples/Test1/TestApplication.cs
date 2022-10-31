using RawSalt.Resources;
using RawSalt.Structs;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Test1;
public class TestApplication
{
	private static void Main(string[] args)
	{
		BitMap bm = new();

		bm[1] = true;
		bm[4] = true;
		bm[6] = true;

		for (int i = 0; i < 8; i++)
		{
			Console.WriteLine($"{i} is: {bm[i]}");
		}
	}
}
