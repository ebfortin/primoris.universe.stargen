using System;
using Primoris.Universe.Stargen.Astrophysics;
using Primoris.Universe.Stargen.Services;



namespace Primoris.Universe.Stargen
{

	/// <summary>
	/// Group of Extension methods and utility functions.
	/// </summary>
	public static class Extensions
	{
		/// <summary>
		/// The common Random class instance to use.
		/// </summary>
		public static Random Random = new Random();

		/// <summary>
		/// Comapre two double numbers with a tolerance.
		/// </summary>
		/// <param name="v1">The v1.</param>
		/// <param name="v2">The v2.</param>
		/// <param name="diff">The difference.</param>
		/// <returns></returns>
		public static bool AlmostEqual(this double v1, double v2, double diff=0.00001)
        {
			if (double.IsNaN(v1) && double.IsNaN(v2))
				return true;

            return Math.Abs(v1 - v2) <= Math.Abs(v1 * diff);
        }

		/// <summary>
		/// Initializes the random seed.
		/// </summary>
		/// <param name="seed">The seed.</param>
		public static void InitRandomSeed(int seed)
        {
            Provider.Use().WithRandom(new Random(seed));
        }

		/// <summary>
		/// Gets the semi minor axis.
		/// </summary>
		/// <param name="a">Semi major axis.</param>
		/// <param name="e">Orbital eccentricity.</param>
		/// <returns></returns>
		public static double GetSemiMinorAxis(double a, double e)
        {
            return a * Math.Sqrt(1 - Math.Pow(e, 2));
        }

		/// <summary>
		/// Pow2s the specified value.
		/// </summary>
		/// <param name="a">Value to apply pow to.</param>
		/// <returns></returns>
		public static double Pow2(this double a)
        {
            return a * a;
        }

		/// <summary>
		/// Pow3s the specified value.
		/// </summary>
		/// <param name="a">Value to apply pow to.</param>
		/// <returns></returns>
		public static double Pow3(this double a)
        {
            return a * a * a;
        }

		/// <summary>
		/// Pow4s the specified value.
		/// </summary>
		/// <param name="a">Value to apply pow to.</param>
		/// <returns></returns>
		public static double Pow4(this double a)
        {
            return a * a * a * a;
        }

		/// <summary>
		/// Power 1/4 of value.
		/// </summary>
		/// <param name="a">Value to apply pow to.</param>
		/// <returns></returns>
		public static double Pow1_4(this double a)
        {
            return Math.Sqrt(Math.Sqrt(a));
        }

		/// <summary>
		/// Power 1/3 of value.
		/// </summary>
		/// <param name="a">Value to apply pow to.</param>
		/// <returns></returns>
		public static double Pow1_3(this double a)
        {
            return Math.Pow(a, (1.0 / 3.0));
        }

		/// <summary>
		/// Returns a Random double number.
		/// </summary>
		/// <returns>Random number.</returns>
		public static double RandomNumber()
        {
            return Provider.Use().GetService<Random>().NextDouble();
        }

		/// <summary>
		/// Returns a random integer number.
		/// </summary>
		/// <param name="lowerBound">The lower bound.</param>
		/// <param name="upperBound">The upper bound.</param>
		/// <returns>Random number.</returns>
		public static int RandomInt(int lowerBound, int upperBound)
        {
            return Provider.Use().GetService<Random>().Next(lowerBound, upperBound);
        }

		/// <summary>
		/// Returns a random double number between two bound.
		/// </summary>
		/// <param name="inner">The inner bound.</param>
		/// <param name="outer">The outer bound.</param>
		/// <returns>Random number.</returns>
		public static double RandomNumber(double inner, double outer)
        {
            var range = outer - inner;
            return Provider.Use().GetService<Random>().NextDouble() * range + inner;
        }

		/// <summary>
		/// Returns a new number around a value given a variation around it.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="variation">The variation.</param>
		/// <returns>A new number around the given value.</returns>
		public static double About(this double value, double variation)
        {
            return (value + (value * RandomNumber(-variation, variation)));
        }

		/// <summary>
		/// Returns a random eccentricity.
		/// </summary>
		/// <returns>Randomized orbital eccenricity.</returns>
		public static double RandomEccentricity()
        {
            return 1.0 - Math.Pow(RandomNumber(), GlobalConstants.ECCENTRICITY_COEFF);
        }
    }
}
