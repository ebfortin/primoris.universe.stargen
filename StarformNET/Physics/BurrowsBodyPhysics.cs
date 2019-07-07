using System;
using System.Collections.Generic;
using System.Text;
using Primoris.Universe.Stargen.Bodies;

namespace Primoris.Universe.Stargen.Physics
{
	public class BurrowsBodyPhysics : IBodyPhysics
	{
		public double GetBaseAngularVelocity(double massSM,
									   double radiusKM,
									   bool isGasGiant)
		{
			// Fogg eq. 12

			var planetaryMassInGrams = massSM * GlobalConstants.SOLAR_MASS_IN_GRAMS;
			var equatorialRadiusInCM = radiusKM * GlobalConstants.CM_PER_KM;
			var k2 = isGasGiant ? 0.24 : 0.33;

			return Math.Sqrt(GlobalConstants.J * (planetaryMassInGrams) /
							 ((k2 / 2.0) * Utilities.Pow2(equatorialRadiusInCM)));
		}

		public double GetChangeInAngularVelocity(double densityGCC,
										   double massSM,
										   double radiusKM,
										   double semiMajorAxisAU,
										   double starMassSM)
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

		public virtual double GetAngularVelocity(double massSM,
								   double radiusKM,
								   double densityGCC,
								   double semiMajorAxisAU,
								   bool isGasGiant,
								   double starMassSM,
								   double starAgeYears)
		{
			var baseAngularVelocity = GetBaseAngularVelocity(massSM, radiusKM, isGasGiant);
			var changeInAngularVelocity = GetChangeInAngularVelocity(
				densityGCC, massSM, radiusKM, semiMajorAxisAU, starMassSM);
			return baseAngularVelocity + (changeInAngularVelocity *
														 starAgeYears);
		}

		public double GetExosphereTemperature(double semiMajorAxisAu,
										double ecosphereRadiusAU,
										double sunTemperature)
		{
			double exoTemp = 0.0;

			exoTemp = GlobalConstants.EARTH_EXOSPHERE_TEMP / Utilities.Pow2(semiMajorAxisAu / ecosphereRadiusAU);
			// Exosphere temperature can't be realisticly higher than the surface temperature of the sun. We therefore clip at sun.Temperature.
			// TODO: Make transition instead of brute clipping.
			exoTemp = exoTemp > sunTemperature ? sunTemperature : exoTemp;
			// Exosphere can't be realisticly lower than the CMB radiation temperature, which is 2.7K. 
			// TODO: Make transition instead of brute clipping.
			exoTemp = exoTemp < GlobalConstants.VACCUM_TEMPERATURE ? GlobalConstants.VACCUM_TEMPERATURE : exoTemp;

			return exoTemp;
		}

		public double GetRMSVelocityCMSec(double exoTemp)
		{
			return Environment.RMSVelocity(GlobalConstants.MOL_NITROGEN, exoTemp);
		}

		public double GetRadius(double massSM,
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

			temp = (2.0 * GlobalConstants.BETA_20 * Math.Pow(GlobalConstants.SOLAR_MASS_IN_GRAMS, (1.0 / 3.0)))
				   / (GlobalConstants.A1_20 * Math.Pow(temp1, (1.0 / 3.0)));

			temp2 = GlobalConstants.A2_20 * Math.Pow(atomic_weight, (4.0 / 3.0)) *
					Math.Pow(GlobalConstants.SOLAR_MASS_IN_GRAMS, (2.0 / 3.0));
			temp2 = temp2 * Math.Pow(massSM, (2.0 / 3.0));
			temp2 = temp2 / (GlobalConstants.A1_20 * Utilities.Pow2(atomic_num));
			temp2 = 1.0 + temp2;
			temp = temp / temp2;
			temp = (temp * Math.Pow(massSM, (1.0 / 3.0))) / GlobalConstants.CM_PER_KM;

			temp /= GlobalConstants.JIMS_FUDGE; /* Make Earth = actual earth */

			return (temp);
		}

		public double GetDensityFromStar(double massSM,
								   double semiMajorAxisAU,
								   double ecosphereRadiusAU,
								   bool isGasGiant)
		{
			double density = Math.Pow(massSM * GlobalConstants.SUN_MASS_IN_EARTH_MASSES, (1.0 / 8.0));
			density *= Utilities.Pow1_4(ecosphereRadiusAU / semiMajorAxisAU);

			if (isGasGiant)
			{
				return (density * 1.2);
			}
			else
			{
				return (density * 5.5);
			}
		}

		public double GetDensityFromBody(double massSM,
								   double radius)
		{
			return Environment.VolumeDensity(massSM, radius);
		}

		public bool TestIsGasGiant(double massSM,
							 double gasMassSM,
							 double molecularWeightRetained)
		{
			return (((massSM * GlobalConstants.SUN_MASS_IN_EARTH_MASSES) > 1.0) && ((gasMassSM / massSM) > 0.05) && (molecularWeightRetained <= 4.0));
		}

		public bool TestHasGreenhouseEffect(double ecosphereRadius,
									  double semiAxisMajorAU)
		{
			return Environment.Greenhouse(ecosphereRadius, semiAxisMajorAU);
		}

		public double GetVolatileGasInventory(double massSM,
												   double escapeVelocity,
												   double rmsVelocity,
												   double sunMass,
												   double gasMassSM,
												   int orbitZone,
												   bool hasGreenhouse,
												   bool hasAccretedGas)
		{
			return Environment.VolatileInventory(
				massSM, escapeVelocity, rmsVelocity, sunMass,
				orbitZone, hasGreenhouse, (gasMassSM / massSM) > 0.000001);
		}

		public virtual double GetMolecularWeightRetained(double surfGrav,
												   double mass,
												   double radius,
												   double exosphereTemp,
												   double sunAge)
		{
			var temp = exosphereTemp;

			var guess1 = Environment.MoleculeLimit(mass, radius, temp);
			var guess2 = guess1;

			var life = Environment.GasLife(guess1, exosphereTemp, surfGrav, radius);

			var loops = 0;

			var target = sunAge;

			if (life > target)
			{
				while ((life > target) && (loops++ < 25))
				{
					guess1 = guess1 / 2.0;
					life = Environment.GasLife(guess1, exosphereTemp, surfGrav, radius);
				}
			}
			else
			{
				while ((life < target) && (loops++ < 25))
				{
					guess2 = guess2 * 2.0;
					life = Environment.GasLife(guess2, exosphereTemp, surfGrav, radius);
				}
			}

			loops = 0;

			while (guess2 - guess1 > 0.1 && loops++ < 25)
			{
				var guess3 = (guess1 + guess2) / 2.0;
				life = Environment.GasLife(guess3, exosphereTemp, surfGrav, radius);

				if (life < target)
				{
					guess1 = guess3;
				}
				else
				{
					guess2 = guess3;
				}
			}

			life = Environment.GasLife(guess2, exosphereTemp, surfGrav, radius);

			return guess2;
		}

		public virtual double GetHillSphere(double bigMass,
									  double smallMassSM,
									  double semiMajorAxisSM)
		{
			return semiMajorAxisSM * Math.Pow(smallMassSM / (3 * bigMass), (1.0 / 3.0)) * GlobalConstants.KM_PER_AU;
		}

		public virtual double GetSurfacePressure(double volatileGasInventory,
										   double radius,
										   double surfaceGravity)
		{
			return Environment.Pressure(volatileGasInventory, radius, surfaceGravity);
		}

		public virtual double GetDayLength(double angularVelocityRadSec,
									 double orbitalPeriod,
									 double eccentricity)
		{
			return Environment.DayLength(angularVelocityRadSec, orbitalPeriod, eccentricity);
		}

		public virtual bool TestHasResonantPeriod(double angularVelocityRadSec,
											double dayLength,
											double orbitalPeriod,
											double eccentricity)
		{
			return Environment.HasResonantPeriod(angularVelocityRadSec, dayLength, orbitalPeriod, eccentricity);
		}

		public virtual double GetEscapeVelocity(double massSM,
										  double radius)
		{
			return Environment.EscapeVelocity(massSM, radius);
		}

		public virtual double GetBoilingPointWater(double surfpres)
		{
			return Math.Abs(surfpres) < 0.001
					? 0.0
					: Environment.BoilingPoint(surfpres);
		}

		public BodyType GetBodyType(double massSM,
						   double gasMassSM,
						   double molecularWeightRetained,
						   double surfacePressure,
						   double waterCoverFraction,
						   double iceCoverFraction,
						   double maxTemperature,
						   double boilingPointWater,
						   double surfaceTemperature)
		{
			if (TestIsGasGiant(massSM, gasMassSM, molecularWeightRetained))
			{
				if ((gasMassSM / massSM) < 0.20)
				{
					return BodyType.SubSubGasGiant;
				}
				else if ((massSM * GlobalConstants.SUN_MASS_IN_EARTH_MASSES) < 20.0)
				{
					return BodyType.SubGasGiant;
				}
				else
				{
					return BodyType.GasGiant;
				}
			}

			// Assign planet type
			if (surfacePressure < 1.0)
			{
				if (((massSM * GlobalConstants.SUN_MASS_IN_EARTH_MASSES) < GlobalConstants.ASTEROID_MASS_LIMIT))
				{
					return BodyType.Asteroids;
				}
				else
				{
					return BodyType.Barren;
				}
			}
			else if ((surfacePressure > 6000.0) && (molecularWeightRetained <= 2.0)) // Retains Hydrogen
			{
				return BodyType.SubSubGasGiant;
			}
			else
			{
				// Atmospheres:
				// TODO remove PlanetType enum entirely and replace it with a more flexible classification systme
				if (waterCoverFraction >= 0.95) // >95% water
				{
					return BodyType.Water;
				}
				else if (iceCoverFraction >= 0.95) // >95% ice
				{
					return BodyType.Ice;
				}
				else if (waterCoverFraction > 0.05) // Terrestrial
				{
					return BodyType.Terrestrial;
				}
				else if (maxTemperature > boilingPointWater) // Hot = Venusian
				{
					return BodyType.Venusian;
				}
				else if ((gasMassSM / massSM) > 0.0001) // Accreted gas, but no greenhouse or liquid water make it an ice world
				{
					return BodyType.Ice;
					//planet.IceCoverFraction = 1.0;
				}
				else if (surfacePressure <= 250.0) // Thin air = Martian
				{
					return BodyType.Martian;
				}
				else if (surfaceTemperature < GlobalConstants.FREEZING_POINT_OF_WATER)
				{
					return BodyType.Ice;
				}
				else
				{
					return BodyType.Undefined;
				}
			}

		}

		public bool TestIsTidallyLocked(double dayLength,
								  double orbitalPeriod)
		{
			return (int)dayLength == (int)(orbitalPeriod * 24);
		}

		public double GetMinimumIllumination(double a,
									   double l)
		{
			return Utilities.Pow2(1.0 / a) * l;
		}

		public double GetRocheLimit(double bodyRadius,
							  double bodyDensity,
							  double satelliteDensity)
		{
			return (1.26 * bodyRadius * Math.Pow(bodyDensity / satelliteDensity, 1.0 / 3.0)) / 1000.0;
		}

		public bool TestIsHabitable(double dayLength,
							  double orbitalPeriod,
							  Breathability breathability,
							  bool hasResonantPeriod,
							  bool isTidallyLocked)
		{
			return breathability == Breathability.Breathable &&
				   !hasResonantPeriod &&
				   !isTidallyLocked;
		}

		public bool TestIsEarthLike(double surfaceTemperature,
							  double waterCoverFraction,
							  double cloudCoverFraction,
							  double iceCoverFraction,
							  double surfacePressure,
							  double surfaceGravityG,
							  Breathability breathability,
							  BodyType planetType)
		{
			double relTemp = (surfaceTemperature - GlobalConstants.FREEZING_POINT_OF_WATER) -
							 GlobalConstants.EARTH_AVERAGE_CELSIUS;
			double seas = waterCoverFraction * 100.0;
			double clouds = cloudCoverFraction * 100.0;
			double pressure = surfacePressure / GlobalConstants.EARTH_SURF_PRES_IN_MILLIBARS;
			double ice = iceCoverFraction * 100.0;

			return
				surfaceGravityG >= .8 &&
				surfaceGravityG <= 1.2 &&
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
				breathability == Bodies.Breathability.Breathable;
		}

		public int GetOrbitalZone(double luminosity,
							double orbRadius)
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


	}

}
