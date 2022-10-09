using System;

namespace RawSalt;

public static class SaltMath
{
	public const double PI = 3.1415926535897931;
	private const float PIf= 3.1415926535897931f;
	public static double DegreesToRadians(double degress)
		=> PI / 180 * degress;
	public static float DegreesToRadians(float degress)
		=> PIf / 180 * degress;
	public static double RadiansToDegrees(double rad)
		=> rad * 180 / PI;
	public static float RadiansToDegrees(float rad)
		=> rad * 180 / PIf;
}
