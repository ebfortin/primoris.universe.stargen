using System;
using System.Linq;
using System.Collections.Generic;
using Primoris.Universe.Stargen.Astrophysics;
using Primoris.Universe.Stargen.Systems;
using Environment = Primoris.Universe.Stargen.Astrophysics.Environment;
using Primoris.Universe.Stargen.Astrophysics.Burrows;
using UnitsNet;

namespace Primoris.Universe.Stargen.Bodies.Burrows
{

	// TODO break this class up
	// TODO orbit zone is supposedly no longer used anywhere. Check references and possibly remove.

	[Serializable]
	public class Planet : SatelliteBody
	{
		private static readonly IScienceAstrophysics BurrowsPhysics = new BodyPhysics();


		public Planet(Star sun, Atmosphere atmos) : base(BurrowsPhysics, sun, atmos) { }

		public Planet(Star sun,
					  Length semiMajorAxisAU,
					  Ratio eccentricity,
					  Angle axialTilt,
					  Duration dayLengthHours,
					  Duration orbitalPeriodDays,
					  Mass massSM,
					  Mass gasMassSM,
					  Length radius,
					  Pressure surfPressure,
					  Temperature dayTimeTempK,
					  Temperature nightTimeTempK,
					  Temperature surfTempK,
					  Acceleration surfGrav) : base(BurrowsPhysics,
							   sun,
							   semiMajorAxisAU,
							   eccentricity,
							   axialTilt,
							   dayLengthHours,
							   orbitalPeriodDays,
							   massSM,
							   gasMassSM,
							   radius,
							   surfPressure,
							   dayTimeTempK,
							   nightTimeTempK,
							   surfTempK,
							   surfGrav)
		{ }


		public Planet(Seed seed,
					   Star star,
					   int num,
					   bool useRandomTilt,
					   string planetID,
					   SystemGenerationOptions genOptions) : base(BurrowsPhysics,
												   seed,
												   star,
												   num,
												   useRandomTilt,
												   planetID,
												   genOptions)
		{ }

		public Planet(Star star, Gas[] atmosComp) : base(BurrowsPhysics, star, atmosComp) { }

		public Planet(Star star) : base(BurrowsPhysics, star) { }

		protected override void AdjustPropertiesForRockyBody()
		{
			double age = Star.Age.Years365;
			double massSM = Mass.SolarMasses;
			double gasMassSM = GasMass.SolarMasses;
			double exosphereTemperature = ExosphereTemperature.Kelvins;
			double surfaceGravityG = SurfaceAcceleration.StandardGravity;
			double radius = Radius.Kilometers;
			double surfaceAccelerationCMSec2 = SurfaceAcceleration.CentimetersPerSecondSquared; //SurfaceAccelerationCMSec2;

			if (gasMassSM / massSM > 0.000001)
			{
				var h2Mass = gasMassSM * 0.85;
				var heMass = (gasMassSM - h2Mass) * 0.999;

				var h2Life = Environment.GasLife(GlobalConstants.MOL_HYDROGEN, exosphereTemperature,
					surfaceGravityG, radius);
				var heLife = Environment.GasLife(GlobalConstants.HELIUM, exosphereTemperature,
					surfaceGravityG, radius);

				if (h2Life < age)
				{
					var h2Loss = (1.0 - 1.0 / Math.Exp(age / h2Life)) * h2Mass;

					GasMass -= Mass.FromSolarMasses(h2Loss);
					Mass -= Mass.FromSolarMasses(h2Loss);

					//SurfaceAccelerationCMSec2 = Environment.Acceleration(massSM, radius);
					SurfaceAcceleration = Acceleration.FromCentimetersPerSecondSquared(Environment.GetAcceleration(massSM, radius));
					//SurfaceGravityG = Environment.Gravity(SurfaceAcceleration);
				}

				if (heLife < age)
				{
					var heLoss = (1.0 - 1.0 / Math.Exp(age / heLife)) * heMass;

					GasMass -= Mass.FromSolarMasses(heLoss);
					Mass -= Mass.FromSolarMasses(heLoss);

					//SurfaceAccelerationCMSec2 = Environment.Acceleration(massSM, radius);
					SurfaceAcceleration = Acceleration.FromCentimetersPerSecondSquared(Environment.GetAcceleration(massSM, radius));
					//SurfaceGravityG = Environment.Gravity(SurfaceAcceleration);
				}
			}

			//SurfaceGravityG = Environment.Gravity(surfaceAccelerationCMSec2);
		}

		protected override void AdjustPropertiesForGasBody()
		{
			HasGreenhouseEffect = false;
			VolatileGasInventory = Ratio.FromDecimalFractions(GlobalConstants.NOT_APPLICABLE);
			BoilingPointWater = Temperature.FromKelvins(GlobalConstants.NOT_APPLICABLE);

			SurfaceTemperature = Temperature.FromKelvins(GlobalConstants.NOT_APPLICABLE);
			GreenhouseRiseTemperature = TemperatureDelta.FromKelvins(0.0);
			Albedo = Ratio.FromDecimalFractions(Utilities.About(GlobalConstants.GAS_GIANT_ALBEDO, 0.1));
			WaterCoverFraction = Ratio.FromDecimalFractions(0.0);
			CloudCoverFraction = Ratio.FromDecimalFractions(0.0);
			IceCoverFraction = Ratio.FromDecimalFractions(0.0);
		}

		protected override void Generate(Seed seed, int planetNo, Star sun, bool useRandomTilt, string planetID, SystemGenerationOptions genOptions)
		{
			var planet = this;

			planet.OrbitZone = Astro.Astronomy.GetOrbitalZone(sun.Luminosity, SemiMajorAxis);
			planet.OrbitalPeriod = Astro.Astronomy.GetPeriod(SemiMajorAxis, Mass, sun.Mass);
			if (useRandomTilt)
			{
				planet.AxialTilt = Environment.Inclination(SemiMajorAxis);
			}

			planet.ExosphereTemperature = Astro.Thermodynamics.GetExosphereTemperature(SemiMajorAxis, sun.EcosphereRadius, sun.Temperature);
			planet.RMSVelocity = Astro.Physics.GetRMSVelocity(Mass.FromGrams(GlobalConstants.MOL_NITROGEN), ExosphereTemperature);
			planet.CoreRadius = Astro.Planetology.GetCoreRadius(DustMass, OrbitZone, false);

			// Calculate the radius as a gas giant, to verify it will retain gas.
			// Then if mass > Earth, it's at least 5% gas and retains He, it's
			// some flavor of gas giant.

			planet.Density = Astro.Physics.GetDensityFromStar(Mass, SemiMajorAxis, sun.EcosphereRadius, true);
			planet.Radius = Length.FromKilometers(Environment.VolumeRadius(Mass.SolarMasses, Density.GramsPerCubicCentimeter));

			planet.SurfaceAcceleration = GetAcceleration(Mass, Radius);
			//planet.SurfaceGravityG = Environment.Gravity(planet.SurfaceAcceleration);

			planet.MolecularWeightRetained = Astro.Physics.GetMolecularWeightRetained(SurfaceAcceleration, Mass, Radius, ExosphereTemperature, sun.Age);

			// Is the planet a gas giant?
			if (Astro.Planetology.TestIsGasGiant(Mass, GasMass, MolecularWeightRetained))
			{
				//Type = GetGasGiantType(MassSM, GasMassSM);

				AdjustPropertiesForGasBody();
			}
			else // If not, it's rocky.
			{
				Radius = Astro.Planetology.GetCoreRadius(Mass, OrbitZone, false);
				Density = Astro.Physics.GetDensityFromBody(Mass, Radius);

				// Radius has changed, we need to adjust Surfa
				SurfaceAcceleration = GetAcceleration(Mass, Radius);
				//SurfaceGravityG = Environment.Gravity(SurfaceAcceleration);

				AdjustPropertiesForRockyBody();

				MolecularWeightRetained = Astro.Physics.GetMolecularWeightRetained(SurfaceAcceleration, Mass, Radius, ExosphereTemperature, sun.Age);
				HasGreenhouseEffect = Astro.Planetology.TestHasGreenhouseEffect(sun.EcosphereRadius, SemiMajorAxis);
			}

			planet.AngularVelocityRadSec = Astro.Dynamics.GetAngularVelocity(Mass,
																  Radius,
																  Density,
																  SemiMajorAxis,
																  Astro.Planetology.TestIsGasGiant(Mass, GasMass, MolecularWeightRetained),
																  sun.Mass,
																  sun.Age);
			planet.DayLength = Astro.Astronomy.GetDayLength(planet.AngularVelocityRadSec, planet.OrbitalPeriod, planet.Eccentricity);
			planet.HasResonantPeriod = Astro.Planetology.TestHasResonantPeriod(planet.AngularVelocityRadSec, planet.DayLength, planet.OrbitalPeriod, planet.Eccentricity);
			planet.EscapeVelocity = Astro.Dynamics.GetEscapeVelocity(planet.Mass, planet.Radius);
			planet.VolatileGasInventory = Astro.Physics.GetVolatileGasInventory(Mass,
																	   EscapeVelocity,
																	   RMSVelocity,
																	   sun.Mass,
																	   GasMass,
																	   OrbitZone,
																	   HasGreenhouseEffect);
			planet.HillSphere = Astro.Astronomy.GetHillSphere(sun.Mass, planet.Mass, planet.SemiMajorAxis);

			if (!Astro.Planetology.TestIsGasGiant(Mass, GasMass, MolecularWeightRetained))
			{
				Pressure surfpres = Astro.Physics.GetSurfacePressure(planet.VolatileGasInventory, planet.Radius, planet.SurfaceAcceleration);


				planet.BoilingPointWater = Astro.Thermodynamics.GetBoilingPointWater(surfpres);

				// Sets: planet.surf_temp, planet.greenhs_rise, planet.albedo, planet.hydrosphere,
				// planet.cloud_cover, planet.ice_cover
				AdjustSurfaceTemperatures(surfpres);

				planet.IsTidallyLocked = Astro.Planetology.TestIsTidallyLocked(DayLength, OrbitalPeriod);

				// Generate complete atmosphere.
				Atmosphere = new Atmosphere(planet, genOptions.GasTable);
			}

			Type = Astro.Planetology.GetBodyType(Mass,
										 GasMass,
										 MolecularWeightRetained,
										 Atmosphere != null ? Atmosphere.SurfacePressure : Pressure.FromMillibars(0.0),
										 WaterCoverFraction,
										 IceCoverFraction,
										 MaxTemperature,
										 BoilingPointWater,
										 SurfaceTemperature);

		}

		/// <summary>
		/// Calculates the surface acceleration of the planet.
		/// </summary>
		/// <param name="mass">Mass of the planet in solar masses</param>
		/// <param name="radius">Radius of the planet in km</param>
		/// <returns>Acceleration returned in units of cm/sec2</returns>
		private Acceleration GetAcceleration(Mass mass, Length radius)
		{
			return Acceleration.FromCentimetersPerSecondSquared(GlobalConstants.GRAV_CONSTANT * (mass.Grams) / Utilities.Pow2(radius.Centimeters));
		}
		protected override IEnumerable<SatelliteBody> GenerateSatellites(Seed seed,
					   Star star,
					   SatelliteBody parentBody,
					   bool useRandomTilt,
					   SystemGenerationOptions genOptions)
		{
			var planet = parentBody;

			// Generate moons
			var sat = new List<SatelliteBody>();
			//var curMoon = seed;
			var n = 0;
			/*if (curMoon != null)
			{
				while (curMoon != null)
				{
					if (curMoon.Mass * GlobalConstants.SUN_MASS_IN_EARTH_MASSES > .000001)
					{
						curMoon.SemiMajorAxisAU = planet.SemiMajorAxisAU;
						curMoon.Eccentricity = planet.Eccentricity;

						n++;

						string moon_id = string.Format("{0}.{1}", parentBody.Position, n);

						var generatedMoon = new Moon(curMoon, star, planet, n, useRandomTilt, moon_id, genOptions);

						sat.Add(generatedMoon);
					}
					curMoon = curMoon.NextBody;
				}
			}*/

			foreach(var curMoon in seed.Satellites)
			{
				if (curMoon.Mass.EarthMasses > .000001)
				{
					curMoon.SemiMajorAxis = planet.SemiMajorAxis;
					curMoon.Eccentricity = planet.Eccentricity;

					n++;

					string moon_id = string.Format("{0}.{1}", parentBody.Position, n);

					var generatedMoon = new Moon(curMoon, star, planet, n, useRandomTilt, moon_id, genOptions);

					sat.Add(generatedMoon);
				}
			}

			return sat;
		}

		// TODO write summary
		// TODO parameter for number of iterations? does it matter?
		/// <summary>
		/// 
		/// </summary>
		/// <param name="planet"></param>
		protected override void AdjustSurfaceTemperatures(Pressure surfpres)
		{
			var initTemp = Astro.Thermodynamics.GetEstimatedTemperature(Star.EcosphereRadius, SemiMajorAxis, Albedo);

			//var h2Life = GasLife(GlobalConstants.MOL_HYDROGEN, planet);
			//var h2oLife = GasLife(GlobalConstants.WATER_VAPOR, planet);
			//var n2Life = GasLife(GlobalConstants.MOL_NITROGEN, planet);
			//var nLife = GasLife(GlobalConstants.ATOMIC_NITROGEN, planet);

			CalculateSurfaceTemperature(true,
							   Ratio.FromDecimalFractions(0.0),
							   Ratio.FromDecimalFractions(0.0),
							   Ratio.FromDecimalFractions(0.0),
							   Temperature.FromKelvins(0.0),
							   Ratio.FromDecimalFractions(0.0),
							   surfpres);

			for (var count = 0; count <= 25; count++)
			{
				var lastWater = WaterCoverFraction;
				var lastClouds = CloudCoverFraction;
				var lastIce = IceCoverFraction;
				var lastTemp = SurfaceTemperature;
				var lastAlbedo = Albedo;

				CalculateSurfaceTemperature(false, lastWater, lastClouds, lastIce, lastTemp, lastAlbedo, surfpres);

				if (Math.Abs((SurfaceTemperature - lastTemp).Kelvins) < 0.25)
					break;
			}

			GreenhouseRiseTemperature = SurfaceTemperature - initTemp;
		}

		/// <summary>
		/// The temperature of the planet calculated in degrees Kelvin
		/// TODO: Eliminate calls to Environment and replace with calls to Physics.
		/// </summary>
		/// <param name="planet"></param>
		/// <param name="first"></param>
		/// <param name="last_water"></param>
		/// <param name="last_clouds"></param>
		/// <param name="last_ice"></param>
		/// <param name="last_temp"></param>
		/// <param name="last_albedo"></param>
		private void CalculateSurfaceTemperature(bool first, Ratio last_water, Ratio last_clouds, Ratio last_ice, Temperature last_temp, Ratio last_albedo, Pressure surfpres)
		{
			Temperature effectiveTemp;
			Ratio waterRaw;
			Ratio cloudsRaw;
			TemperatureDelta greenhouseTemp;
			bool boilOff = false;

			var planet = this;

			if (first)
			{
				planet.Albedo = Ratio.FromDecimalFractions(GlobalConstants.EARTH_ALBEDO);

				effectiveTemp = Astro.Thermodynamics.GetEffectiveTemperature(planet.Star.EcosphereRadius, planet.SemiMajorAxis, planet.Albedo);
				greenhouseTemp = Astro.Thermodynamics.GetGreenhouseTemperatureRise(Astro.Planetology.GetOpacity(planet.MolecularWeightRetained, surfpres), effectiveTemp, surfpres);
				planet.SurfaceTemperature = effectiveTemp + greenhouseTemp;

				SetTempRange(surfpres.Millibars);
			}

			if (planet.HasGreenhouseEffect && planet.MaxTemperature < planet.BoilingPointWater)
			{
				planet.HasGreenhouseEffect = false;

				planet.VolatileGasInventory = Astro.Physics.GetVolatileGasInventory(planet.Mass,
					planet.EscapeVelocity, planet.RMSVelocity, planet.Star.Mass, planet.GasMass,
					planet.OrbitZone, planet.HasGreenhouseEffect);
				//planet.Atmosphere.SurfacePressure = Pressure(planet.VolatileGasInventory, planet.RadiusKM, planet.SurfaceGravityG);

				planet.BoilingPointWater = Astro.Thermodynamics.GetBoilingPointWater(surfpres);
			}

			waterRaw = planet.WaterCoverFraction = Astro.Planetology.GetWaterFraction(planet.VolatileGasInventory, planet.Radius);
			cloudsRaw = planet.CloudCoverFraction = Astro.Planetology.GetCloudFraction(planet.SurfaceTemperature,
													 planet.MolecularWeightRetained,
													 planet.Radius,
													 planet.WaterCoverFraction);
			planet.IceCoverFraction = Astro.Planetology.GetIceFraction(planet.WaterCoverFraction, planet.SurfaceTemperature);

			if (planet.HasGreenhouseEffect && surfpres.Millibars > 0.0)
			{
				planet.CloudCoverFraction = Ratio.FromDecimalFractions(1.0);
			}

			if (planet.DaytimeTemperature >= planet.BoilingPointWater && !first && !(Astro.Planetology.TestIsTidallyLocked(planet.DayLength, planet.OrbitalPeriod) || planet.HasResonantPeriod))
			{
				planet.WaterCoverFraction = Ratio.FromDecimalFractions(0.0);
				boilOff = true;

				if (planet.MolecularWeightRetained.Grams > GlobalConstants.WATER_VAPOR)
				{
					planet.CloudCoverFraction = Ratio.FromDecimalFractions(0.0);
				}
				else
				{
					planet.CloudCoverFraction = Ratio.FromDecimalFractions(1.0);
				}
			}

			if (planet.SurfaceTemperature.Kelvins < GlobalConstants.FREEZING_POINT_OF_WATER - 3.0)
			{
				planet.WaterCoverFraction = Ratio.FromDecimalFractions(0.0);
			}

			planet.Albedo = Astro.Planetology.GetAlbedo(planet.WaterCoverFraction, planet.CloudCoverFraction, planet.IceCoverFraction, surfpres);

			effectiveTemp = Astro.Thermodynamics.GetEffectiveTemperature(planet.Star.EcosphereRadius, planet.SemiMajorAxis, planet.Albedo);
			greenhouseTemp = Astro.Thermodynamics.GetGreenhouseTemperatureRise(
				Astro.Planetology.GetOpacity(planet.MolecularWeightRetained, surfpres),
				effectiveTemp, surfpres);
			planet.SurfaceTemperature = effectiveTemp + greenhouseTemp;

			if (!first)
			{
				if (!boilOff)
				{
					planet.WaterCoverFraction = (planet.WaterCoverFraction + last_water * 2) / 3;
				}
				planet.CloudCoverFraction = (planet.CloudCoverFraction + last_clouds * 2) / 3;
				planet.IceCoverFraction = (planet.IceCoverFraction + last_ice * 2) / 3;
				planet.Albedo = (planet.Albedo + last_albedo * 2) / 3;
				planet.SurfaceTemperature = Temperature.FromKelvins((planet.SurfaceTemperature.Kelvins + last_temp.Kelvins * 2.0) / 3.0);
			}

			SetTempRange(surfpres.Millibars);
		}

		private double Lim(double x)
		{
			return x / Math.Sqrt(Math.Sqrt(1 + x * x * x * x));
		}

		private double Soft(double v, double max, double min)
		{
			double dv = v - min;
			double dm = max - min;
			return (Lim(2 * dv / dm - 1) + 1) / 2 * dm + min;
		}

		private void SetTempRange(double surfpres)
		{
			var planet = this;

			var pressmod = 1 / Math.Sqrt(1 + 20 * surfpres / 1000.0);
			var ppmod = 1 / Math.Sqrt(10 + 5 * surfpres / 1000.0);
			var tiltmod = Math.Abs(Math.Cos(planet.AxialTilt.Degrees * Math.PI / 180) * Math.Pow(1 + planet.Eccentricity.DecimalFractions, 2));
			var daymod = 1 / (200 / planet.DayLength.Hours + 1);
			var mh = Math.Pow(1 + daymod, pressmod);
			var ml = Math.Pow(1 - daymod, pressmod);
			var hi = mh * planet.SurfaceTemperature.Kelvins;
			var lo = ml * planet.SurfaceTemperature.Kelvins;
			var sh = hi + Math.Pow((100 + hi) * tiltmod, Math.Sqrt(ppmod));
			var wl = lo - Math.Pow((150 + lo) * tiltmod, Math.Sqrt(ppmod));
			var max = planet.SurfaceTemperature.Kelvins + Math.Sqrt(planet.SurfaceTemperature.Kelvins) * 10;
			var min = planet.SurfaceTemperature.Kelvins / Math.Sqrt(planet.DayLength.Hours + 24);

			if (lo < min) lo = min;
			if (wl < 0) wl = 0;

			planet.DaytimeTemperature = Temperature.FromKelvins(Soft(hi, max, min));
			planet.NighttimeTemperature = Temperature.FromKelvins(Soft(lo, max, min));
			planet.MaxTemperature = Temperature.FromKelvins(Soft(sh, max, min));
			planet.MinTemperature = Temperature.FromKelvins(Soft(wl, max, min));
		}

	}
}
