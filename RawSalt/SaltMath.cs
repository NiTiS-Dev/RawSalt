using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace RawSalt;

public static class SaltMath
{
	public static double PI => 3.1415926535897931;
	private static float PIf => 3.1415926535897931f;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double DegreesToRadians(double degress)
		=> PI / 180 * degress;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float DegreesToRadians(float degress)
		=> PIf / 180 * degress;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double RadiansToDegrees(double rad)
		=> rad * 180 / PI;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float RadiansToDegrees(float rad)
		=> rad * 180 / PIf;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (T Quotient, T Remainder) DivRem<T>(T left, T right)
		where T : 
			IDivisionOperators<T, T, T>,
			IMultiplyOperators<T, T, T>,
			ISubtractionOperators<T, T, T>
	{
		T quotient = left / right;
		return (quotient, left - (quotient * right));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Clamp<T>(T value, T min, T max)
		where T : IComparisonOperators<T, T, bool>
	{
		if (min > max)
		{
			throw new ArgumentOutOfRangeException(nameof(min));
 		}

		if (value < min)
		{
			return min;
		}
		else if (value > max)
		{
			return max;
		}

		return value;
	}
}
