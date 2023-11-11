using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using RawSalt.Mathematics.Geometry;

namespace Testing.Benchmark.RawSalt;

public class Program
{
	static void Main(string[] args)
	{
		BenchmarkRunner.Run<MatrixAddition>();
	}
}

[SimpleJob(RuntimeMoniker.Net70)]
[SimpleJob(RuntimeMoniker.Net80)]
public class MatrixAddition
{
	Matrix4x4 resultMatrix;

	Matrix4x4 lhs, rhs;

	[GlobalSetup]
	public void Setup()
	{
		lhs = new Matrix4x4()
		{
			M11 = 2,
			M44 = 1231,
			M22 = 1231,
			M31 = 1000,
		};
		rhs = new Matrix4x4()
		{
			M11 = 211,
			M44 = 123111,
			M22 = 1231,
			M31 = 111000,
			M14 = int.MaxValue
		};
	}

	[Benchmark]
	public void VT_op_add()
	{
		resultMatrix = Matrix4x4.op_Addition(lhs, rhs);
	}
	[Benchmark]
	public void REF_op_add()
	{
		resultMatrix = Matrix4x4.op_Addition2(in lhs, in rhs);
	}
	[Benchmark]
	public void VT_op_add_RAW()
	{
		resultMatrix = Matrix4x4.op_AdditionRaw(lhs, rhs);
	}
	[Benchmark]
	public void REF_op_add_RAW()
	{
		resultMatrix = Matrix4x4.op_AdditionRaw2(in lhs, in rhs);
	}
}