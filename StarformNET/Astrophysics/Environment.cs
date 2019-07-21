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
        /// Returns the radius (center?) of a star's ecosphere in au.
        /// </summary>
        /// <param name="luminosity">Luminosity of the star in solar luminosity
        /// units.</param>
        /// <returns>Star's ecosphere radius in au.</returns>
        public static double StarEcosphereRadiusAU(double luminosity)
        {
            // No idea the source of this. It centers the Solar System's ecosphere at
            // 1 au.
            return Math.Sqrt(luminosity);
        }


		/// <summary>
		/// Returns the mass of a star using the Mass-Luminosity relationship.
		/// </summary>
		/// <param name="lumRatio">Luminosity ratio of the star</param>
		/// <returns>Mass ratio</returns>
		public static double LuminosityToMass(double lumRatio)
		{
			double a = lumRatio;
			if (a <= (0.3815 * Math.Pow(0.6224, 2.5185)))
			{
				return 1.46613 * Math.Pow(a, 0.3970617431010522);
			}
			else if (a <= 1)
			{
				return Math.Pow(a, 0.2197319270490002);
			}
			else if (a <= Math.Pow(3.1623, 4.351))
			{
				return Math.Pow(a, 0.2298322224775914);
			}
			else if (a <= (2.7563 * Math.Pow(16, 3.4704)))
			{
				return 0.746654 * Math.Pow(a, 0.2881512217611803);
			}
			else
			{
				return 0.221579 * Math.Pow(a, 0.4023659115599726);
			}
		}

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
        /// Returns true if the planet is tidally locked to its parent body.
        /// </summary>
        /*public static bool IsTidallyLocked(SatelliteBody planet)
        {
            return (int) planet.DayLength == (int) (planet.OrbitalPeriod * 24);
        }*/


        /// <summary>
        /// Calculates the radius of a planet.
        /// </summary>
        /// <param name="mass">Mass in units of solar masses</param>
        /// <param name="density">Density in units of grams/cc</param>
        /// <returns>Radius in units of km</returns>
        public static double VolumeRadius(double mass, double density)
        {
            double volume;

            mass = mass * GlobalConstants.SOLAR_MASS_IN_GRAMS;
            volume = mass / density;
            return (Math.Pow((3.0 * volume) / (4.0 * Math.PI), (1.0 / 3.0)) / GlobalConstants.CM_PER_KM);
        }


        /// <summary>
        /// Density returned in units of grams/cc
        /// </summary>
        /// <param name="mass">Mass in units of solar masses</param>
        /// <param name="equatRadius">Equatorial radius in km</param>
        /// <returns>Units of grams/cc</returns>
        public static double VolumeDensity(double mass, double equatRadius)
        {
            mass = mass * GlobalConstants.SOLAR_MASS_IN_GRAMS;
            equatRadius = equatRadius * GlobalConstants.CM_PER_KM;
            double volume = (4.0 * Math.PI * Utilities.Pow3(equatRadius)) / 3.0;
            return (mass / volume);
        }



        /// <summary>
        /// Checks if a planet's rotation is in resonance with its orbit
        /// </summary>
        /// <param name="angularVelocity">The planet's angular velocity in
        /// radians/sec</param>
        /// <param name="dayInHours">The length of the planet's day in
        /// hours</param>
        /// <param name="orbitalPeriodDays">The orbital period of the planet
        /// in days</param>
        /// <param name="ecc">The eccentricity of the planet's orbit</param>
        /// <returns>True if the planet is in a resonant orbit</returns>
        public static bool HasResonantPeriod(double angularVelocity,
            double dayInHours, double orbitalPeriodDays, double ecc)
        {
            var yearInHours = orbitalPeriodDays * 24.0;
            return (angularVelocity <= 0.0 || dayInHours >= yearInHours)
                   && ecc > 0.1;
        }
        
        /// <summary>
        /// Returns a randomized inclination in units of degrees
        /// </summary>
        /// <param name="semiMajorAxisAU">Orbital radius in units of AU</param>
        public static Angle Inclination(Length semiMajorAxis)
        {
            var inclination = ((int)(Math.Pow(semiMajorAxis.Kilometers / GlobalConstants.ASTRONOMICAL_UNIT_KM, 0.2) * Utilities.About(GlobalConstants.EARTH_AXIAL_TILT, 0.4)) % 360);
            return Angle.FromDegrees(inclination);
        }

        /// <summary>
        /// Returns escape velocity in cm/sec
        /// </summary>
        /// <param name="mass">Mass in units of solar mass</param>
        /// <param name="radius">Radius in km</param>
        /// <returns></returns>
        public static double EscapeVelocity(double mass, double radius)
        {
            // This function implements the escape velocity calculation. Note that
            // it appears that Fogg's eq.15 is incorrect.	

            var massInGrams = mass * GlobalConstants.SOLAR_MASS_IN_GRAMS;
            var radiusinCM = radius * GlobalConstants.CM_PER_KM;
            return (Math.Sqrt(2.0 * GlobalConstants.GRAV_CONSTANT * massInGrams / radiusinCM));
        }
        
        /// <summary>
        /// Calculates the root-mean-square velocity of particles in a gas.
        /// </summary>
        /// <param name="molecularWeight">Mass of gas in g/mol</param>
        /// <param name="exosphericTemp">Temperature in K</param>
        /// <returns>Returns RMS velocity in m/sec</returns>
        public static double RMSVelocity(double molecularWeight, double exosphericTemp)
        {
            // This is Fogg's eq.16.  The molecular weight (usually assumed to be N2)
            // is used as the basis of the Root Mean Square (RMS) velocity of the
            // molecule or atom.
            // https://en.wikipedia.org/wiki/Root-mean-square_speed

            return (Math.Sqrt((3.0 * GlobalConstants.MOLAR_GAS_CONST * exosphericTemp) / molecularWeight)
                   * GlobalConstants.CM_PER_METER);
        }

        /// <summary>
        /// Returns the smallest molecular weight retained by the body, which is useful
        /// for determining atmospheric composition.
        /// </summary>
        /// <param name="mass">Mass in solar masses</param>
        /// <param name="equatorialRadius">Equatorial radius in units of kilometers</param>
        /// <param name="exosphericTemp"></param>
        /// <returns></returns>
        public static double MoleculeLimit(double mass, double equatorialRadius, double exosphericTemp)
        {
            var escapeVelocity = EscapeVelocity(mass, equatorialRadius);

            return ((3.0 * GlobalConstants.MOLAR_GAS_CONST * exosphericTemp) /
                    (Utilities.Pow2((escapeVelocity / GlobalConstants.GAS_RETENTION_THRESHOLD) / GlobalConstants.CM_PER_METER)));

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

        /// <summary>
        /// Calculates the surface gravity of a planet.
        /// </summary>
        /// <param name="acceleration">Acceleration in units of cm/sec2</param>
        /// <returns>Gravity in units of Earth gravities</returns>
        public static double Gravity(Acceleration acceleration)
        {
            return acceleration.CentimetersPerSecondSquared / GlobalConstants.EARTH_ACCELERATION;
        }
        












        // TODO figure out how this function differs from EffTemp, write summary
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ecosphereRadius"></param>
        /// <param name="orbitalRadius"></param>
        /// <param name="albedo"></param>
        /// <returns></returns>
        public static double EstTemp(double ecosphereRadius, double orbitalRadius, double albedo)
        {
            return Math.Sqrt(ecosphereRadius / orbitalRadius)
                  * Utilities.Pow1_4((1.0 - albedo) / (1.0 - GlobalConstants.EARTH_ALBEDO))
                  * GlobalConstants.EARTH_AVERAGE_KELVIN;
        }
        



        
 

 

        /// <summary>
        /// Calculates the number of years it takes for 1/e of a gas to escape from a
        /// planet's atmosphere
        /// </summary>
        /// <param name="molecularWeight">Molecular weight of the gas</param>
        /// <param name="exoTempKelvin">Exosphere temperature of the planet in Kelvin</param>
        /// <param name="surfGravG">Surface gravity of the planet in G</param>
        /// <param name="radiusKM">Radius of the planet in km</param>
        /// <returns></returns>
        public static double GasLife(double molecularWeight, 
            double exoTempKelvin, double surfGravG, double radiusKM)
        {
            // Taken from Dole p. 34. He cites Jeans (1916) & Jones (1923)

            var v = RMSVelocity(molecularWeight, exoTempKelvin);
            var g = surfGravG * GlobalConstants.EARTH_ACCELERATION;
            var r = radiusKM * GlobalConstants.CM_PER_KM;
            var t = (Utilities.Pow3(v) / (2.0 * Utilities.Pow2(g) * r)) * Math.Exp((3.0 * g * r) / Utilities.Pow2(v));
            var years = t / (GlobalConstants.SECONDS_PER_HOUR * 24.0 * GlobalConstants.DAYS_IN_A_YEAR);

            if (years > 2.0E10)
            {
                years = double.MaxValue;
            }

            return years;
        }
        
        /// <summary>
        /// Calculates the minimum molecular weight retained by a planet
        /// </summary>
        /*public static double MinMolecularWeight(SatelliteBody planet)
        {
            var surfGrav = planet.SurfaceGravityG;
            var mass = planet.Mass;
            var radius = planet.Radius.Kilometers;
            var exosphereTemp = planet.ExosphereTemperature.Kelvins;
            var temp = planet.ExosphereTemperature.Kelvins;
            var target = 5.0E9;

            var guess1 = MoleculeLimit(mass, radius, temp);
            var guess2 = guess1;

            var life = GasLife(guess1, exosphereTemp, surfGrav, radius);

            var loops = 0;

            if (planet.Star != null)
            {
                target = planet.Star.Age.Years365;
            }

            if (life > target)
            {
                while ((life > target) && (loops++ < 25))
                {
                    guess1 = guess1 / 2.0;
                    life = GasLife(guess1, exosphereTemp, surfGrav, radius);
                }
            }
            else
            {
                while ((life < target) && (loops++ < 25))
                {
                    guess2 = guess2 * 2.0;
                    life = GasLife(guess2, exosphereTemp, surfGrav, radius);
                }
            }

            loops = 0;

            while (guess2 - guess1 > 0.1 && loops++ < 25)
            {
                var guess3 = (guess1 + guess2) / 2.0;
                life = GasLife(guess3, exosphereTemp, surfGrav, radius);

                if (life < target)
                {
                    guess1 = guess3;
                }
                else
                {
                    guess2 = guess3;
                }
            }

            life = GasLife(guess2, exosphereTemp, surfGrav, radius);

            return guess2;
        }*/

        /// <summary>
        /// Inspired partial pressure, taking into account humidification of the air in the nasal
        /// passage and throat.
        /// </summary>
        /// <param name="surf_pressure">Total atmospheric surface pressure in millibars</param>
        /// <param name="gas_pressure">Partial gas pressure in millibars</param>
        /// <returns>Inspired partial pressure in millibars</returns>
        public static double InspiredPartialPressure(double surf_pressure, double gas_pressure)
        {
            //  This formula is on Dole's p. 14
            var pH2O = (GlobalConstants.H20_ASSUMED_PRESSURE);
            var fraction = gas_pressure / surf_pressure;

            return (surf_pressure - pH2O) * fraction;
        }


    }
}
