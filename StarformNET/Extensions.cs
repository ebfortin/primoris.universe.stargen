using System;
using Primoris.Universe.Stargen.Astrophysics;
using Primoris.Universe.Stargen.Services;


namespace Primoris.Universe.Stargen
{


	public static class Extensions
	{

        public static Random Random = new Random();

        public static bool AlmostEqual(this double v1, double v2, double diff=0.00001)
        {
			if (double.IsNaN(v1) && double.IsNaN(v2))
				return true;

            return Math.Abs(v1 - v2) <= Math.Abs(v1 * diff);
        }

        public static void InitRandomSeed(int seed)
        {
            Provider.Use().WithRandom(new Random(seed));
        }

        public static double GetSemiMinorAxis(double a, double e)
        {
            return a * Math.Sqrt(1 - Math.Pow(e, 2));
        }

        public static double Pow2(this double a)
        {
            return a * a;
        }

        public static double Pow3(this double a)
        {
            return a * a * a;
        }

        public static double Pow4(this double a)
        {
            return a * a * a * a;
        }

        public static double Pow1_4(this double a)
        {
            return Math.Sqrt(Math.Sqrt(a));
        }

        public static double Pow1_3(this double a)
        {
            return Math.Pow(a, (1.0 / 3.0));
        }

        public static double RandomNumber()
        {
            return Provider.Use().GetService<Random>().NextDouble();
        }

        public static int RandomInt(int lowerBound, int upperBound)
        {
            return Provider.Use().GetService<Random>().Next(lowerBound, upperBound);
        }

        public static double RandomNumber(double inner, double outer)
        {
            var range = outer - inner;
            return Provider.Use().GetService<Random>().NextDouble() * range + inner;
        }

        public static double About(this double value, double variation)
        {
            return (value + (value * RandomNumber(-variation, variation)));
        }

        public static double RandomEccentricity()
        {
            return 1.0 - Math.Pow(RandomNumber(), GlobalConstants.ECCENTRICITY_COEFF);
        }
    }
}
