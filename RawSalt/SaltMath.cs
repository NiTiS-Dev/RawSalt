using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RawSalt;

public static class SaltMath
{
	public static double DegressToRadiant(double degress)
		=> (Math.PI / 180) * degress;
	public static float DegressToRadiant(float degress)
		=> (float)(Math.PI / 180) * degress;
}
