using System;
using System.Collections.Generic;
using System.Text;
using Primoris.Universe.Stargen.Bodies;
using UnitsNet;


namespace Primoris.Universe.Stargen.Astrophysics.Burrows
{

	public class BodyPhysics : IScienceAstrophysics, IScienceDynamics, IScienceThermodynamics, ISciencePhysics, IScienceAstronomy, ISciencePlanetology
	{
		public IScienceAstronomy Astronomy => this;

		public IScienceDynamics Dynamics => this;

		public ISciencePhysics Physics => this;

		public ISciencePlanetology Planetology => this;

		public IScienceThermodynamics Thermodynamics => this;

		/// <summary>
		/// Fogg eq. 12
		/// </summary>
		/// <param name="massSM"></param>
		/// <param name="radiusKM"></param>
		/// <param name="isGasGiant"></param>
		/// <returns></returns>
		public virtual RotationalSpeed GetBaseAngularVelocity(Mass massSM,
									   Length radiusKM,
									   bool isGasGiant)
		{
			var planetaryMassInGrams = massSM.Grams; //* GlobalConstants.SOLAR_MASS_IN_GRAMS;
			var equatorialRadiusInCM = radiusKM.Centimeters; //* GlobalConstants.CM_PER_KM;
			var k2 = isGasGiant ? 0.24 : 0.33;

			return RotationalSpeed.FromRadiansPerSecond(Math.Sqrt(GlobalConstants.J * planetaryMassInGrams /
							 (k2 / 2.0 * Utilities.Pow2(equatorialRadiusInCM))));
		}

		public virtual RotationalSpeed GetChangeInAngularVelocity(Density densityGCC,
										   Mass massSM,
										   Length radiusKM,
										   Length semiMajorAxisAU,
										   Mass starMassSM)
		{
			// Fogg eq. 13 

			var planetaryMassInGrams = massSM.Grams; // * GlobalConstants.SOLAR_MASS_IN_GRAMS;
			var equatorialRadiusInCM = radiusKM.Centimeters; // * GlobalConstants.CM_PER_KM;

			return RotationalSpeed.FromRadiansPerSecond(GlobalConstants.CHANGE_IN_EARTH_ANG_VEL *
				   (densityGCC.GramsPerCubicCentimeter / GlobalConstants.EARTH_DENSITY) *
				   (equatorialRadiusInCM / GlobalConstants.EARTH_RADIUS) *
				   (GlobalConstants.EARTH_MASS_IN_GRAMS / planetaryMassInGrams) *
				   Math.Pow(starMassSM.SolarMasses, 2.0) *
				   (1.0 / Math.Pow(semiMajorAxisAU.AstronomicalUnits, 6.0)));
		}

		public virtual RotationalSpeed GetAngularVelocity(Mass massSM,
								   Length radiusKM,
								   Density densityGCC,
								   Length semiMajorAxisAU,
								   bool isGasGiant,
								   Mass starMassSM,
								   Duration starAgeYears)
		{
			var baseAngularVelocity = GetBaseAngularVelocity(massSM, radiusKM, isGasGiant);
			var changeInAngularVelocity = GetChangeInAngularVelocity(
				densityGCC, massSM, radiusKM, semiMajorAxisAU, starMassSM);
			return baseAngularVelocity + changeInAngularVelocity * starAgeYears.Years365;
		}


		/// <remarks>
		/// Originally this temperature was clipped at the Star surface temperature, ie it doesn't make sense for the
		/// exosphere temperature to be significally higher than the Star temperature. But it turns out that doing so
		/// make the generator creating only Barren and Gas Giant planets. This will have to be investigated and corrected.
		/// Validation with StarformNET shows that the exo temp can go really high, sometimes on the 30k Kelvin.
		/// 
		/// TODO: Make sure Exosphere Temperature makes sense and understand why it needs to be verrry high for the generator to
		/// create habitable planets.
		/// </remarks>
		/// <param name="semiMajorAxisAu"></param>
		/// <param name="ecosphereRadiusAU"></param>
		/// <param name="sunTemperature"></param>
		/// <returns></returns>
		public virtual Temperature GetExosphereTemperature(Length semiMajorAxisAu,
										Length ecosphereRadiusAU,
										Temperature sunTemperature)
		{
			var exoTemp = GlobalConstants.EARTH_EXOSPHERE_TEMP / Utilities.Pow2(semiMajorAxisAu / ecosphereRadiusAU);
			// Exosphere temperature can't be realisticly higher than the surface temperature of the sun. We therefore clip at sun.Temperature.
			// TODO: Make transition instead of brute clipping.
			//exoTemp = exoTemp > sunTemperature ? sunTemperature : exoTemp;
			// Exosphere can't be realisticly lower than the CMB radiation temperature, which is 2.7K. 
			// TODO: Make transition instead of brute clipping.
			//exoTemp = exoTemp < GlobalConstants.VACCUM_TEMPERATURE ? GlobalConstants.VACCUM_TEMPERATURE : exoTemp;

			return Temperature.FromKelvins(exoTemp);
		}

		/// <summary>
		/// This is Fogg's eq.16.  The molecular weight (usually assumed to be N2)
		/// is used as the basis of the Root Mean Square (RMS) velocity of the
		/// molecule or atom.
		/// https://en.wikipedia.org/wiki/Root-mean-square_speed
		/// </summary>
		/// <param name="exoTemp"></param>
		/// <returns></returns>
		public virtual Speed GetRMSVelocity(Mass molecularWeight, Temperature exoTemp)
		{
			//return Environment.RMSVelocity(GlobalConstants.MOL_NITROGEN, exoTemp);

			return Speed.FromMetersPerSecond(Math.Sqrt((3.0 * GlobalConstants.MOLAR_GAS_CONST * exoTemp.Kelvins) / molecularWeight.Grams));
		}

		/// <summary>
		/// This formula is listed as eq.9 in Fogg's article, although some typos
		/// crop up in that eq.  See "The Internal Constitution of Planets", by
		/// Dr. D. S. Kothari, Mon. Not. of the Royal Astronomical Society, vol 96
		/// pp.833-843, 1936 for the derivation.  Specifically, this is Kothari's
		/// eq.23, which appears on page 840.
		/// http://articles.adsabs.harvard.edu//full/1936MNRAS..96..833K/0000840.000.html
		/// </summary>
		/// <param name="massSM"></param>
		/// <param name="orbitZone"></param>
		/// <param name="giant"></param>
		/// <returns></returns>
		public virtual Length GetCoreRadius(Mass massSM,
						  int orbitZone,
						  bool giant)
		{
			// This formula is listed as eq.9 in Fogg's article, although some typos
			// crop up in that eq.  See "The Internal Constitution of Planets", by
			// Dr. D. S. Kothari, Mon. Not. of the Royal Astronomical Society, vol 96
			// pp.833-843, 1936 for the derivation.  Specifically, this is Kothari's
			// eq.23, which appears on page 840.
			// http://articles.adsabs.harvard.edu//full/1936MNRAS..96..833K/0000840.000.html

			double temp1;
			double temp, temp2, atomic_weight, atomic_num;

			if (orbitZone == 1)
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
			else if (orbitZone == 2)
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

			temp = 2.0 * GlobalConstants.BETA_20 * Math.Pow(GlobalConstants.SOLAR_MASS_IN_GRAMS, 1.0 / 3.0)
				   / (GlobalConstants.A1_20 * Math.Pow(temp1, 1.0 / 3.0));

			temp2 = GlobalConstants.A2_20 * Math.Pow(atomic_weight, 4.0 / 3.0) *
					Math.Pow(GlobalConstants.SOLAR_MASS_IN_GRAMS, 2.0 / 3.0);
			temp2 = temp2 * Math.Pow(massSM.SolarMasses, 2.0 / 3.0);
			temp2 = temp2 / (GlobalConstants.A1_20 * Utilities.Pow2(atomic_num));
			temp2 = 1.0 + temp2;
			temp = temp / temp2;
			temp = temp * Math.Pow(massSM.SolarMasses, 1.0 / 3.0) / GlobalConstants.CM_PER_KM;

			temp /= GlobalConstants.JIMS_FUDGE; /* Make Earth = actual earth */

			return Length.FromKilometers(temp);
		}

		public virtual Density GetDensityFromStar(Mass massSM,
								   Length semiMajorAxisAU,
								   Length ecosphereRadiusAU,
								   bool isGasGiant)
		{
			double density = Math.Pow(massSM.SolarMasses * GlobalConstants.SUN_MASS_IN_EARTH_MASSES, 1.0 / 8.0);
			density *= Utilities.Pow1_4(ecosphereRadiusAU / semiMajorAxisAU);

			if (isGasGiant)
			{
				return Density.FromGramsPerCubicCentimeter(density * 1.2);
			}
			else
			{
				return Density.FromGramsPerCubicCentimeter(density * 5.5);
			}
		}

		public virtual Density GetDensityFromBody(Mass massSM,
								   Length radius)
		{
			//return Environment.VolumeDensity(massSM, radius);

			var mass = massSM.SolarMasses * GlobalConstants.SOLAR_MASS_IN_GRAMS;
			var equatRadius = radius.Kilometers * GlobalConstants.CM_PER_KM;
			double volume = (4.0 * Math.PI * Utilities.Pow3(equatRadius)) / 3.0;
			return Density.FromGramsPerCubicCentimeter(mass / volume);
		}

		public virtual bool TestIsGasGiant(Mass massSM,
							 Mass gasMassSM,
							 Mass molecularWeightRetained)
		{
			if (massSM == gasMassSM)
				return true;

			return massSM.SolarMasses * GlobalConstants.SUN_MASS_IN_EARTH_MASSES > 1.0 && gasMassSM.SolarMasses / massSM.SolarMasses > 0.05 && molecularWeightRetained.Grams <= 4.0;
		}

		/// <summary>
		/// Old grnhouse:  
		///	Note that if the orbital radius of the planet is greater than or equal
		///	to R_inner, 99% of it's volatiles are assumed to have been deposited in
		///	surface reservoirs (otherwise, it suffers from the greenhouse effect).
		///
		///	if ((orb_radius < r_greenhouse) && (zone == 1)) 
		///
		///	The new definition is based on the inital surface temperature and what
		///	state water is in. If it's too hot, the water will never condense out
		///	of the atmosphere, rain down and form an ocean. The albedo used here
		///	was chosen so that the boundary is about the same as the old method	
		///	Neither zone, nor r_greenhouse are used in this version				JLB
		///
		/// TODO Reevaluate this method since it apparently only considers water vapor as a greenhouse gas. 
		/// GL
		/// </summary>
		/// <param name="ecosphereRadius"></param>
		/// <param name="semiAxisMajorAU"></param>
		/// <returns></returns>
		public virtual bool TestHasGreenhouseEffect(Length ecosphereRadius,
									  Length semiAxisMajorAU)
		{
			var temp = GetEffectiveTemperature(ecosphereRadius, semiAxisMajorAU, Ratio.FromDecimalFractions(GlobalConstants.GREENHOUSE_TRIGGER_ALBEDO));
			return temp.Kelvins > GlobalConstants.FREEZING_POINT_OF_WATER;
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
		/// <param name="hasAccretedGas">True if the planet has accreted any</param>
		public virtual Ratio GetVolatileGasInventory(Mass massSM,
												   Speed escapeVelocity,
												   Speed rmsVelocity,
												   Mass sunMass,
												   Mass gasMassSM,
												   int orbitZone,
												   bool hasGreenhouse)
		{
			var hasAccretedGas = gasMassSM / massSM > 0.000001;

			// This implements Fogg's eq.17.  The 'inventory' returned is unitless.

			var velocityRatio = escapeVelocity / rmsVelocity;
			if (velocityRatio >= GlobalConstants.GAS_RETENTION_THRESHOLD)
			{
				double proportionConst;
				switch (orbitZone)
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
				var earthUnits = massSM * GlobalConstants.SUN_MASS_IN_EARTH_MASSES;
				var volInv = Utilities.About((proportionConst * earthUnits) / sunMass, 0.2);
				if (hasGreenhouse || hasAccretedGas)
				{
					return Ratio.FromDecimalFractions(volInv);
				}
				return Ratio.FromDecimalFractions(volInv / 100.0); /* 100 . 140 JLB */
			}

			return Ratio.FromDecimalFractions(0.0);
		}

		public virtual Mass GetMolecularWeightRetained(Acceleration surfGrav,
												   Mass mass,
												   Length radius,
												   Temperature exosphereTemp,
												   Duration sunAge)
		{
			var temp = exosphereTemp;

			var guess1 = GetInitialMolecularWeightRetained(mass, radius, temp);
			var guess2 = guess1;

			var life = GetGasLife(guess1, exosphereTemp, surfGrav, radius);

			var loops = 0;

			var target = sunAge;

			if (life > target)
			{
				while (life > target && loops++ < 25)
				{
					guess1 = guess1 / 2.0;
					life = GetGasLife(guess1, exosphereTemp, surfGrav, radius);
				}
			}
			else
			{
				while (life < target && loops++ < 25)
				{
					guess2 = guess2 * 2.0;
					life = GetGasLife(guess2, exosphereTemp, surfGrav, radius);
				}
			}

			loops = 0;

			while (guess2.Grams - guess1.Grams > 0.1 && loops++ < 25)
			{
				var guess3 = (guess1 + guess2) / 2.0;
				life = GetGasLife(guess3, exosphereTemp, surfGrav, radius);

				if (life < target)
				{
					guess1 = guess3;
				}
				else
				{
					guess2 = guess3;
				}
			}

			//life = GetGasLife(guess2, exosphereTemp, surfGrav, radius);

			return guess2;
		}

		public virtual Length GetHillSphere(Mass bigMass,
									  Mass smallMassSM,
									  Length semiMajorAxisAU)
		{
			return semiMajorAxisAU * Math.Pow(smallMassSM / (3 * bigMass), 1.0 / 3.0); //* GlobalConstants.KM_PER_AU;
		}

		/// <summary>
		/// Returns pressure in units of millibars. This implements Fogg's eq.18.
		/// </summary>
		/// <remarks¨>
		/// JLB: Aparently this assumed that earth pressure = 1000mb. I've added a
		///	fudge factor (EARTH_SURF_PRES_IN_MILLIBARS / 1000.) to correct for that.
		/// </remarks>
		/// <param name="volatileGasInventory"></param>
		/// <param name="equatorialRadius">Radius in km</param>
		/// <param name="gravity">Gravity in units of Earth gravities</param>
		/// <returns>Pressure in millibars (mb)</returns>
		public virtual Pressure GetSurfacePressure(Ratio volatileGasInventory,
										   Length radius,
										   Acceleration surfaceGravity)
		{
			//  JLB: Aparently this assumed that earth pressure = 1000mb. I've added a
			//	fudge factor (EARTH_SURF_PRES_IN_MILLIBARS / 1000.) to correct for that

			var equatorialRadius = GlobalConstants.KM_EARTH_RADIUS / radius.Kilometers;
			return Pressure.FromMillibars(volatileGasInventory.Value * surfaceGravity.StandardGravity *
					(GlobalConstants.EARTH_SURF_PRES_IN_MILLIBARS / 1000.0) /
					Utilities.Pow2(equatorialRadius));     
		}

		/// <remakrs>
		/// Fogg's information for this routine came from Dole "Habitable Planets
		/// for Man", Blaisdell Publishing Company, NY, 1964.  From this, he came
		/// up with his eq.12, which is the equation for the 'base_angular_velocity'
		/// below.  He then used an equation for the change in angular velocity per
		/// time (dw/dt) from P. Goldreich and S. Soter's paper "Q in the Solar
		/// System" in Icarus, vol 5, pp.375-389 (1966).	 Using as a comparison the
		/// change in angular velocity for the Earth, Fogg has come up with an
		/// approximation for our new planet (his eq.13) and take that into account.
		/// This is used to find 'change_in_angular_velocity' below.
		/// </remarks>
		public virtual Duration GetDayLength(RotationalSpeed angularVelocityRadSec,
									 Duration orbitalPeriod,
									 Ratio eccentricity)
		{
			var stopped = false;
			var dayInHours = GlobalConstants.RADIANS_PER_ROTATION / (GlobalConstants.SECONDS_PER_HOUR * angularVelocityRadSec.RadiansPerSecond);
			if (angularVelocityRadSec.RadiansPerSecond <= 0.0)
			{
				stopped = true;
				dayInHours = double.MaxValue;
			}

			var yearInHours = orbitalPeriod.Days * 24.0;
			if (dayInHours >= yearInHours || stopped)
			{
				if (eccentricity.Value > 0.1)
				{
					var spinResonanceFactor = (1.0 - eccentricity.Value) / (1.0 + eccentricity.Value);
					return Duration.FromHours(spinResonanceFactor * yearInHours);
				}

				return Duration.FromHours(yearInHours);
			}

			return Duration.FromHours(dayInHours);
		}

		public virtual bool TestHasResonantPeriod(RotationalSpeed angularVelocityRadSec,
											Duration dayLength,
											Duration orbitalPeriod,
											Ratio eccentricity)
		{
			var yearInHours = orbitalPeriod.Years365 * 24.0;
			return (angularVelocityRadSec.RadiansPerSecond <= 0.0 || dayLength.Hours >= yearInHours)
				   && eccentricity.Value > 0.1;
		}

		/// <remarks>
		/// This function implements the escape velocity calculation. Note that
		/// it appears that Fogg's eq.15 is incorrect.	
		/// </remarks>
		public virtual Speed GetEscapeVelocity(Mass massSM,
										  Length radius)
		{
			//var massInGrams = massSM * GlobalConstants.SOLAR_MASS_IN_GRAMS;
			//var radiusinCM = radius * GlobalConstants.CM_PER_KM;
			return Speed.FromCentimetersPerSecond(Math.Sqrt(2.0 * GlobalConstants.GRAV_CONSTANT * massSM.Grams / radius.Centimeters));
		}

		/// <summary>
		/// Returns the boiling point of water using Fogg's eq.21.
		/// </summary>
		/// <param name="surfacePressure">Atmospheric pressure in millibars (mb)</param>
		/// <returns>Boiling point of water in Kelvin.</returns>
		public virtual Temperature GetBoilingPointWater(Pressure surfpres)
		{
			var surfacePressureInBars = surfpres.Bars; // / GlobalConstants.MILLIBARS_PER_BAR;

			return Temperature.FromKelvins(Math.Abs(surfpres.Millibars) < 0.001
					? 0.0
					: (1.0 / ((Math.Log(surfacePressureInBars) / -5050.5) + (1.0 / 373.0))));
		}

		public virtual BodyType GetBodyType(Mass massSM,
						   Mass gasMassSM,
						   Mass molecularWeightRetained,
						   Pressure surfacePressure,
						   Ratio waterCoverFraction,
						   Ratio iceCoverFraction,
						   Temperature maxTemperature,
						   Temperature boilingPointWater,
						   Temperature surfaceTemperature)
		{
			if (TestIsGasGiant(massSM, gasMassSM, molecularWeightRetained))
			{
				if (gasMassSM / massSM < 0.20)
				{
					return BodyType.SubSubGasGiant;
				}
				else if (massSM.SolarMasses * GlobalConstants.SUN_MASS_IN_EARTH_MASSES < 20.0)
				{
					return BodyType.SubGasGiant;
				}
				else
				{
					return BodyType.GasGiant;
				}
			}

			// Assign planet type
			if (surfacePressure.Millibars < 1.0)
			{
				if (massSM.SolarMasses * GlobalConstants.SUN_MASS_IN_EARTH_MASSES < GlobalConstants.ASTEROID_MASS_LIMIT)
				{
					return BodyType.Asteroids;
				}
				else
				{
					return BodyType.Barren;
				}
			}
			else if (surfacePressure.Millibars > 6000.0 && molecularWeightRetained.Grams <= 2.0) // Retains Hydrogen
			{
				return BodyType.SubSubGasGiant;
			}
			else
			{
				// Atmospheres:
				// TODO remove PlanetType enum entirely and replace it with a more flexible classification systme
				if (waterCoverFraction.Value >= 0.95) // >95% water
				{
					return BodyType.Water;
				}
				else if (iceCoverFraction.Value >= 0.95) // >95% ice
				{
					return BodyType.Ice;
				}
				else if (waterCoverFraction.Value > 0.05) // Terrestrial
				{
					return BodyType.Terrestrial;
				}
				else if (maxTemperature > boilingPointWater) // Hot = Venusian
				{
					return BodyType.Venusian;
				}
				else if (gasMassSM / massSM > 0.0001) // Accreted gas, but no greenhouse or liquid water make it an ice world
				{
					return BodyType.Ice;
					//planet.IceCoverFraction = 1.0;
				}
				else if (surfacePressure.Millibars <= 250.0) // Thin air = Martian
				{
					return BodyType.Martian;
				}
				else if (surfaceTemperature.Kelvins < GlobalConstants.FREEZING_POINT_OF_WATER)
				{
					return BodyType.Ice;
				}
				else
				{
					return BodyType.Undefined;
				}
			}

		}

		public virtual bool TestIsTidallyLocked(Duration dayLength,
								  Duration orbitalPeriod)
		{
			return (int)dayLength.Hours == (int)(orbitalPeriod.Hours);
		}

		public virtual Ratio GetMinimumIllumination(Length a,
									   Luminosity l)
		{
			return Ratio.FromDecimalFractions(Utilities.Pow2(1.0 / a.AstronomicalUnits) * l.SolarLuminosities);
		}

		public virtual Length GetRocheLimit(Length bodyRadius,
							  Density bodyDensity,
							  Density satelliteDensity)
		{
			return 1.26 * bodyRadius * Math.Pow(bodyDensity / satelliteDensity, 1.0 / 3.0);
		}

		public virtual bool TestIsHabitable(Duration dayLength,
							  Duration orbitalPeriod,
							  Breathability breathability,
							  bool hasResonantPeriod,
							  bool isTidallyLocked)
		{
			return breathability == Breathability.Breathable &&
				   !hasResonantPeriod &&
				   !isTidallyLocked;
		}

		public virtual bool TestIsEarthLike(Temperature surfaceTemperature,
							  Ratio waterCoverFraction,
							  Ratio cloudCoverFraction,
							  Ratio iceCoverFraction,
							  Pressure surfacePressure,
							  Acceleration surfaceGravityG,
							  Breathability breathability,
							  BodyType planetType)
		{
			double relTemp = surfaceTemperature.Kelvins - GlobalConstants.FREEZING_POINT_OF_WATER -
							 GlobalConstants.EARTH_AVERAGE_CELSIUS;
			double seas = waterCoverFraction.Value * 100.0;
			double clouds = cloudCoverFraction.Value * 100.0;
			double pressure = surfacePressure.Millibars / GlobalConstants.EARTH_SURF_PRES_IN_MILLIBARS;
			double ice = iceCoverFraction.Value * 100.0;

			return
				surfaceGravityG.StandardGravity >= .8 &&
				surfaceGravityG.StandardGravity <= 1.2 &&
				relTemp >= -2.0 &&
				relTemp <= 3.0 &&
				ice <= 10.0 &&
				pressure >= 0.5 &&
				pressure <= 2.0 &&
				clouds >= 40.0 &&
				clouds <= 80.0 &&
				seas >= 50.0 &&
				seas <= 80.0 &&
				planetType != BodyType.Water &&
				breathability == Breathability.Breathable;
		}

		public virtual int GetOrbitalZone(Luminosity luminosity, Length orbRadius)
		{
			if (orbRadius.AstronomicalUnits < 4.0 * Math.Sqrt(luminosity.SolarLuminosities))
			{
				return 1;
			}
			else if (orbRadius.AstronomicalUnits < 15.0 * Math.Sqrt(luminosity.SolarLuminosities))
			{
				return 2;
			}
			else
			{
				return 3;
			}
		}

		public virtual Duration GetPeriod(Length separation, Mass smallMass, Mass largeMass)
		{
			double period_in_years;

			period_in_years = Math.Sqrt(Utilities.Pow3(separation.AstronomicalUnits) / (smallMass.SolarMasses + largeMass.SolarMasses));
			return Duration.FromDays(period_in_years * GlobalConstants.DAYS_IN_A_YEAR);
		}

		/// <summary>
		/// This is Fogg's eq.19.
		/// </summary>
		/// <param name="ecosphereRadius"></param>
		/// <param name="orbitalRadius"></param>
		/// <param name="albedo"></param>
		/// <returns></returns>
		public virtual Temperature GetEffectiveTemperature(Length ecosphereRadius, Length orbitalRadius, Ratio albedo)
		{
			// This is Fogg's eq.19.
			return Temperature.FromKelvins(Math.Sqrt(ecosphereRadius / orbitalRadius)
				  * Utilities.Pow1_4((1.0 - albedo.Value) / (1.0 - GlobalConstants.EARTH_ALBEDO))
				  * GlobalConstants.EARTH_EFFECTIVE_TEMP);
		}

		/// <summary>
		/// This is Fogg's eq.20, and is also Hart's eq.20 in his "Evolution of
		///	Earth's Atmosphere" article.  The effective temperature given is in
		///	units of Kelvin, as is the rise in temperature produced by the
		///	greenhouse effect, which is returned.
		///	TODO: Validate opticalDepth units.
		/// </summary>
		/// <param name="opticalDepth"></param>
		/// <param name="effectiveTemp"></param>
		/// <param name="surfPressure"></param>
		/// <returns></returns>
		public virtual TemperatureDelta GetGreenhouseTemperatureRise(Ratio opticalDepth, Temperature effectiveTemp, Pressure surfPressure)
		{
			var convectionFactor = GlobalConstants.EARTH_CONVECTION_FACTOR * Math.Pow(surfPressure.Millibars / GlobalConstants.EARTH_SURF_PRES_IN_MILLIBARS, 0.25);
			var rise = (Utilities.Pow1_4(1.0 + 0.75 * opticalDepth.Value) - 1.0) *
							   effectiveTemp.Kelvins * convectionFactor;

			if (rise < 0.0) rise = 0.0;

			return TemperatureDelta.FromKelvins(rise);
		}

		public virtual Ratio GetOpacity(Mass molecularWeightParam, Pressure surfPressureParam)
		{
			var molecularWeight = molecularWeightParam.Grams;
			var surfPressure = surfPressureParam.Millibars;

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

			return Ratio.FromDecimalFractions(opticalDepth);
		}

		/// <summary>
		/// This function is Fogg's eq.22.
		/// </summary>
		/// <param name="volatileGasInventory"></param>
		/// <param name="planetRadius"></param>
		/// <returns></returns>
		public virtual Ratio GetWaterFraction(Ratio volatileGasInventory, Length planetRadius)
		{
			var temp = (0.71 * volatileGasInventory.Value / 1000.0)
					 * Utilities.Pow2(GlobalConstants.KM_EARTH_RADIUS / planetRadius.Kilometers);
			return Ratio.FromDecimalFractions(temp >= 1.0 ? 1.0 : temp);
		}

		/// <summary>
		///  Given the surface temperature of a planet (in Kelvin), this function
		///	 returns the fraction of cloud cover available.	 This is Fogg's eq.23.
		///	 See Hart in "Icarus" (vol 33, pp23 - 39, 1978) for an explanation.
		///	 This equation is Hart's eq.3.	
		///	 I have modified it slightly using constants and relationships from
		///	 Glass's book "Introduction to Planetary Geology", p.46.
		///	 The 'CLOUD_COVERAGE_FACTOR' is the amount of surface area on Earth	
		///	 covered by one Kg. of cloud.
		/// </summary>
		/// <param name="surfaceTemp"></param>
		/// <param name="smallestMWRetained"></param>
		/// <param name="equatorialRadius"></param>
		/// <param name="hydroFraction"></param>
		/// <returns></returns>
		public virtual Ratio GetCloudFraction(Temperature surfaceTemp, Mass smallestMWRetained, Length equatorialRadius, Ratio hydroFraction)
		{
			if (smallestMWRetained.Grams > GlobalConstants.WATER_VAPOR)
			{
				return Ratio.FromDecimalFractions(0.0);
			}

			var surfArea = 4.0 * Math.PI * Utilities.Pow2(equatorialRadius.Kilometers);
			var hydroMass = hydroFraction.Value * surfArea * GlobalConstants.EARTH_WATER_MASS_PER_AREA;
			var waterVaporKg = (0.00000001 * hydroMass) *
								Math.Exp(GlobalConstants.Q2_36 * (surfaceTemp.Kelvins - GlobalConstants.EARTH_AVERAGE_KELVIN));
			var fraction = GlobalConstants.CLOUD_COVERAGE_FACTOR * waterVaporKg / surfArea;
			return Ratio.FromDecimalFractions(fraction >= 1.0 ? 1.0 : fraction);
		}

		/// <summary>
		/// This is Fogg's eq.24. See Hart[24] in Icarus vol.33, p.28 for an explanation.
		/// I have changed a constant from 70 to 90 in order to bring it more in	
		/// line with the fraction of the Earth's surface covered with ice, which	
		/// is approximatly .016 (=1.6%).
		/// </summary>
		/// <param name="hydroFraction"></param>
		/// <param name="surfTemp"></param>
		/// <returns></returns>
		public virtual Ratio GetIceFraction(Ratio hydroFraction, Temperature surfTempParam)
		{
			var surfTemp = surfTempParam.Kelvins;

			if (surfTemp > 328.0)
			{
				surfTemp = 328.0;
			}
			var temp = Math.Pow(((328.0 - surfTemp) / 90.0), 5.0);
			if (temp > (1.5 * hydroFraction.Value))
			{
				temp = (1.5 * hydroFraction.Value);
			}

			return Ratio.FromDecimalFractions(temp >= 1.0 ? 1.0 : temp);
		}

		/// <summary>
		/// The surface temperature passed in is in units of Kelvin.
		/// The cloud adjustment is the fraction of cloud cover obscuring each
		/// of the three major components of albedo that lie below the clouds.
		/// </summary>
		/// <param name="waterFraction"></param>
		/// <param name="cloudFraction"></param>
		/// <param name="iceFraction"></param>
		/// <param name="surfPressure"></param>
		/// <returns></returns>
		public virtual Ratio GetAlbedo(Ratio waterFractionParam, Ratio cloudFractionParam, Ratio iceFractionParam, Pressure surfPressure)
		{
			var waterFraction = waterFractionParam.Value;
			var cloudFraction = cloudFractionParam.Value;
			var iceFraction = iceFractionParam.Value;

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

			if (surfPressure.Millibars == 0.0)
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

			return Ratio.FromDecimalFractions(cloud_part + rock_part + water_part + ice_part);
		}

		public virtual Mass GetInitialMolecularWeightRetained(Mass mass, Length radius, Temperature exoTemp)
		{
			var escapeVelocity = GetEscapeVelocity(mass, radius);

			return Mass.FromGrams((3.0 * GlobalConstants.MOLAR_GAS_CONST * exoTemp.Kelvins) /
					(Utilities.Pow2((escapeVelocity.CentimetersPerSecond / GlobalConstants.GAS_RETENTION_THRESHOLD) / GlobalConstants.CM_PER_METER)));
		}

		/// <summary>
		/// Taken from Dole p. 34. He cites Jeans (1916) & Jones (1923). 
		/// </summary>
		/// <param name="molecularWeight"></param>
		/// <param name="exoTempKelvin"></param>
		/// <param name="surfGravG"></param>
		/// <param name="radiusKM"></param>
		/// <returns></returns>
		public virtual Duration GetGasLife(Mass molecularWeight, Temperature exoTempKelvin, Acceleration surfGravG, Length radiusKM)
		{
			var v = GetRMSVelocity(molecularWeight, exoTempKelvin).CentimetersPerSecond;
			var g = surfGravG.StandardGravity * GlobalConstants.EARTH_ACCELERATION;
			var r = radiusKM.Centimeters;
			var t = (Utilities.Pow3(v) / (2.0 * Utilities.Pow2(g) * r)) * Math.Exp((3.0 * g * r) / Utilities.Pow2(v));
			var years = t / (GlobalConstants.SECONDS_PER_HOUR * 24.0 * GlobalConstants.DAYS_IN_A_YEAR);

			if (years > 2.0E10)
			{
				years = double.MaxValue;
			}

			return Duration.FromYears365(years);
		}

		/// <summary>
		///  This formula is on Dole's p. 14 
		/// </summary>
		/// <param name="surf_pressure"></param>
		/// <param name="gas_pressure"></param>
		/// <returns></returns>
		public virtual Pressure GetInspiredPartialPressure(Pressure surf_pressure, Pressure gas_pressure)
		{
			var pH2O = (GlobalConstants.H20_ASSUMED_PRESSURE);
			var fraction = gas_pressure / surf_pressure;

			return Pressure.FromMillibars((surf_pressure.Millibars - pH2O) * fraction);
		}

		// TODO figure out how this function differs from EffTemp, write summary
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ecosphereRadius"></param>
		/// <param name="orbitalRadius"></param>
		/// <param name="albedo"></param>
		/// <returns></returns>
		public virtual Temperature GetEstimatedTemperature(Length ecosphereRadius, Length orbitalRadius, Ratio albedo)
		{
			return Temperature.FromKelvins(Math.Sqrt(ecosphereRadius / orbitalRadius)
				  * Utilities.Pow1_4((1.0 - albedo.DecimalFractions) / (1.0 - GlobalConstants.EARTH_ALBEDO))
				  * GlobalConstants.EARTH_AVERAGE_KELVIN);
		}

        /// <summary>
        /// The following is Holman & Wiegert's equation 1 from
        /// Long-Term Stability of Planets in Binary Systems
        /// The Astronomical Journal, 117:621-628, Jan 1999
        /// </summary>
        /// <param name="mass"></param>
        /// <param name="otherMass"></param>
        /// <param name="otherSemiMajorAxis"></param>
        /// <param name="ecc"></param>
        /// <returns></returns>
        public virtual Length GetOuterLimit(Mass mass, Mass otherMass, Length otherSemiMajorAxis, Ratio ecc)
        {
            if (otherMass.SolarMasses < .001)
            {
                return Length.Zero;
            }

            // The following is Holman & Wiegert's equation 1 from
            // Long-Term Stability of Planets in Binary Systems
            // The Astronomical Journal, 117:621-628, Jan 1999
            double m1 = mass.SolarMasses;
            double m2 = otherMass.SolarMasses;
            double mu = m2 / (m1 + m2);
            double e = otherSemiMajorAxis.AstronomicalUnits;
            double e2 = Utilities.Pow2(e);
            double a = ecc.DecimalFractions;

            return Length.FromAstronomicalUnits((0.464 + -0.380 * mu + -0.631 * e + 0.586 * mu * e + 0.150 * e2 + -0.198 * mu * e2) * a);
        }

        /// <summary>
        /// Give SatelliteBody radius given a mass and a density.
        /// </summary>
        /// <param name="mass"></param>
        /// <param name="density"></param>
        /// <returns></returns>
        public virtual Length GetRadius(Mass mass, Density density)
        {
            double volume;

            double m = mass.Grams;
            volume = m / density.GramsPerCubicCentimeter;
            return Length.FromKilometers(Math.Pow((3.0 * volume) / (4.0 * Math.PI), (1.0 / 3.0)) / GlobalConstants.CM_PER_KM);
        }

        public virtual Length GetStellarDustLimit(Mass mass)
        {
            return Length.FromAstronomicalUnits(200.0 * Math.Pow(mass.SolarMasses, 1.0 / 3.0));
        }

        public virtual Length GetEcosphereRadius(Mass mass, Luminosity lum)
        {
            return Length.FromKilometers(Math.Sqrt(lum.SolarLuminosities) * GlobalConstants.ASTRONOMICAL_UNIT_KM);
        }
    }

}
