using System;
using System.Runtime.CompilerServices;
using RawSalt.Maths;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Triangle;

internal class Program
{
	static void Main(string[] args)
	{
		const int H = 500;
		const int W = 500;

		using Image<Rgba32> img = new Image<Rgba32>(H, W);

		Random rnd = new();
		img.ProcessPixelRows( rows =>
		{
			for (int y = 0; y < H; y++)
			{
				var span = rows.GetRowSpan(y);
				for (int x = 0; x < W; x++)
				{
					int castInt = rnd.Next();
					Rgba32 colour = Unsafe.As<int, Rgba32>(ref castInt);
					colour.A = 255;
					span[x] = colour;
				}
			}
		});

		img.SaveAsPng("C:/Desktop/img.png");
	}
}
