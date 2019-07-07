using System;
using System.Collections.Generic;
using System.Text;
using Primoris.Universe.Stargen.Bodies;

namespace Primoris.Universe.Stargen.Physics
{
	public class BurrowsBodyPhysics : IBodyPhysics
	{
		public double GetExosphereTemperature(double semiMajorAxisAu, double ecosphereRadiusAU, double sunTemperature)
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

		public double GetRadius(double massSM, int orbitZone)
		{
			return Environment.KothariRadius(massSM, false, orbitZone);
		}

		public double GetDensityFromStar(double massSM, double semiMajorAxisAU, double ecosphereRadiusAU, bool isGasGiant)
		{
			return Environment.EmpiricalDensityGCC(massSM, semiMajorAxisAU, ecosphereRadiusAU, isGasGiant);
		}

		public double GetDensityFromBody(double massSM, double radius)
		{
			return Environment.VolumeDensity(massSM, radius);
		}

		public bool TestIsGasGiant(double massSM, double gasMassSM, double molecularWeightRetained)
		{
			return (((massSM * GlobalConstants.SUN_MASS_IN_EARTH_MASSES) > 1.0) && ((gasMassSM / massSM) > 0.05) && (molecularWeightRetained <= 4.0));
		}

		public bool TestHasGreenhouseEffect(double ecosphereRadius, double semiAxisMajorAU)
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

		public virtual double GetMolecularWeightRetained(double surfGrav, double mass, double radius, double exosphereTemp, double sunAge)
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

		public virtual double GetHillSphere(double sunMass, double massSM, double semiMajorAxisSM)
		{
			return Environment.SimplifiedHillSphere(sunMass, massSM, semiMajorAxisSM);
		}

		public virtual double GetSurfacePressure(double volatileGasInventory, double radius, double surfaceGravity)
		{
			return Environment.Pressure(volatileGasInventory, radius, surfaceGravity);
		}

		public virtual double GetAngularVelocity(double massSM,
										   double radiusKM,
										   double densityGCC,
										   double semiMajorAxisAU,
										   bool isGasGiant,
										   double starMassSM,
										   double starAgeYears)
		{
			return Environment.AngularVelocity(massSM,
									  radiusKM,
									  densityGCC,
									  semiMajorAxisAU,
									  isGasGiant,
									  starMassSM,
									  starAgeYears);
		}

		public virtual double GetDayLength(double angularVelocityRadSec, double orbitalPeriod, double eccentricity)
		{
			return Environment.DayLength(angularVelocityRadSec, orbitalPeriod, eccentricity);
		}

		public virtual bool TestHasResonantPeriod(double angularVelocityRadSec, double dayLength, double orbitalPeriod, double eccentricity)
		{
			return Environment.HasResonantPeriod(angularVelocityRadSec, dayLength, orbitalPeriod, eccentricity);
		}

		public virtual double GetEscapeVelocity(double massSM, double radius)
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

		public bool TestIsTidallyLocked(double dayLength, double orbitalPeriod)
		{
			return (int)dayLength == (int)(orbitalPeriod * 24);
		}

	}

}
