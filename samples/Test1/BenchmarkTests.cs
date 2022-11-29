using BenchmarkDotNet.Attributes;
using NiTiS.Math;
using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Test1;
public class BenchmarkTests
{
	const int count = 5000;
	[Benchmark(Description = "NiTiS")]
	public void TestNITISMATH()
	{
		Mat4<float>[] arr = new Mat4<float>[count];
		for (int i = 0; i < count; i++)
		{
			arr[i] = Mat4.CreateTranslation<float>(new(i, i, -i));
		}
	}
	[Benchmark(Description = "Silk.NET")]
	public void TestSILKMATH()
	{
		Matrix4X4<float>[] arr = new Matrix4X4<float>[count];
		for (int i = 0; i < count; i++)
		{
			arr[i] = Matrix4X4.CreateTranslation<float>(new(i, i, -i));
		}
	}
	[Benchmark(Description = "std")]
	public void TestSYSMATH()
	{
		Matrix4x4[] arr = new Matrix4x4[count];
		for (int i = 0; i < count; i++)
		{
			arr[i] = System.Numerics.Matrix4x4.CreateTranslation(new(i, i, -i));
		}
	}
}
