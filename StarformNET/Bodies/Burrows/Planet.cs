using System;
using System.Linq;
using System.Collections.Generic;
using Primoris.Universe.Stargen.Astrophysics;
using Primoris.Universe.Stargen.Systems;
using Environment = Primoris.Universe.Stargen.Astrophysics.Environment;
using Primoris.Universe.Stargen.Astrophysics.Burrows;

namespace Primoris.Universe.Stargen.Bodies.Burrows
{

	// TODO break this class up
	// TODO orbit zone is supposedly no longer used anywhere. Check references and possibly remove.

	[Serializable]
	public class Planet : SatelliteBody
	{
		private static readonly IBodyPhysics BurrowsPhysics = new BodyPhysics();


		public Planet(Star sun, Atmosphere atmos) : base(BurrowsPhysics, sun, atmos) { }

		public Planet(Star sun,
					  double semiMajorAxisAU,
					  double eccentricity,
					  double axialTilt,
					  double dayLengthHours,
					  double orbitalPeriodDays,
					  double massSM,
					  double gasMassSM,
					  double radius,
					  double surfPressure,
					  double dayTimeTempK,
					  double nightTimeTempK,
					  double surfTempK,
					  double surfGrav) : base(BurrowsPhysics,
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


		public Planet(Star star) : base(BurrowsPhysics, star) { }

		protected override void AdjustPropertiesForRockyBody()
		{
			double age = Star.Age;
			double massSM = MassSM;
			double gasMassSM = GasMassSM;
			double exosphereTemperature = ExosphereTemperature;
			double surfaceGravityG = SurfaceGravityG;
			double radius = Radius;
			double surfaceAccelerationCMSec2 = SurfaceAccelerationCMSec2;

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

					GasMassSM -= h2Loss;
					MassSM -= h2Loss;

					SurfaceAccelerationCMSec2 = Environment.Acceleration(massSM, radius);
					SurfaceGravityG = Environment.Gravity(surfaceAccelerationCMSec2);
				}

				if (heLife < age)
				{
					var heLoss = (1.0 - 1.0 / Math.Exp(age / heLife)) * heMass;

					GasMassSM -= heLoss;
					MassSM -= heLoss;

					SurfaceAccelerationCMSec2 = Environment.Acceleration(massSM, radius);
					SurfaceGravityG = Environment.Gravity(surfaceAccelerationCMSec2);
				}
			}

			//SurfaceGravityG = Environment.Gravity(surfaceAccelerationCMSec2);
		}

		protected override void AdjustPropertiesForGasBody()
		{
			HasGreenhouseEffect = false;
			VolatileGasInventory = GlobalConstants.NOT_APPLICABLE;
			BoilingPointWater = GlobalConstants.NOT_APPLICABLE;

			SurfaceTemperature = GlobalConstants.NOT_APPLICABLE;
			GreenhouseRiseTemperature = 0;
			Albedo = Utilities.About(GlobalConstants.GAS_GIANT_ALBEDO, 0.1);
			WaterCoverFraction = 0.0;
			CloudCoverFraction = 0.0;
			IceCoverFraction = 0.0;
		}

		protected override void Generate(Seed seed, int planetNo, Star sun, bool useRandomTilt, string planetID, SystemGenerationOptions genOptions)
		{
			var planet = this;

			planet.OrbitZone = Physics.GetOrbitalZone(sun.LuminositySM, SemiMajorAxisAU);
			planet.OrbitalPeriod = Physics.GetPeriod(SemiMajorAxisAU, MassSM, sun.MassSM);
			if (useRandomTilt)
			{
				planet.AxialTilt = Environment.Inclination(SemiMajorAxisAU);
			}

			planet.ExosphereTemperature = Physics.GetExosphereTemperature(SemiMajorAxisAU, sun.EcosphereRadiusAU, sun.Temperature);
			planet.RMSVelocityCMSec = Physics.GetRMSVelocityCMSec(ExosphereTemperature);
			planet.CoreRadius = Physics.GetCoreRadius(DustMassSM, OrbitZone, false);

			// Calculate the radius as a gas giant, to verify it will retain gas.
			// Then if mass > Earth, it's at least 5% gas and retains He, it's
			// some flavor of gas giant.

			planet.DensityGCC = Physics.GetDensityFromStar(MassSM, SemiMajorAxisAU, sun.EcosphereRadiusAU, true);
			planet.Radius = Environment.VolumeRadius(MassSM, DensityGCC);

			planet.SurfaceAccelerationCMSec2 = Environment.Acceleration(MassSM, Radius);
			planet.SurfaceGravityG = Environment.Gravity(planet.SurfaceAccelerationCMSec2);

			planet.MolecularWeightRetained = Physics.GetMolecularWeightRetained(SurfaceGravityG, MassSM, Radius, ExosphereTemperature, sun.Age);

			// Is the planet a gas giant?
			if (Physics.TestIsGasGiant(MassSM, GasMassSM, MolecularWeightRetained))
			{
				//Type = GetGasGiantType(MassSM, GasMassSM);

				AdjustPropertiesForGasBody();
			}
			else // If not, it's rocky.
			{
				Radius = Physics.GetCoreRadius(MassSM, OrbitZone, false);
				DensityGCC = Physics.GetDensityFromBody(MassSM, Radius);

				// Radius has changed, we need to adjust Surfa
				SurfaceAccelerationCMSec2 = Environment.Acceleration(MassSM, Radius);
				SurfaceGravityG = Environment.Gravity(SurfaceAccelerationCMSec2);

				AdjustPropertiesForRockyBody();

				MolecularWeightRetained = Physics.GetMolecularWeightRetained(SurfaceGravityG, MassSM, Radius, ExosphereTemperature, sun.Age);
				HasGreenhouseEffect = Physics.TestHasGreenhouseEffect(sun.EcosphereRadius, SemiMajorAxisAU);
			}

			planet.AngularVelocityRadSec = Physics.GetAngularVelocity(MassSM,
																  Radius,
																  DensityGCC,
																  SemiMajorAxisAU,
																  Physics.TestIsGasGiant(MassSM, GasMassSM, MolecularWeightRetained),
																  sun.MassSM,
																  sun.Age);
			planet.DayLength = Physics.GetDayLength(planet.AngularVelocityRadSec, planet.OrbitalPeriod, planet.Eccentricity);
			planet.HasResonantPeriod = Physics.TestHasResonantPeriod(planet.AngularVelocityRadSec, planet.DayLength, planet.OrbitalPeriod, planet.Eccentricity);
			planet.EscapeVelocityCMSec = Physics.GetEscapeVelocity(planet.MassSM, planet.Radius);
			planet.VolatileGasInventory = Physics.GetVolatileGasInventory(MassSM,
																	   EscapeVelocityCMSec,
																	   RMSVelocityCMSec,
																	   sun.MassSM,
																	   GasMassSM,
																	   OrbitZone,
																	   HasGreenhouseEffect);
			planet.HillSphere = Physics.GetHillSphere(sun.MassSM, planet.MassSM, planet.SemiMajorAxisAU);

			if (!Physics.TestIsGasGiant(MassSM, GasMassSM, MolecularWeightRetained))
			{
				double surfpres = Physics.GetSurfacePressure(planet.VolatileGasInventory, planet.Radius, planet.SurfaceGravityG);


				planet.BoilingPointWater = Physics.GetBoilingPointWater(surfpres);

				// Sets: planet.surf_temp, planet.greenhs_rise, planet.albedo, planet.hydrosphere,
				// planet.cloud_cover, planet.ice_cover
				AdjustSurfaceTemperatures(surfpres);

				planet.IsTidallyLocked = Physics.TestIsTidallyLocked(DayLength, OrbitalPeriod);

				// Generate complete atmosphere.
				Atmosphere = new Atmosphere(planet, genOptions.GasTable);
			}

			Type = Physics.GetBodyType(MassSM,
				 GasMassSM,
				 MolecularWeightRetained,
				 Atmosphere != null ? Atmosphere.SurfacePressure : 0.0,
				 WaterCoverFraction,
				 IceCoverFraction,
				 MaxTemperature,
				 BoilingPointWater,
				 SurfaceTemperature);

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
				if (curMoon.Mass * GlobalConstants.SUN_MASS_IN_EARTH_MASSES > .000001)
				{
					curMoon.SemiMajorAxisAU = planet.SemiMajorAxisAU;
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
		protected override void AdjustSurfaceTemperatures(double surfpres)
		{
			var initTemp = Environment.EstTemp(Star.EcosphereRadiusAU, SemiMajorAxisAU, Albedo);

			//var h2Life = GasLife(GlobalConstants.MOL_HYDROGEN, planet);
			//var h2oLife = GasLife(GlobalConstants.WATER_VAPOR, planet);
			//var n2Life = GasLife(GlobalConstants.MOL_NITROGEN, planet);
			//var nLife = GasLife(GlobalConstants.ATOMIC_NITROGEN, planet);

			CalculateSurfaceTemperature(true, 0, 0, 0, 0, 0, surfpres);

			for (var count = 0; count <= 25; count++)
			{
				var lastWater = WaterCoverFraction;
				var lastClouds = CloudCoverFraction;
				var lastIce = IceCoverFraction;
				var lastTemp = SurfaceTemperature;
				var lastAlbedo = Albedo;

				CalculateSurfaceTemperature(false, lastWater, lastClouds, lastIce, lastTemp, lastAlbedo, surfpres);

				if (Math.Abs(SurfaceTemperature - lastTemp) < 0.25)
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
		private void CalculateSurfaceTemperature(bool first, double last_water, double last_clouds, double last_ice, double last_temp, double last_albedo, double surfpres)
		{
			double effectiveTemp;
			double waterRaw;
			double cloudsRaw;
			double greenhouseTemp;
			bool boilOff = false;

			var planet = this;

			if (first)
			{
				planet.Albedo = GlobalConstants.EARTH_ALBEDO;

				effectiveTemp = Environment.EffTemp(planet.Star.EcosphereRadiusAU, planet.SemiMajorAxisAU, planet.Albedo);
				greenhouseTemp = Environment.GreenRise(Environment.Opacity(planet.MolecularWeightRetained,
														 surfpres),
												 effectiveTemp,
												 surfpres);
				planet.SurfaceTemperature = effectiveTemp + greenhouseTemp;

				SetTempRange(surfpres);
			}

			if (planet.HasGreenhouseEffect && planet.MaxTemperature < planet.BoilingPointWater)
			{
				planet.HasGreenhouseEffect = false;

				planet.VolatileGasInventory = Environment.VolatileInventory(planet.MassSM,
					planet.EscapeVelocityCMSec, planet.RMSVelocityCMSec, planet.Star.MassSM,
					planet.OrbitZone, planet.HasGreenhouseEffect, planet.GasMassSM / planet.MassSM > 0.000001);
				//planet.Atmosphere.SurfacePressure = Pressure(planet.VolatileGasInventory, planet.RadiusKM, planet.SurfaceGravityG);

				planet.BoilingPointWater = Environment.BoilingPoint(surfpres);
			}

			waterRaw = planet.WaterCoverFraction = Environment.HydroFraction(planet.VolatileGasInventory, planet.Radius);
			cloudsRaw = planet.CloudCoverFraction = Environment.CloudFraction(planet.SurfaceTemperature,
													 planet.MolecularWeightRetained,
													 planet.Radius,
													 planet.WaterCoverFraction);
			planet.IceCoverFraction = Environment.IceFraction(planet.WaterCoverFraction, planet.SurfaceTemperature);

			if (planet.HasGreenhouseEffect && surfpres > 0.0)
			{
				planet.CloudCoverFraction = 1.0;
			}

			if (planet.DaytimeTemperature >= planet.BoilingPointWater && !first && !(Environment.IsTidallyLocked(planet) || planet.HasResonantPeriod))
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

			if (planet.SurfaceTemperature < GlobalConstants.FREEZING_POINT_OF_WATER - 3.0)
			{
				planet.WaterCoverFraction = 0.0;
			}

			planet.Albedo = Environment.PlanetAlbedo(planet.WaterCoverFraction, planet.CloudCoverFraction, planet.IceCoverFraction, surfpres);

			effectiveTemp = Environment.EffTemp(planet.Star.EcosphereRadiusAU, planet.SemiMajorAxisAU, planet.Albedo);
			greenhouseTemp = Environment.GreenRise(
				Environment.Opacity(planet.MolecularWeightRetained, surfpres),
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
				planet.SurfaceTemperature = (planet.SurfaceTemperature + last_temp * 2) / 3;
			}

			SetTempRange(surfpres);
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
			var tiltmod = Math.Abs(Math.Cos(planet.AxialTilt * Math.PI / 180) * Math.Pow(1 + planet.Eccentricity, 2));
			var daymod = 1 / (200 / planet.DayLength + 1);
			var mh = Math.Pow(1 + daymod, pressmod);
			var ml = Math.Pow(1 - daymod, pressmod);
			var hi = mh * planet.SurfaceTemperature;
			var lo = ml * planet.SurfaceTemperature;
			var sh = hi + Math.Pow((100 + hi) * tiltmod, Math.Sqrt(ppmod));
			var wl = lo - Math.Pow((150 + lo) * tiltmod, Math.Sqrt(ppmod));
			var max = planet.SurfaceTemperature + Math.Sqrt(planet.SurfaceTemperature) * 10;
			var min = planet.SurfaceTemperature / Math.Sqrt(planet.DayLength + 24);

			if (lo < min) lo = min;
			if (wl < 0) wl = 0;

			planet.DaytimeTemperature = Soft(hi, max, min);
			planet.NighttimeTemperature = Soft(lo, max, min);
			planet.MaxTemperature = Soft(sh, max, min);
			planet.MinTemperature = Soft(wl, max, min);
		}

	}
}
