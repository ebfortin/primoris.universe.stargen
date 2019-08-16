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
		public Planet(Seed seed, StellarBody star, Body parentBody) : base(seed, star, parentBody)
		{
			Generate();
		}

		public Planet(Seed seed, StellarBody star, Body parentBody, IEnumerable<Layer> layers) : base(seed, star, parentBody, layers)
		{

		}

		public Planet(StellarBody sun,
					  Body parentBody,
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
					  Acceleration surfGrav) : base(new Seed(semiMajorAxisAU, eccentricity, massSM, massSM - gasMassSM, gasMassSM), sun, parentBody)
		{
			Parent = parentBody;
			StellarBody = sun;

			SemiMajorAxis = semiMajorAxisAU;
			Eccentricity = eccentricity;
			AxialTilt = axialTilt;
			OrbitZone = Science.Astronomy.GetOrbitalZone(sun.Luminosity, SemiMajorAxis);
			DayLength = dayLengthHours;
			OrbitalPeriod = orbitalPeriodDays;

			Mass = massSM;
			GasMass = gasMassSM;
			DustMass = massSM - GasMass;
			Radius = radius;
			Density = Science.Physics.GetDensityFromStar(Mass, SemiMajorAxis, sun.EcosphereRadius, true);
			ExosphereTemperature = Temperature.FromKelvins(GlobalConstants.EARTH_EXOSPHERE_TEMP / Utilities.Pow2(SemiMajorAxis / sun.EcosphereRadius));
			SurfaceAcceleration = surfGrav; //Acceleration.FromCentimetersPerSecondSquared(GlobalConstants.GRAV_CONSTANT * massSM.Grams / Utilities.Pow2(radius.Centimeters));
			EscapeVelocity = Science.Dynamics.GetEscapeVelocity(GasMass + DustMass, Radius);

			DaytimeTemperature = dayTimeTempK;
			NighttimeTemperature = nightTimeTempK;
			Temperature = surfTempK;
			//SurfaceGravityG = surfGrav;
			MolecularWeightRetained = Science.Physics.GetMolecularWeightRetained(SurfaceAcceleration, GasMass + DustMass, Radius, ExosphereTemperature, sun.Age);

			VolatileGasInventory = Science.Physics.GetVolatileGasInventory(GasMass + DustMass,
																  EscapeVelocity,
																  RMSVelocity,
																  sun.Mass,
																  GasMass,
																  OrbitZone,
																  Science.Planetology.TestHasGreenhouseEffect(sun.EcosphereRadius, SemiMajorAxis));
			SurfacePressure = Science.Physics.GetSurfacePressure(VolatileGasInventory, Radius, SurfaceAcceleration);

			if(Science.Planetology.TestIsGasGiant(Mass, GasMass, MolecularWeightRetained))
			{
				Layers.Add(new BasicGiantGaseousLayer().Generate(this, Mass, Chemical.Load(), Layers));
			} else
			{
				// Add basic Burrows layer.
				Layers.Add(new BasicSolidLayer().Generate(this, DustMass, new Chemical[0], new Layer[0]));

				// Generate complete atmosphere.
				Layers.Add(new BasicGaseousLayer(SurfacePressure).Generate(this, GasMass, Chemical.Load(), Layers));
			}

			IsForming = false;
		}


		private void AdjustPropertiesForRockyBody()
		{
			// TODO: Remove last references to Environment.
			double age = Parent.Age.Years365;
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

				var h2Life = Science.Physics.GetGasLife(Mass.FromGrams(GlobalConstants.MOL_HYDROGEN), ExosphereTemperature, SurfaceAcceleration, Radius).Years365;
				var heLife = Science.Physics.GetGasLife(Mass.FromGrams(GlobalConstants.HELIUM), ExosphereTemperature, SurfaceAcceleration, Radius).Years365;

				if (h2Life < age)
				{
					var h2Loss = (1.0 - 1.0 / Math.Exp(age / h2Life)) * h2Mass;

					GasMass -= Mass.FromSolarMasses(h2Loss);
					//Mass -= Mass.FromSolarMasses(h2Loss);

					//SurfaceAccelerationCMSec2 = Environment.Acceleration(massSM, radius);
					//SurfaceAcceleration = Acceleration.FromCentimetersPerSecondSquared(Environment.GetAcceleration(massSM, radius));
					SurfaceAcceleration = Science.Physics.GetAcceleration(Mass, Radius);
					//SurfaceGravityG = Environment.Gravity(SurfaceAcceleration);
				}

				if (heLife < age)
				{
					var heLoss = (1.0 - 1.0 / Math.Exp(age / heLife)) * heMass;

					GasMass -= Mass.FromSolarMasses(heLoss);
					//Mass -= Mass.FromSolarMasses(heLoss);

					//SurfaceAccelerationCMSec2 = Environment.Acceleration(massSM, radius);
					//SurfaceAcceleration = Acceleration.FromCentimetersPerSecondSquared(Environment.GetAcceleration(massSM, radius));
					SurfaceAcceleration = Science.Physics.GetAcceleration(Mass, Radius);
					//SurfaceGravityG = Environment.Gravity(SurfaceAcceleration);
				}
			}

			//SurfaceGravityG = Environment.Gravity(surfaceAccelerationCMSec2);
		}

		private void AdjustPropertiesForGasBody()
		{
			HasGreenhouseEffect = false;
			VolatileGasInventory = Ratio.FromDecimalFractions(GlobalConstants.NOT_APPLICABLE);
			BoilingPointWater = Temperature.FromKelvins(GlobalConstants.NOT_APPLICABLE);

			Temperature = Temperature.FromKelvins(GlobalConstants.NOT_APPLICABLE);
			GreenhouseRiseTemperature = TemperatureDelta.Zero;
			Albedo = Ratio.FromDecimalFractions(Utilities.About(GlobalConstants.GAS_GIANT_ALBEDO, 0.1));
			WaterCoverFraction = Ratio.Zero;
			CloudCoverFraction = Ratio.Zero;
			IceCoverFraction = Ratio.Zero;
		}

		private Angle GetRandomInclination(Length semiMajorAxis)
		{
			var inclination = ((int)(Math.Pow(semiMajorAxis.Kilometers / GlobalConstants.ASTRONOMICAL_UNIT_KM, 0.2) * Utilities.About(GlobalConstants.EARTH_AXIAL_TILT, 0.4)) % 360);
			return Angle.FromDegrees(inclination);
		}

		protected override void Generate()
		{
			var planet = this;
			var sun = StellarBody;
			var mass = GasMass + DustMass;

			planet.OrbitZone = Science.Astronomy.GetOrbitalZone(sun.Luminosity, SemiMajorAxis);
			planet.OrbitalPeriod = Science.Astronomy.GetPeriod(SemiMajorAxis, mass, sun.Mass);

			planet.AxialTilt = GetRandomInclination(SemiMajorAxis);

			planet.ExosphereTemperature = Science.Thermodynamics.GetExosphereTemperature(SemiMajorAxis, sun.EcosphereRadius, sun.Temperature);
			planet.RMSVelocity = Science.Physics.GetRMSVelocity(Mass.FromGrams(GlobalConstants.MOL_NITROGEN), ExosphereTemperature);
			planet.CoreRadius = Science.Planetology.GetCoreRadius(DustMass, OrbitZone, false);

			// Calculate the radius as a gas giant, to verify it will retain gas.
			// Then if mass > Earth, it's at least 5% gas and retains He, it's
			// some flavor of gas giant.

			planet.Density = Science.Physics.GetDensityFromStar(mass, SemiMajorAxis, sun.EcosphereRadius, true);
			planet.Radius = Mathematics.GetRadiusFromVolume(mass, Density);

			planet.SurfaceAcceleration = GetAcceleration(mass, Radius);
			//planet.SurfaceGravityG = Environment.Gravity(planet.SurfaceAcceleration);

			planet.MolecularWeightRetained = Science.Physics.GetMolecularWeightRetained(SurfaceAcceleration, mass, Radius, ExosphereTemperature, sun.Age);

			// Is the planet a gas giant?
			if (Science.Planetology.TestIsGasGiant(mass, GasMass, MolecularWeightRetained))
			{
				//Type = GetGasGiantType(MassSM, GasMassSM);

				AdjustPropertiesForGasBody();
				SurfacePressure = Pressure.Zero;
				Layers.Clear();
				Layers.Add(new BasicGiantGaseousLayer().Generate(this, GasMass, new Chemical[0], Layers));
			}
			else // If not, it's rocky.
			{
				Radius = Science.Planetology.GetCoreRadius(mass, OrbitZone, false);
				Density = Science.Physics.GetDensityFromBody(mass, Radius);

				// Radius has changed, we need to adjust Surfa
				SurfaceAcceleration = GetAcceleration(mass, Radius);
				//SurfaceGravityG = Environment.Gravity(SurfaceAcceleration);

				AdjustPropertiesForRockyBody();

				MolecularWeightRetained = Science.Physics.GetMolecularWeightRetained(SurfaceAcceleration, mass, Radius, ExosphereTemperature, sun.Age);
			}

			planet.AngularVelocity = Science.Dynamics.GetAngularVelocity(mass,
																  Radius,
																  Density,
																  SemiMajorAxis,
																  Science.Planetology.TestIsGasGiant(Mass, GasMass, MolecularWeightRetained),
																  sun.Mass,
																  sun.Age);
			planet.DayLength = Science.Astronomy.GetDayLength(planet.AngularVelocity, planet.OrbitalPeriod, planet.Eccentricity);
			planet.HasResonantPeriod = Science.Planetology.TestHasResonantPeriod(planet.AngularVelocity, planet.DayLength, planet.OrbitalPeriod, planet.Eccentricity);
			planet.EscapeVelocity = Science.Dynamics.GetEscapeVelocity(mass, planet.Radius);
			planet.VolatileGasInventory = Science.Physics.GetVolatileGasInventory(mass,
																	   EscapeVelocity,
																	   RMSVelocity,
																	   sun.Mass,
																	   GasMass,
																	   OrbitZone,
																	   HasGreenhouseEffect);
			planet.HillSphere = Science.Astronomy.GetHillSphere(sun.Mass, mass, planet.SemiMajorAxis);

			if (!Science.Planetology.TestIsGasGiant(mass, GasMass, MolecularWeightRetained))
			{
				Pressure surfpres = Science.Physics.GetSurfacePressure(planet.VolatileGasInventory, planet.Radius, planet.SurfaceAcceleration);

				// Calculate all atmosphere layers total mass.
				if (GasMass == Mass.Zero)
				{
					Area surf = Area.FromSquareKilometers(4.0 * Math.PI * Math.Pow(planet.Radius.Kilometers, 2.0));
					GasMass = Mass.FromKilograms(surf.SquareMeters * surfpres.NewtonsPerSquareMeter / planet.SurfaceAcceleration.MetersPerSecondSquared);
				}
				planet.BoilingPointWater = Science.Thermodynamics.GetBoilingPointWater(surfpres);

				// Sets: planet.surf_temp, planet.greenhs_rise, planet.albedo, planet.hydrosphere,
				// planet.cloud_cover, planet.ice_cover
				AdjustSurfaceTemperatures(surfpres);

				planet.IsTidallyLocked = Science.Planetology.TestIsTidallyLocked(DayLength, OrbitalPeriod);

				// Add basic Burrows layer.
				Layers.Add(new BasicSolidLayer().Generate(this, DustMass, new Chemical[0], new Layer[0]));

				// Generate complete atmosphere.
				if(surfpres.Millibars > 0.0 && GasMass.SolarMasses > 0.0)
					Layers.Add(new BasicGaseousLayer(surfpres).Generate(this, GasMass, Chemical.Load(), Layers));
				SurfacePressure = surfpres;

                HasGreenhouseEffect = Science.Planetology.TestHasGreenhouseEffect(sun.EcosphereRadius, SemiMajorAxis) & SurfacePressure > Pressure.Zero;
            }

			Type = Science.Planetology.GetBodyType(Mass,
										 GasMass,
										 MolecularWeightRetained,
										 SurfacePressure,
										 WaterCoverFraction,
										 IceCoverFraction,
										 MaxTemperature,
										 BoilingPointWater,
										 Temperature);


			IsForming = false;
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

		protected override IEnumerable<SatelliteBody> GenerateSatellites(Seed seed, SystemGenerationOptions genOptions)
		{
			var planet = this;
			var star = StellarBody;

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

					string moon_id = string.Format("{0}.{1}", this.Position, n);

					var generatedMoon = new Moon(curMoon, star, planet, moon_id, genOptions);

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
		private void AdjustSurfaceTemperatures(Pressure surfpres)
		{
			var initTemp = Science.Thermodynamics.GetEstimatedTemperature(StellarBody.EcosphereRadius, SemiMajorAxis, Albedo);

			//var h2Life = GasLife(GlobalConstants.MOL_HYDROGEN, planet);
			//var h2oLife = GasLife(GlobalConstants.WATER_VAPOR, planet);
			//var n2Life = GasLife(GlobalConstants.MOL_NITROGEN, planet);
			//var nLife = GasLife(GlobalConstants.ATOMIC_NITROGEN, planet);

			CalculateSurfaceTemperature(true,
							   Ratio.Zero,
							   Ratio.Zero,
							   Ratio.Zero,
							   Temperature.Zero,
							   Ratio.Zero,
							   surfpres);

			for (var count = 0; count <= 25; count++)
			{
				var lastWater = WaterCoverFraction;
				var lastClouds = CloudCoverFraction;
				var lastIce = IceCoverFraction;
				var lastTemp = Temperature;
				var lastAlbedo = Albedo;

				CalculateSurfaceTemperature(false, lastWater, lastClouds, lastIce, lastTemp, lastAlbedo, surfpres);

				if (Math.Abs((Temperature - lastTemp).Kelvins) < 0.25)
					break;
			}

			GreenhouseRiseTemperature = Temperature - initTemp;
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

				effectiveTemp = Science.Thermodynamics.GetEffectiveTemperature(planet.StellarBody.EcosphereRadius, planet.SemiMajorAxis, planet.Albedo);
				greenhouseTemp = Science.Thermodynamics.GetGreenhouseTemperatureRise(Science.Planetology.GetOpacity(planet.MolecularWeightRetained, surfpres), effectiveTemp, surfpres);
				planet.Temperature = effectiveTemp + greenhouseTemp;

				SetTempRange(surfpres.Millibars);
			}

			if (planet.HasGreenhouseEffect && planet.MaxTemperature < planet.BoilingPointWater)
			{
				planet.HasGreenhouseEffect = false;

				planet.VolatileGasInventory = Science.Physics.GetVolatileGasInventory(planet.Mass,
					planet.EscapeVelocity, planet.RMSVelocity, planet.Parent.Mass, planet.GasMass,
					planet.OrbitZone, planet.HasGreenhouseEffect);
				//planet.Atmosphere.SurfacePressure = Pressure(planet.VolatileGasInventory, planet.RadiusKM, planet.SurfaceGravityG);

				planet.BoilingPointWater = Science.Thermodynamics.GetBoilingPointWater(surfpres);
			}

			waterRaw = planet.WaterCoverFraction = Science.Planetology.GetWaterFraction(planet.VolatileGasInventory, planet.Radius);
			cloudsRaw = planet.CloudCoverFraction = Science.Planetology.GetCloudFraction(planet.Temperature,
													 planet.MolecularWeightRetained,
													 planet.Radius,
													 planet.WaterCoverFraction);
			planet.IceCoverFraction = Science.Planetology.GetIceFraction(planet.WaterCoverFraction, planet.Temperature);

			if (planet.HasGreenhouseEffect && surfpres.Millibars > 0.0)
			{
				planet.CloudCoverFraction = Ratio.FromDecimalFractions(1.0);
			}

			if (planet.DaytimeTemperature >= planet.BoilingPointWater && !first && !(Science.Planetology.TestIsTidallyLocked(planet.DayLength, planet.OrbitalPeriod) || planet.HasResonantPeriod))
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

			if (planet.Temperature.Kelvins < GlobalConstants.FREEZING_POINT_OF_WATER - 3.0)
			{
				planet.WaterCoverFraction = Ratio.FromDecimalFractions(0.0);
			}

			planet.Albedo = Science.Planetology.GetAlbedo(planet.WaterCoverFraction, planet.CloudCoverFraction, planet.IceCoverFraction, surfpres);

			effectiveTemp = Science.Thermodynamics.GetEffectiveTemperature(planet.StellarBody.EcosphereRadius, planet.SemiMajorAxis, planet.Albedo);
			greenhouseTemp = Science.Thermodynamics.GetGreenhouseTemperatureRise(
				Science.Planetology.GetOpacity(planet.MolecularWeightRetained, surfpres),
				effectiveTemp, surfpres);
			planet.Temperature = effectiveTemp + greenhouseTemp;

			if (!first)
			{
				if (!boilOff)
				{
					planet.WaterCoverFraction = (planet.WaterCoverFraction + last_water * 2) / 3;
				}
				planet.CloudCoverFraction = (planet.CloudCoverFraction + last_clouds * 2) / 3;
				planet.IceCoverFraction = (planet.IceCoverFraction + last_ice * 2) / 3;
				planet.Albedo = (planet.Albedo + last_albedo * 2) / 3;
				planet.Temperature = Temperature.FromKelvins((planet.Temperature.Kelvins + last_temp.Kelvins * 2.0) / 3.0);
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
			var hi = mh * planet.Temperature.Kelvins;
			var lo = ml * planet.Temperature.Kelvins;
			var sh = hi + Math.Pow((100 + hi) * tiltmod, Math.Sqrt(ppmod));
			var wl = lo - Math.Pow((150 + lo) * tiltmod, Math.Sqrt(ppmod));
			var max = planet.Temperature.Kelvins + Math.Sqrt(planet.Temperature.Kelvins) * 10;
			var min = planet.Temperature.Kelvins / Math.Sqrt(planet.DayLength.Hours + 24);

			if (lo < min) lo = min;
			if (wl < 0) wl = 0;

			planet.DaytimeTemperature = Temperature.FromKelvins(Soft(hi, max, min));
			planet.NighttimeTemperature = Temperature.FromKelvins(Soft(lo, max, min));
			planet.MaxTemperature = Temperature.FromKelvins(Soft(sh, max, min));
			planet.MinTemperature = Temperature.FromKelvins(Soft(wl, max, min));
		}

	}
}
