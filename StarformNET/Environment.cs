namespace DLS.StarformNET
{
    using System;
    using Data;

    // TODO Figure out a way to logically break this class up
    public static class Environment
    {
        /// <summary>
        /// Returns the illumination of a body at its farthest point from a
        /// star.
        /// </summary>
        /// <param name="a">Semi-major axis of the body in au</param>
        /// <param name="l">Luminance of the star in solar luminance units</param>
        /// <returns>Illumination where 1.0 is equal to the light received by
        /// the Earth from the Sun.</returns>
        public static double MinimumIllumination(double a, double l)
        {
            // No idea about the source of this other than the obvious fact that
            // light follows the inverse square law.
            return Utilities.Pow2(1.0 / a) * l;
        }

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
        /// Returns the hill sphere of object m orbiting object M with a semi-
        /// major axis a. Ignores eccentricity.
        /// </summary>
        /// <param name="M">The mass of the larger body</param>
        /// <param name="m">The mass of the smaller body</param>
        /// <param name="a">Semi-major axis in AU</param>
        /// <returns>Hill sphere in KM</returns>
        public static double SimplifiedHillSphereKM(double M, double m, double a)
        {
            return SimplifiedHillSphereAU(M, m, a) * GlobalConstants.KM_PER_AU;
        }

        /// <summary>
        /// Returns the hill sphere of object m orbiting object M with a semi-
        /// major axis a. Ignores eccentricity.
        /// </summary>
        /// <param name="M">The mass of the larger body</param>
        /// <param name="m">The mass of the smaller body</param>
        /// <param name="a">Semi-major axis in AU</param>
        /// <returns>Hill sphere in AU</returns>
        public static double SimplifiedHillSphereAU(double M, double m, double a)
        {
            return a * Math.Pow(m / (3 * M), (1.0 / 3.0));
        }

        /// <summary>
        /// Returns the Roche limit of an object in KM
        /// </summary>
        /// <param name="bodyRadius">Radius of the body in km</param>
        /// <param name="bodyDensity">Density of the body in g/cc</param>
        /// <param name="satelliteDensity">Density of the orbiting satellite
        /// in g/cc </param>
        /// <returns>Roche limit for the satellite in KM</returns>
        public static double RocheLimitKM(double bodyRadius, double bodyDensity, double satelliteDensity)
        {
            return (1.26 * bodyRadius * Math.Pow(bodyDensity / satelliteDensity, 1.0 / 3.0)) / 1000.0;
        }

        /// <summary>
        /// Returns the Roche limit of an object in AU
        /// </summary>
        /// <param name="bodyRadius">Radius of the body in km</param>
        /// <param name="bodyDensity">Density of the body in g/cc</param>
        /// <param name="satelliteDensity">Density of the orbiting satellite
        /// in g/cc </param>
        /// <returns>Roche limit for the satellite in AU</returns>
        public static double RocheLimitAU(double bodyRadius, double bodyDensity, double satelliteDensity)
        {
            return RocheLimitKM(bodyRadius, bodyDensity, satelliteDensity) / GlobalConstants.KM_PER_AU;
        }

		/// <summary>
		/// Returns the mass of a star using the Mass-Luminosity relationship.
		/// </summary>
		/// <param name="lumRatio">Luminosity ratio of the star</param>
		/// <returns>Mass ratio</returns>
		public static double Mass(double lumRatio)
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
        public static double Luminosity(double massRatio)
        {
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
        public static bool IsTidallyLocked(Planet planet)
        {
            return (int) planet.DayLengthHours == (int) (planet.OrbitalPeriodDays * 24);
        }

        /// <summary>
        /// Returns true if the planet's conditions can support human life
        /// </summary>
        public static bool IsHabitable(Planet planet)
        {
            return planet.Atmosphere.Breathability == Data.Breathability.Breathable &&
                   !planet.HasResonantPeriod &&
                   !IsTidallyLocked(planet);
        }

        /// <summary>
        /// Returns true if the planet's conditions are similar to Earth
        /// </summary>
        public static bool IsEarthlike(Planet planet)
        {
            double relTemp = (planet.SurfaceTempKelvin - GlobalConstants.FREEZING_POINT_OF_WATER) -
                             GlobalConstants.EARTH_AVERAGE_CELSIUS;
            double seas = planet.WaterCoverFraction * 100.0;
            double clouds = planet.CloudCoverFraction * 100.0;
            double pressure = planet.Atmosphere.SurfacePressure / GlobalConstants.EARTH_SURF_PRES_IN_MILLIBARS;
            double ice = planet.IceCoverFraction * 100.0;

            return
                planet.SurfaceGravityG >= .8 &&
                planet.SurfaceGravityG <= 1.2 &&
                relTemp >= -2.0 &&
                relTemp <= 3.0 &&
                ice <= 10.0 &&
                pressure >= 0.5 &&
                pressure <= 2.0 &&
                clouds >= 40.0 &&
                clouds <= 80.0 &&
                seas >= 50.0 &&
                seas <= 80.0 &&
                planet.Type != PlanetType.Water &&
                planet.Atmosphere.Breathability == Data.Breathability.Breathable;
        }

        /// <summary>
        /// This function, given the orbital radius of a planet in AU, returns
        /// the orbital 'zone' of the planet.
        /// </summary>
        public static int OrbitalZone(double luminosity, double orbRadius)
        {
            if (orbRadius < (4.0 * Math.Sqrt(luminosity)))
            {
                return (1);
            }
            else if (orbRadius < (15.0 * Math.Sqrt(luminosity)))
            {
                return (2);
            }
            else
            {
                return (3);
            }
        }

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
        /// Returns the radius of the planet in kilometers.        
        /// </summary>
        /// <param name="mass">Mass in units of solar mass</param>
        /// <param name="giant">Is the planet a gas giant?</param>
        /// <param name="zone">Orbital zone</param>
        /// <returns>Radius in km</returns>
        public static double KothariRadius(double mass, bool giant, int zone)
        {
            // This formula is listed as eq.9 in Fogg's article, although some typos
            // crop up in that eq.  See "The Internal Constitution of Planets", by
            // Dr. D. S. Kothari, Mon. Not. of the Royal Astronomical Society, vol 96
            // pp.833-843, 1936 for the derivation.  Specifically, this is Kothari's
            // eq.23, which appears on page 840.
            // http://articles.adsabs.harvard.edu//full/1936MNRAS..96..833K/0000840.000.html

            double temp1;
            double temp, temp2, atomic_weight, atomic_num;

            if (zone == 1)
            {
                if (giant)
                {
                    atomic_weight = 9.5;
                    atomic_num = 4.5;
                }
                else
                {
                    atomic_weight = 15.0;
                    atomic_num = 8.0;
                }
            }
            else if (zone == 2)
            {
                if (giant)
                {
                    atomic_weight = 2.47;
                    atomic_num = 2.0;
                }
                else
                {
                    atomic_weight = 10.0;
                    atomic_num = 5.0;
                }
            }
            else
            {
                if (giant)
                {
                    atomic_weight = 7.0;
                    atomic_num = 4.0;
                }
                else
                {
                    atomic_weight = 10.0;
                    atomic_num = 5.0;
                }
            }

            temp1 = atomic_weight * atomic_num;

            temp = (2.0 * GlobalConstants.BETA_20 * Math.Pow(GlobalConstants.SOLAR_MASS_IN_GRAMS, (1.0 / 3.0)))
                   / (GlobalConstants.A1_20 * Math.Pow(temp1, (1.0 / 3.0)));

            temp2 = GlobalConstants.A2_20 * Math.Pow(atomic_weight, (4.0 / 3.0)) *
                    Math.Pow(GlobalConstants.SOLAR_MASS_IN_GRAMS, (2.0 / 3.0));
            temp2 = temp2 * Math.Pow(mass, (2.0 / 3.0));
            temp2 = temp2 / (GlobalConstants.A1_20 * Utilities.Pow2(atomic_num));
            temp2 = 1.0 + temp2;
            temp = temp / temp2;
            temp = (temp * Math.Pow(mass, (1.0 / 3.0))) / GlobalConstants.CM_PER_KM;

            temp /= GlobalConstants.JIMS_FUDGE; /* Make Earth = actual earth */

            return (temp);
        }

        /// <summary>
        /// Returns the the empirical density in grams/cc
        /// </summary>
        /// <param name="mass">Mass in units of solar masses</param>
        /// <param name="orbRadius">Radius in units of AU</param>
        /// <param name="rEcosphere"></param>
        /// <param name="isGasGiant"></param>
        /// <returns>Density in grams/cc</returns>
        public static double EmpiricalDensity(double mass, double orbRadius, double rEcosphere, bool isGasGiant)
        {
            double density = Math.Pow(mass * GlobalConstants.SUN_MASS_IN_EARTH_MASSES, (1.0 / 8.0));
            density *= Utilities.Pow1_4(rEcosphere / orbRadius);

            if (isGasGiant)
            {
                return (density * 1.2);
            }
            else
            {
                return (density * 5.5);
            }
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
        /// Returns a planet's period in Earth days
        /// </summary>
        /// <param name="separation">Separation in units of AU</param>
        /// <param name="smallMass">Units of solar masses</param>
        /// <param name="largeMass">Units of solar masses</param>
        /// <returns>Period in Earth days</returns>
        public static double Period(double separation, double smallMass, double largeMass)
        {
            double period_in_years;

            period_in_years = Math.Sqrt(Utilities.Pow3(separation) / (smallMass + largeMass));
            return (period_in_years * GlobalConstants.DAYS_IN_A_YEAR);
        }

        /// <summary>
        /// Provides an approximation of angular velocity for non-tidally decelerated
        /// planets.
        /// </summary>
        /// <param name="massSM">Mass of the body in solar masses</param>
        /// <param name="radiusKM">Radius of the body in km</param>
        /// <param name="isGasGiant">Is the body a gas giant?</param>
        /// <returns>Angular velocity in rad/sec</returns>
        public static double BaseAngularVelocity(double massSM, double radiusKM, bool isGasGiant)
        {
            // Fogg eq. 12

            var planetaryMassInGrams = massSM * GlobalConstants.SOLAR_MASS_IN_GRAMS;
            var equatorialRadiusInCM = radiusKM * GlobalConstants.CM_PER_KM;
            var k2 = isGasGiant ? 0.24 : 0.33;
            
            return Math.Sqrt(GlobalConstants.J * (planetaryMassInGrams) /
                             ((k2 / 2.0) * Utilities.Pow2(equatorialRadiusInCM)));
        }

        /// <summary>
        /// Provides an approximation of braking due to tidal forces as a ratio to the
        /// effect on Earth.
        /// </summary>
        /// <param name="densityGCC">Density of the body in grams/cm3</param>
        /// <param name="massSM">Mass of the body in solar masses</param>
        /// <param name="radiusKM">Radius of the body in KM</param>
        /// <param name="semiMajorAxisAU">Semi-major axis of the body's orbit in AU</param>
        /// <param name="starMassSM">Mass of the parent star in solar masses</param>
        /// <returns></returns>
        public static double ChangeInAngularVelocity(double densityGCC, double massSM, double radiusKM, double semiMajorAxisAU, double starMassSM)
        {
            // Fogg eq. 13 

            var planetaryMassInGrams = massSM * GlobalConstants.SOLAR_MASS_IN_GRAMS;
            var equatorialRadiusInCM = radiusKM * GlobalConstants.CM_PER_KM;

            return GlobalConstants.CHANGE_IN_EARTH_ANG_VEL * 
                   (densityGCC / GlobalConstants.EARTH_DENSITY) *
                   (equatorialRadiusInCM / GlobalConstants.EARTH_RADIUS) *
                   (GlobalConstants.EARTH_MASS_IN_GRAMS / planetaryMassInGrams) *
                   Math.Pow(starMassSM, 2.0) *
                   (1.0 / Math.Pow(semiMajorAxisAU, 6.0));
        }

        /// <summary>
        /// Returns an approximation of a planet's angular velocity in radians/sec
        /// </summary>
        /// <param name="massSM">Mass of the planet in solar masses</param>
        /// <param name="radiusKM">Radius of the planet in km</param>
        /// <param name="densityGCC">Density of the planet in grams/cm3</param>
        /// <param name="semiMajorAxisAU">Semi-major axis of the planet's orbit
        /// in AU</param>
        /// <param name="isGasGiant">Is the planet a gas giant?</param>
        /// <param name="starMassSM">Mass of the parent star in solar
        /// masses</param>
        /// <param name="starAgeYears">Age of the parent star in years</param>
        /// <returns>Angular velocity in radians/sec</returns>
        public static double AngularVelocity(double massSM, double radiusKM,
            double densityGCC, double semiMajorAxisAU, bool isGasGiant,
            double starMassSM, double starAgeYears)
        {
            var baseAngularVelocity = BaseAngularVelocity(massSM, radiusKM, isGasGiant);
            var changeInAngularVelocity = ChangeInAngularVelocity(
                densityGCC, massSM, radiusKM, semiMajorAxisAU, starMassSM);
            return baseAngularVelocity + (changeInAngularVelocity *
                                                         starAgeYears);
        }

        /// <summary>
        /// Returns an approximation of a planet's angular velocity in radians/sec
        /// </summary>
        /// <param name="planet">The planet to calculate angular velocity
        /// for</param>
        /// <returns>Angular velocity in radians/sec</returns>
        public static double AngularVelocity(Planet planet)
        {
            return AngularVelocity(planet.MassSM,
                planet.RadiusKM, planet.DensityGCC, planet.SemiMajorAxisAU,
                planet.IsGasGiant, planet.Star.Mass, planet.Star.AgeYears);
        }

        /// <summary>
        /// Returns the length of a planet's day in hours.
        /// </summary>
        /// <param name="angularVelocity">The planet's angular velocity in radians/sec</param>
        /// <param name="orbitalPeriodDays">The planet's orbital period in days</param>
        /// <param name="ecc">The eccentricity of the planet's orbit</param>
        /// <returns>Length of day in hours</returns>
        public static double DayLength(double angularVelocity, double orbitalPeriodDays, double ecc)
        {
            // Fogg's information for this routine came from Dole "Habitable Planets
            // for Man", Blaisdell Publishing Company, NY, 1964.  From this, he came
            // up with his eq.12, which is the equation for the 'base_angular_velocity'
            // below.  He then used an equation for the change in angular velocity per
            // time (dw/dt) from P. Goldreich and S. Soter's paper "Q in the Solar
            // System" in Icarus, vol 5, pp.375-389 (1966).	 Using as a comparison the
            // change in angular velocity for the Earth, Fogg has come up with an
            // approximation for our new planet (his eq.13) and take that into account.
            // This is used to find 'change_in_angular_velocity' below.

            var stopped = false;
            var dayInHours = GlobalConstants.RADIANS_PER_ROTATION / (GlobalConstants.SECONDS_PER_HOUR * angularVelocity);
            if (angularVelocity <= 0.0)
            {
                stopped = true;
                dayInHours = double.MaxValue;
            }

            var yearInHours = orbitalPeriodDays * 24.0;
            if (dayInHours >= yearInHours || stopped)
            {
                if (ecc > 0.1)
                {
                    var spinResonanceFactor = (1.0 - ecc) / (1.0 + ecc);
                    return spinResonanceFactor * yearInHours;
                }

                return yearInHours;
            }

            return dayInHours;
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
        public static double Inclination(double semiMajorAxisAU)
        {
            var inclination = (int)(Math.Pow(semiMajorAxisAU, 0.2) * 
                                    Utilities.About(GlobalConstants.EARTH_AXIAL_TILT, 0.4));
            return inclination % 360;
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
        public static double Acceleration(double mass, double radius)
        {
            return (GlobalConstants.GRAV_CONSTANT * (mass * GlobalConstants.SOLAR_MASS_IN_GRAMS) /
                               Utilities.Pow2(radius * GlobalConstants.CM_PER_KM));
        }

        /// <summary>
        /// Calculates the surface gravity of a planet.
        /// </summary>
        /// <param name="acceleration">Acceleration in units of cm/sec2</param>
        /// <returns>Gravity in units of Earth gravities</returns>
        public static double Gravity(double acceleration)
        {
            return acceleration / GlobalConstants.EARTH_ACCELERATION;
        }
        
        /// <summary>
        /// Calculates the inventory of volatiles in a planet's atmosphere
        /// as a result of outgassing. This value is used to calculate 
        /// the planet's surface pressure. Implements Fogg's eq. 17.
        /// </summary>
        /// <param name="mass">Planet mass in solar masses</param>
        /// <param name="escapeVelocity">Planet escape velocity in cm/sec</param>
        /// <param name="rmsVelocity">Planet RMS velocity in cm/sec</param>
        /// <param name="stellarMass">Mass of the planet's star in solar masses</param>
        /// <param name="zone">Planet's "zone" in the system</param>
        /// <param name="hasGreenhouseEffect">True if the planet is experiencing
        /// a runaway greenhouse effect</param>
        /// <param name="hasAccretedGas">True if the planet has accreted any
        /// gas</param>
        public static double VolatileInventory(double mass, double escapeVelocity, double rmsVelocity, double stellarMass, int zone, bool hasGreenhouseEffect, bool hasAccretedGas)
        {
            // This implements Fogg's eq.17.  The 'inventory' returned is unitless.

            var velocityRatio = escapeVelocity / rmsVelocity;
            if (velocityRatio >= GlobalConstants.GAS_RETENTION_THRESHOLD)
            {
                double proportionConst;
                switch (zone)
                {
                    case 1:
                        proportionConst = 100000.0;    /* 100 . 140 JLB */
                        break;
                    case 2:
                        proportionConst = 75000.0;
                        break;
                    case 3:
                        proportionConst = 250.0;
                        break;
                    default:
                        proportionConst = 0.0;
                        break;
                }
                var earthUnits = mass * GlobalConstants.SUN_MASS_IN_EARTH_MASSES;
                var volInv = Utilities.About((proportionConst * earthUnits) / stellarMass, 0.2);
                if (hasGreenhouseEffect || hasAccretedGas)
                {
                    return volInv;
                }
                return volInv / 100.0; /* 100 . 140 JLB */
            }

            return 0.0;
        }

        /// <summary>
        /// Returns pressure in units of millibars. This implements Fogg's eq.18.
        /// </summary>
        /// <param name="volatileGasInventory"></param>
        /// <param name="equatorialRadius">Radius in km</param>
        /// <param name="gravity">Gravity in units of Earth gravities</param>
        /// <returns>Pressure in millibars (mb)</returns>
        public static double Pressure(double volatileGasInventory, double equatorialRadius, double gravity)
        {
            //  JLB: Aparently this assumed that earth pressure = 1000mb. I've added a
            //	fudge factor (EARTH_SURF_PRES_IN_MILLIBARS / 1000.) to correct for that

            equatorialRadius = GlobalConstants.KM_EARTH_RADIUS / equatorialRadius;
            return (volatileGasInventory * gravity *
                    (GlobalConstants.EARTH_SURF_PRES_IN_MILLIBARS / 1000.0) /
                    Utilities.Pow2(equatorialRadius));
        }

        /// <summary>
        /// Returns the boiling point of water using Fogg's eq.21.
        /// </summary>
        /// <param name="surfacePressure">Atmospheric pressure in millibars (mb)</param>
        /// <returns>Boiling point of water in Kelvin.</returns>
        public static double BoilingPoint(double surfacePressure)
        {
            var surfacePressureInBars = surfacePressure / GlobalConstants.MILLIBARS_PER_BAR;
            return (1.0 / ((Math.Log(surfacePressureInBars) / -5050.5) + (1.0 / 373.0)));
        }

        /// <summary>
        /// This function is Fogg's eq.22.	 Given the volatile gas inventory and
        /// planetary radius of a planet (in Km), this function returns the
        /// fraction of the planet covered with water.	
        /// </summary>
        /// <param name="volatileGasInventory"></param>
        /// <param name="planetRadius">Radius in km</param>
        /// <returns>Fraction of the planet covered in water</returns>
        public static double HydroFraction(double volatileGasInventory, double planetRadius)
        {
            var temp = (0.71 * volatileGasInventory / 1000.0)
                     * Utilities.Pow2(GlobalConstants.KM_EARTH_RADIUS / planetRadius);
            return temp >= 1.0 ? 1.0 : temp;
        }

        /// <summary>
        /// Returns the fraction of cloud cover available.
        /// </summary>
        public static double CloudFraction(double surfaceTemp, double smallestMWRetained, double equatorialRadius, double hydroFraction)
        {
            //	 Given the surface temperature of a planet (in Kelvin), this function
            //	 returns the fraction of cloud cover available.	 This is Fogg's eq.23.
            //	 See Hart in "Icarus" (vol 33, pp23 - 39, 1978) for an explanation.
            //	 This equation is Hart's eq.3.	
            //	 I have modified it slightly using constants and relationships from
            //	 Glass's book "Introduction to Planetary Geology", p.46.
            //	 The 'CLOUD_COVERAGE_FACTOR' is the amount of surface area on Earth	
            //	 covered by one Kg. of cloud.

            if (smallestMWRetained > GlobalConstants.WATER_VAPOR)
            {
                return 0.0;
            }
            
            var surfArea = 4.0 * Math.PI * Utilities.Pow2(equatorialRadius);
            var hydroMass = hydroFraction * surfArea * GlobalConstants.EARTH_WATER_MASS_PER_AREA;
            var waterVaporKg = (0.00000001 * hydroMass) *
                                Math.Exp(GlobalConstants.Q2_36 * (surfaceTemp - GlobalConstants.EARTH_AVERAGE_KELVIN));
            var fraction = GlobalConstants.CLOUD_COVERAGE_FACTOR * waterVaporKg / surfArea;
            return fraction >= 1.0 ? 1.0 : fraction;
        }

        /// <summary>
        /// Given the surface temperature of a planet (in Kelvin), this function
        ///	returns the fraction of the planet's surface covered by ice.
        /// </summary>
        /// <returns>Fraction of the planet's surface covered in ice</returns>
        public static double IceFraction(double hydroFraction, double surfTemp)
        {
            // This is Fogg's eq.24. See Hart[24] in Icarus vol.33, p.28 for an explanation.
            // I have changed a constant from 70 to 90 in order to bring it more in	
            // line with the fraction of the Earth's surface covered with ice, which	
            // is approximatly .016 (=1.6%).											

            if (surfTemp > 328.0)
            {
                surfTemp = 328.0;
            }
            var temp = Math.Pow(((328.0 - surfTemp) / 90.0), 5.0);
            if (temp > (1.5 * hydroFraction))
            {
                temp = (1.5 * hydroFraction);
            }

            return temp >= 1.0 ? 1.0 : temp;
        }

        // TODO look up Fogg's eq.19. and write a summary
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ecosphereRadius">Ecosphere radius in AU</param>
        /// <param name="orbitalRadius">Orbital radius in AU</param>
        /// <param name="albedo"></param>
        /// <returns>Temperature in Kelvin</returns>
        public static double EffTemp(double ecosphereRadius, double orbitalRadius, double albedo)
        {
            // This is Fogg's eq.19.
            return Math.Sqrt(ecosphereRadius / orbitalRadius)
                  * Utilities.Pow1_4((1.0 - albedo) / (1.0 - GlobalConstants.EARTH_ALBEDO))
                  * GlobalConstants.EARTH_EFFECTIVE_TEMP;
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
        /// Calculates whether or not a planet is suffering from a runaway greenhouse
        /// effect.
        /// </summary>
        /// <param name="ecosphereRadius">Radius of the star's ecosphere in AU</param>
        /// <param name="orbitalRadius">Orbital radius of the planet in AU</param>
        /// <returns>True if the planet is suffering a runaway greenhouse effect.</returns>
        public static bool Greenhouse(double ecosphereRadius, double orbitalRadius)
        {
            // Old grnhouse:  
            //	Note that if the orbital radius of the planet is greater than or equal
            //	to R_inner, 99% of it's volatiles are assumed to have been deposited in
            //	surface reservoirs (otherwise, it suffers from the greenhouse effect).

            //	if ((orb_radius < r_greenhouse) && (zone == 1)) 

            //	The new definition is based on the inital surface temperature and what
            //	state water is in. If it's too hot, the water will never condense out
            //	of the atmosphere, rain down and form an ocean. The albedo used here
            //	was chosen so that the boundary is about the same as the old method	
            //	Neither zone, nor r_greenhouse are used in this version				JLB

            // TODO Reevaluate this method since it apparently only considers water vapor as a greenhouse gas. 
            // GL

            var temp = EffTemp(ecosphereRadius, orbitalRadius, GlobalConstants.GREENHOUSE_TRIGGER_ALBEDO);
            return temp > GlobalConstants.FREEZING_POINT_OF_WATER;
        }

        /// <summary>
        /// Returns the rise in temperature produced by the greenhouse effect.
        /// </summary>
        /// <param name="opticalDepth"></param>
        /// <param name="effectiveTemp">Effective temperature in units of Kelvin</param>
        /// <param name="surfPressure"></param>
        /// <returns>Rise in temperature in Kelvin</returns>
        public static double GreenRise(double opticalDepth, double effectiveTemp, double surfPressure)
        {
            //	This is Fogg's eq.20, and is also Hart's eq.20 in his "Evolution of
            //	Earth's Atmosphere" article.  The effective temperature given is in
            //	units of Kelvin, as is the rise in temperature produced by the
            //	greenhouse effect, which is returned.

            var convectionFactor = GlobalConstants.EARTH_CONVECTION_FACTOR * Math.Pow(surfPressure / GlobalConstants.EARTH_SURF_PRES_IN_MILLIBARS, 0.25);
            var rise = (Utilities.Pow1_4(1.0 + 0.75 * opticalDepth) - 1.0) *
                               effectiveTemp * convectionFactor;

            if (rise < 0.0) rise = 0.0;

            return rise;
        }
        
        /// <summary>
        /// Calculates the albedo of a planetary body.
        /// </summary>
        /// <param name="waterFraction">Fraction of surface covered by water.</param>
        /// <param name="cloudFraction">Fraction of planet covered by clouds.</param>
        /// <param name="iceFraction">Fraction of planet covered by ice.</param>
        /// <param name="surfPressure">Surface pressure in mb.</param>
        /// <returns>Average overall albedo of the body.</returns>
        public static double PlanetAlbedo(double waterFraction, double cloudFraction, double iceFraction, double surfPressure)
        {
            // The surface temperature passed in is in units of Kelvin.
            // The cloud adjustment is the fraction of cloud cover obscuring each
            // of the three major components of albedo that lie below the clouds.

            double rock_fraction, cloud_adjustment, components, cloud_part,
            rock_part, water_part, ice_part;

            rock_fraction = 1.0 - waterFraction - iceFraction;
            components = 0.0;
            if (waterFraction > 0.0)
            {
                components = components + 1.0;
            }
            if (iceFraction > 0.0)
            {
                components = components + 1.0;
            }
            if (rock_fraction > 0.0)
            {
                components = components + 1.0;
            }

            cloud_adjustment = cloudFraction / components;

            if (rock_fraction >= cloud_adjustment)
            {
                rock_fraction = rock_fraction - cloud_adjustment;
            }
            else
            {
                rock_fraction = 0.0;
            }

            if (waterFraction > cloud_adjustment)
            {
                waterFraction = waterFraction - cloud_adjustment;
            }
            else
            {
                waterFraction = 0.0;
            }

            if (iceFraction > cloud_adjustment)
            {
                iceFraction = iceFraction - cloud_adjustment;
            }
            else
            {
                iceFraction = 0.0;
            }

            cloud_part = cloudFraction * GlobalConstants.CLOUD_ALBEDO;     /* about(...,0.2); */

            if (surfPressure == 0.0)
            {
                rock_part = rock_fraction * GlobalConstants.ROCKY_AIRLESS_ALBEDO;   /* about(...,0.3); */
                ice_part = iceFraction * GlobalConstants.AIRLESS_ICE_ALBEDO;       /* about(...,0.4); */
                water_part = 0;
            }
            else
            {
                rock_part = rock_fraction * GlobalConstants.ROCKY_ALBEDO;   /* about(...,0.1); */
                water_part = waterFraction * GlobalConstants.WATER_ALBEDO; /* about(...,0.2); */
                ice_part = iceFraction * GlobalConstants.ICE_ALBEDO;       /* about(...,0.1); */
            }

            return cloud_part + rock_part + water_part + ice_part;
        }

        /// <summary>
        /// Returns the dimensionless quantity of optical depth, which is useful in determing the amount
        /// of greenhouse effect on a planet.
        /// </summary>
        /// <param name="molecularWeight"></param>
        /// <param name="surfPressure"></param>
        /// <returns></returns>
        public static double Opacity(double molecularWeight, double surfPressure)
        {
            var opticalDepth = 0.0;
            if ((molecularWeight >= 0.0) && (molecularWeight < 10.0))
            {
                opticalDepth = opticalDepth + 3.0;
            }

            if ((molecularWeight >= 10.0) && (molecularWeight < 20.0))
            {
                opticalDepth = opticalDepth + 2.34;
            }

            if ((molecularWeight >= 20.0) && (molecularWeight < 30.0))
            {
                opticalDepth = opticalDepth + 1.0;
            }

            if ((molecularWeight >= 30.0) && (molecularWeight < 45.0))
            {
                opticalDepth = opticalDepth + 0.15;
            }

            if ((molecularWeight >= 45.0) && (molecularWeight < 100.0))
            {
                opticalDepth = opticalDepth + 0.05;
            }

            if (surfPressure >= (70.0 * GlobalConstants.EARTH_SURF_PRES_IN_MILLIBARS))
            {
                opticalDepth = opticalDepth * 8.333;
            }
            else if (surfPressure >= (50.0 * GlobalConstants.EARTH_SURF_PRES_IN_MILLIBARS))
            {
                opticalDepth = opticalDepth * 6.666;
            }
            else if (surfPressure >= (30.0 * GlobalConstants.EARTH_SURF_PRES_IN_MILLIBARS))
            {
                opticalDepth = opticalDepth * 3.333;
            }
            else if (surfPressure >= (10.0 * GlobalConstants.EARTH_SURF_PRES_IN_MILLIBARS))
            {
                opticalDepth = opticalDepth * 2.0;
            }
            else if (surfPressure >= (5.0 * GlobalConstants.EARTH_SURF_PRES_IN_MILLIBARS))
            {
                opticalDepth = opticalDepth * 1.5;
            }

            return opticalDepth;
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
        public static double MinMolecularWeight(Planet planet)
        {
            var surfGrav = planet.SurfaceGravityG;
            var mass = planet.MassSM;
            var radius = planet.RadiusKM;
            var exosphereTemp = planet.ExosphereTempKelvin;
            var temp = planet.ExosphereTempKelvin;
            var target = 5.0E9;

            var guess1 = MoleculeLimit(mass, radius, temp);
            var guess2 = guess1;

            var life = GasLife(guess1, exosphereTemp, surfGrav, radius);

            var loops = 0;

            if (planet.Star != null)
            {
                target = planet.Star.AgeYears;
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
        }

        /// <summary>
        /// The temperature of the planet calculated in degrees Kelvin
        /// </summary>
        /// <param name="planet"></param>
        /// <param name="first"></param>
        /// <param name="last_water"></param>
        /// <param name="last_clouds"></param>
        /// <param name="last_ice"></param>
        /// <param name="last_temp"></param>
        /// <param name="last_albedo"></param>
        public static void CalculateSurfaceTemperature(ref Planet planet, bool first, double last_water, double last_clouds, double last_ice, double last_temp, double last_albedo)
        {
            double effectiveTemp;
            double waterRaw;
            double cloudsRaw;
            double greenhouseTemp;
            bool boilOff = false;

            if (first)
            {
                planet.Albedo = GlobalConstants.EARTH_ALBEDO;

                effectiveTemp = EffTemp(planet.Star.EcosphereRadiusAU, planet.SemiMajorAxisAU, planet.Albedo);
                greenhouseTemp = GreenRise(Opacity(planet.MolecularWeightRetained,
                                                         planet.Atmosphere.SurfacePressure),
                                                 effectiveTemp,
                                                 planet.Atmosphere.SurfacePressure);
                planet.SurfaceTempKelvin = effectiveTemp + greenhouseTemp;

                SetTempRange(ref planet);
            }

            if (planet.HasGreenhouseEffect && planet.MaxTempKelvin < planet.BoilingPointWaterKelvin)
            {
                planet.HasGreenhouseEffect = false;

                planet.VolatileGasInventory = VolatileInventory(planet.MassSM,
                    planet.EscapeVelocityCMSec, planet.RMSVelocityCMSec, planet.Star.Mass,
                    planet.OrbitZone, planet.HasGreenhouseEffect, (planet.GasMassSM / planet.MassSM) > 0.000001);
                planet.Atmosphere.SurfacePressure = Pressure(planet.VolatileGasInventory, planet.RadiusKM, planet.SurfaceGravityG);

                planet.BoilingPointWaterKelvin = BoilingPoint(planet.Atmosphere.SurfacePressure);
            }

            waterRaw = planet.WaterCoverFraction = HydroFraction(planet.VolatileGasInventory, planet.RadiusKM);
            cloudsRaw = planet.CloudCoverFraction = CloudFraction(planet.SurfaceTempKelvin,
                                                     planet.MolecularWeightRetained,
                                                     planet.RadiusKM,
                                                     planet.WaterCoverFraction);
            planet.IceCoverFraction = IceFraction(planet.WaterCoverFraction, planet.SurfaceTempKelvin);

            if ((planet.HasGreenhouseEffect) && (planet.Atmosphere.SurfacePressure > 0.0))
            {
                planet.CloudCoverFraction = 1.0;
            }

            if ((planet.DaytimeTempKelvin >= planet.BoilingPointWaterKelvin) && (!first) && !(IsTidallyLocked(planet) || planet.HasResonantPeriod))
            {
                planet.WaterCoverFraction = 0.0;
                boilOff = true;

                if (planet.MolecularWeightRetained > GlobalConstants.WATER_VAPOR)
                {
                    planet.CloudCoverFraction = 0.0;
                }
                else
                {
                    planet.CloudCoverFraction = 1.0;
                }
            }

            if (planet.SurfaceTempKelvin < (GlobalConstants.FREEZING_POINT_OF_WATER - 3.0))
            {
                planet.WaterCoverFraction = 0.0;
            }

            planet.Albedo = PlanetAlbedo(planet.WaterCoverFraction, planet.CloudCoverFraction, planet.IceCoverFraction, planet.Atmosphere.SurfacePressure);

            effectiveTemp = EffTemp(planet.Star.EcosphereRadiusAU, planet.SemiMajorAxisAU, planet.Albedo);
            greenhouseTemp = GreenRise(
                Opacity(planet.MolecularWeightRetained, planet.Atmosphere.SurfacePressure),
                effectiveTemp, planet.Atmosphere.SurfacePressure);
            planet.SurfaceTempKelvin = effectiveTemp + greenhouseTemp;

            if (!first)
            {
                if (!boilOff)
                {
                    planet.WaterCoverFraction = (planet.WaterCoverFraction + (last_water * 2)) / 3;
                }
                planet.CloudCoverFraction = (planet.CloudCoverFraction + (last_clouds * 2)) / 3;
                planet.IceCoverFraction = (planet.IceCoverFraction + (last_ice * 2)) / 3;
                planet.Albedo = (planet.Albedo + (last_albedo * 2)) / 3;
                planet.SurfaceTempKelvin = (planet.SurfaceTempKelvin + (last_temp * 2)) / 3;
            }

            SetTempRange(ref planet);
        }

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
        
        /// <summary>
        /// Returns the breathability state of the planet's atmosphere.
        /// </summary>
        /// <returns></returns>
        public static Breathability Breathability(Planet planet)
        {
            // This function uses figures on the maximum inspired partial pressures
            // of Oxygen, other atmospheric and traces gases as laid out on pages 15,
            // 16 and 18 of Dole's Habitable Planets for Man to derive breathability
            // of the planet's atmosphere.                                       JLB

            if (planet == null)
            {
                throw new ArgumentNullException();
            }

            var oxygenOk = false;

            if (planet.Atmosphere.Composition.Count == 0)
            {
                return Data.Breathability.None;
            }

            var poisonous = false;
            planet.Atmosphere.PoisonousGases.Clear();
            for (var index = 0; index < planet.Atmosphere.Composition.Count; index++)
            {
                var gas = planet.Atmosphere.Composition[index];

                var ipp = InspiredPartialPressure(planet.Atmosphere.SurfacePressure, planet.Atmosphere.Composition[index].SurfacePressure);
                if (ipp > gas.GasType.MaxIpp)
                {
                    poisonous = true;
                    planet.Atmosphere.PoisonousGases.Add(gas);
                }

                // TODO why not just have a min_ipp for every gas, even if it's going to be zero for everything that's not oxygen?
                if (gas.GasType.Num == GlobalConstants.AN_O)
                {
                    oxygenOk = ((ipp >= GlobalConstants.MIN_O2_IPP) && (ipp <= GlobalConstants.MAX_O2_IPP));
                }
            }

            if (poisonous)
            {
                return Data.Breathability.Poisonous;
            }
            return oxygenOk ? Data.Breathability.Breathable : Data.Breathability.Unbreathable;
        }

        // TODO write summary
        // TODO parameter for number of iterations? does it matter?
        /// <summary>
        /// 
        /// </summary>
        /// <param name="planet"></param>
        public static void IterateSurfaceTemp(ref Planet planet)
        {
            var initTemp = EstTemp(planet.Star.EcosphereRadiusAU, planet.SemiMajorAxisAU, planet.Albedo);

            //var h2Life = GasLife(GlobalConstants.MOL_HYDROGEN, planet);
            //var h2oLife = GasLife(GlobalConstants.WATER_VAPOR, planet);
            //var n2Life = GasLife(GlobalConstants.MOL_NITROGEN, planet);
            //var nLife = GasLife(GlobalConstants.ATOMIC_NITROGEN, planet);

            CalculateSurfaceTemperature(ref planet, true, 0, 0, 0, 0, 0);

            for (var count = 0; count <= 25; count++)
            {
                var lastWater = planet.WaterCoverFraction;
                var lastClouds = planet.CloudCoverFraction;
                var lastIce = planet.IceCoverFraction;
                var lastTemp = planet.SurfaceTempKelvin;
                var lastAlbedo = planet.Albedo;

                CalculateSurfaceTemperature(ref planet, false, lastWater, lastClouds, lastIce, lastTemp, lastAlbedo);

                if (Math.Abs(planet.SurfaceTempKelvin - lastTemp) < 0.25)
                    break;
            }

            planet.GreenhouseRiseKelvin = planet.SurfaceTempKelvin - initTemp;
        }

        private static double Lim(double x)
        {
            return x / Math.Sqrt(Math.Sqrt(1 + x * x * x * x));
        }

        private static double Soft(double v, double max, double min)
        {
            double dv = v - min;
            double dm = max - min;
            return (Lim(2 * dv / dm - 1) + 1) / 2 * dm + min;
        }

        private static void SetTempRange(ref Planet planet)
        {
            var pressmod = 1 / Math.Sqrt(1 + 20 * planet.Atmosphere.SurfacePressure / 1000.0);
            var ppmod = 1 / Math.Sqrt(10 + 5 * planet.Atmosphere.SurfacePressure / 1000.0);
            var tiltmod = Math.Abs(Math.Cos(planet.AxialTiltDegrees * Math.PI / 180) * Math.Pow(1 + planet.Eccentricity, 2));
            var daymod = 1 / (200 / planet.DayLengthHours + 1);
            var mh = Math.Pow(1 + daymod, pressmod);
            var ml = Math.Pow(1 - daymod, pressmod);
            var hi = mh * planet.SurfaceTempKelvin;
            var lo = ml * planet.SurfaceTempKelvin;
            var sh = hi + Math.Pow((100 + hi) * tiltmod, Math.Sqrt(ppmod));
            var wl = lo - Math.Pow((150 + lo) * tiltmod, Math.Sqrt(ppmod));
            var max = planet.SurfaceTempKelvin + Math.Sqrt(planet.SurfaceTempKelvin) * 10;
            var min = planet.SurfaceTempKelvin / Math.Sqrt(planet.DayLengthHours + 24);

            if (lo < min) lo = min;
            if (wl < 0) wl = 0;

            planet.DaytimeTempKelvin = Soft(hi, max, min);
            planet.NighttimeTempKelvin = Soft(lo, max, min);
            planet.MaxTempKelvin = Soft(sh, max, min);
            planet.MinTempKelvin = Soft(wl, max, min);
        }
    }
}
