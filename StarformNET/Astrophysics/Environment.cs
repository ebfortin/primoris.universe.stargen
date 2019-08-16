using System;
using Primoris.Universe.Stargen.Bodies;
using UnitsNet;

namespace Primoris.Universe.Stargen.Astrophysics
{


	// TODO: Figure out a way to logically break this class up
	// TODO: Make it a Service and Fluent API support.
	public static class Environment
    {






        /// <summary>
        /// Returns the luminosity of a star using the Mass-Luminosity relationship.
        /// </summary>
        /// <param name="massRatio">Mass of the star</param>
        /// <returns>Luminosity ratio</returns>
        public static double MassToLuminosity(Mass massRatioParam)
        {
			var massRatio = massRatioParam.SolarMasses;

			if (massRatio <= 0.6224)
			{
				return 0.3815 * Math.Pow(massRatio, 2.5185);
			}
			else if (massRatio <= 1.0)
			{
				return Math.Pow(massRatio, 4.551);
			}
			else if (massRatio <= 3.1623)
			{
				return Math.Pow(massRatio, 4.351);
			}
			else if (massRatio <= 16.0)
			{
				return 2.7563 * Math.Pow(massRatio, 3.4704);
			}
			else
			{
				return 42.321 * Math.Pow(massRatio, 2.4853);
			}
		}






        /// <summary>
        /// Calculates the surface acceleration of the planet.
        /// </summary>
        /// <param name="mass">Mass of the planet in solar masses</param>
        /// <param name="radius">Radius of the planet in km</param>
        /// <returns>Acceleration returned in units of cm/sec2</returns>
        public static double GetAcceleration(double mass, double radius)
        {
            return (GlobalConstants.GRAV_CONSTANT * (mass * GlobalConstants.SOLAR_MASS_IN_GRAMS) / Utilities.Pow2(radius * GlobalConstants.CM_PER_KM));
        }

        









        



        
 

 

        



    }
}
