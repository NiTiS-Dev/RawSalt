using System;

namespace RawSalt;

public static class SaltMath
{
	public static double DegreesToRadians(double degress)
		=> Math.PI / 180 * degress;
	public static float DegreesToRadians(float degress)
		=> (float)(Math.PI / 180) * degress;
	public static double RadiansToDegrees(double rad)
		=> rad * 180 / Math.PI;
	public static float RadiansToDegrees(float rad)
		=> (float)(rad * 180 / Math.PI);
}
