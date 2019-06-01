using System;
using System.Collections.Generic;

namespace Primoris.Universe.Stargen.Data
{

    // TODO break this class up
    // TODO orbit zone is supposedly no longer used anywhere. Check references and possibly remove.

    [Serializable]
    public class Planet : IEquatable<Planet>
    {
        public int Position;
        public Star Star { get; }
		public Atmosphere Atmosphere { get; private set; }

        #region Orbit data

        /// <summary>
        /// Semi-major axis of the body's orbit in astronomical units (au).
        /// </summary>
        public double SemiMajorAxisAU { get; set; }

        /// <summary>
        /// Eccentricity of the body's orbit.
        /// </summary>
        public double Eccentricity { get; set; }

        /// <summary>
        /// Axial tilt of the planet expressed in degrees.
        /// </summary>
        public double AxialTiltDegrees { get; set; }

        /// <summary>
        /// Orbital zone the planet is located in. Value is 1, 2, or 3. Used in
        /// radius and volatile inventory calculations.
        /// </summary>
        public int OrbitZone { get; set; }

        /// <summary>
        /// The length of the planet's year in days.
        /// </summary>
        public double OrbitalPeriodDays { get; set; }

        /// <summary>
        /// Angular velocity about the planet's axis in radians/sec.
        /// </summary>
        public double AngularVelocityRadSec { get; set; }

        /// <summary>
        /// The length of the planet's day in hours.
        /// </summary>
        public double DayLengthHours { get; set; }

        /// <summary>
        /// The Hill sphere of the planet expressed in km.
        /// </summary>
        public double HillSphereKM { get; set; }

        #endregion

        #region Size & mass data

        /// <summary>
        /// The mass of the planet in units of Solar mass.
        /// </summary>
        public double MassSM { get; set; } 

        /// <summary>
        /// The mass of dust retained by the planet (ie, the mass of the planet
        /// sans atmosphere). Given in units of Solar mass.
        /// </summary>
        public double DustMassSM { get; set; }

        /// <summary>
        /// The mass of gas retained by the planet (ie, the mass of its
        /// atmosphere). Given in units of Solar mass.
        /// </summary>
        public double GasMassSM { get; set; }

        /// <summary>
        /// The velocity required to escape from the body given in cm/sec.
        /// </summary>
        public double EscapeVelocityCMSec { get; set; }

        /// <summary>
        /// The gravitational acceleration felt at the surface of the planet. Given in cm/sec^2
        /// </summary>
        public double SurfaceAccelerationCMSec2 { get; set; }

        /// <summary>
        /// The gravitational acceleration felt at the surface of the planet. Given as a fraction of Earth gravity (Gs).
        /// </summary>
        public double SurfaceGravityG { get; set; }

        /// <summary>
        /// The radius of the planet's core in km.
        /// </summary>
        public double CoreRadiusKM { get; set; }

        /// <summary>
        /// The radius of the planet's surface in km.
        /// </summary>
        public double RadiusKM { get; set; }

        /// <summary>
        /// The density of the planet given in g/cc. 
        /// </summary>
        public double DensityGCC { get; set; }

        #endregion

        #region Planet properties

        public PlanetType Type { get; set; }

        public bool IsGasGiant => Type == PlanetType.GasGiant ||
                                  Type == PlanetType.SubGasGiant ||
                                  Type == PlanetType.SubSubGasGiant;

        public bool IsTidallyLocked { get; set; }

        public bool IsEarthlike { get; set; }

        public bool IsHabitable { get; set; }

        public bool HasResonantPeriod { get; set; }

        public bool HasGreenhouseEffect { get; set; }

        #endregion

        #region Moon data

        public List<Planet> Moons { get; set; }

        public double MoonSemiMajorAxisAU { get; set; }

        public double MoonEccentricity { get; set; }

        #endregion

        #region Atmospheric data
        /// <summary>
        /// The root-mean-square velocity of N2 at the planet's exosphere given
        /// in cm/sec. Used to determine where or not a planet is capable of
        /// retaining an atmosphere.
        /// </summary>
        public double RMSVelocityCMSec { get; set; }

        /// <summary>
        /// The smallest molecular weight the planet is capable of retaining.
        /// I believe this is in g/mol.
        /// </summary>
        public double MolecularWeightRetained { get; set; }

        /// <summary>
        /// Unitless value for the inventory of volatile gases that result from
        /// outgassing. Used in the calculation of surface pressure. See Fogg
        /// eq. 16. 
        /// </summary>
        public double VolatileGasInventory { get; set; } 

        /// <summary>
        /// Boiling point of water on the planet given in Kelvin.
        /// </summary>
        public double BoilingPointWaterKelvin { get; set; }

        /// <summary>
        /// Planetary albedo. Unitless value between 0 (no reflection) and 1 
        /// (completely reflective).
        /// </summary>
        public double Albedo { get; set; }

        #endregion

        #region Temperature data
        /// <summary>
        /// Illumination received by the body at at the farthest point of its
        /// orbit. 1.0 is the amount of illumination received by an object 1 au
        /// from the Sun.
        /// </summary>
        public double Illumination { get; set; }

        /// <summary>
        /// Temperature at the body's exosphere given in Kelvin.
        /// </summary>
        public double ExosphereTempKelvin { get; set; }

        /// <summary>
        /// Temperature at the body's surface given in Kelvin.
        /// </summary>
        public double SurfaceTempKelvin { get; set; }

        /// <summary>
        /// Amount (in Kelvin) that the planet's surface temperature is being
        /// increased by a runaway greenhouse effect.
        /// </summary>
        public double GreenhouseRiseKelvin { get; set; }

        /// <summary>
        /// Average daytime temperature in Kelvin.
        /// </summary>
        public double DaytimeTempKelvin { get; set; }

        /// <summary>
        /// Average nighttime temperature in Kelvin.
        /// </summary>
        public double NighttimeTempKelvin { get; set; }

        /// <summary>
        /// Maximum (summer/day) temperature in Kelvin.
        /// </summary>
        public double MaxTempKelvin { get; set; }

        /// <summary>
        /// Minimum (winter/night) temperature in Kelvin.
        /// </summary>
        public double MinTempKelvin { get; set; }

        #endregion

        #region Surface coverage

        /// <summary>
        /// Amount of the body's surface that is covered in water. Given as a
        /// value between 0 (no water) and 1 (completely covered).
        /// </summary>
        public double WaterCoverFraction { get; set; }

        /// <summary>
        /// Amount of the body's surface that is obscured by cloud cover. Given
        /// as a value between 0 (no cloud coverage) and 1 (surface not visible
        /// at all).
        /// </summary>
        public double CloudCoverFraction { get; set; }

        /// <summary>
        /// Amount of the body's surface that is covered in ice. Given as a 
        /// value between 0 (no ice) and 1 (completely covered).
        /// </summary>
        public double IceCoverFraction { get; set; }

        #endregion

        public Planet()
        {

        }

		public Planet(Atmosphere atmos)
		{
			Atmosphere = atmos;
			atmos.Planet = this;
		}

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
					  double surfGrav)
		{
			Star = sun;

			SemiMajorAxisAU = semiMajorAxisAU;
			Eccentricity = eccentricity;
			AxialTiltDegrees = axialTilt;
			OrbitZone = Environment.OrbitalZone(Star.Luminosity, SemiMajorAxisAU);
			DayLengthHours = dayLengthHours;
			OrbitalPeriodDays = orbitalPeriodDays;

			MassSM = massSM;
			GasMassSM = gasMassSM;
			DustMassSM = MassSM - GasMassSM;
			RadiusKM = radius;
			DensityGCC = Environment.EmpiricalDensity(MassSM, SemiMajorAxisAU, Star.EcosphereRadiusAU, true);
			ExosphereTempKelvin = GlobalConstants.EARTH_EXOSPHERE_TEMP / Utilities.Pow2(SemiMajorAxisAU / Star.EcosphereRadiusAU);
			SurfaceAccelerationCMSec2 = Environment.Acceleration(MassSM, RadiusKM);
			EscapeVelocityCMSec = Environment.EscapeVelocity(MassSM, RadiusKM);

			DaytimeTempKelvin = dayTimeTempK;
			NighttimeTempKelvin = nightTimeTempK;
			SurfaceTempKelvin = surfTempK;
			SurfaceGravityG = surfGrav;
			MolecularWeightRetained = Environment.MinMolecularWeight(this);

			Atmosphere = new Atmosphere(this, surfPressure);

			IterateSurfaceTemp();
		}

		public Planet(Star star)
		{
			Star = star;
			Atmosphere = new Atmosphere(this);
		}

        public Planet(PlanetSeed seed, Star star, int num, bool useRandomTilt, string planetID, bool isMoon, SystemGenerationOptions genOptions)
        {
            Star = star;
            Position = num;
            SemiMajorAxisAU = seed.SemiMajorAxisAU;
            Eccentricity = seed.Eccentricity;
            MassSM = seed.Mass;
            DustMassSM = seed.DustMass;
            GasMassSM = seed.GasMass;

			Atmosphere = new Atmosphere(this);

			GeneratePlanet(seed, num, star, useRandomTilt, planetID, isMoon, genOptions);
        }

		private void GeneratePlanet(PlanetSeed seed, int planetNo, Star sun, bool useRandomTilt, string planetID, bool isMoon, SystemGenerationOptions genOptions)
		{
			var planet = this;

			planet.OrbitZone = Environment.OrbitalZone(sun.Luminosity, planet.SemiMajorAxisAU);
			planet.OrbitalPeriodDays = Environment.Period(planet.SemiMajorAxisAU, planet.MassSM, sun.Mass);
			if (useRandomTilt)
			{
				planet.AxialTiltDegrees = Environment.Inclination(planet.SemiMajorAxisAU);
			}
			planet.ExosphereTempKelvin = GlobalConstants.EARTH_EXOSPHERE_TEMP / Utilities.Pow2(planet.SemiMajorAxisAU / sun.EcosphereRadiusAU);
			planet.RMSVelocityCMSec = Environment.RMSVelocity(GlobalConstants.MOL_NITROGEN, planet.ExosphereTempKelvin);
			planet.CoreRadiusKM = Environment.KothariRadius(planet.DustMassSM, false, planet.OrbitZone);

			// Calculate the radius as a gas giant, to verify it will retain gas.
			// Then if mass > Earth, it's at least 5% gas and retains He, it's
			// some flavor of gas giant.

			planet.DensityGCC = Environment.EmpiricalDensity(planet.MassSM, planet.SemiMajorAxisAU, sun.EcosphereRadiusAU, true);
			planet.RadiusKM = Environment.VolumeRadius(planet.MassSM, planet.DensityGCC);

			planet.SurfaceAccelerationCMSec2 = Environment.Acceleration(planet.MassSM, planet.RadiusKM);
			planet.SurfaceGravityG = Environment.Gravity(planet.SurfaceAccelerationCMSec2);

			planet.MolecularWeightRetained = Environment.MinMolecularWeight(planet);

			// Is the planet a gas giant?
			if (((planet.MassSM * GlobalConstants.SUN_MASS_IN_EARTH_MASSES) > 1.0) && ((planet.GasMassSM / planet.MassSM) > 0.05) && (planet.MolecularWeightRetained <= 4.0))
			{
				if ((planet.GasMassSM / planet.MassSM) < 0.20)
				{
					planet.Type = PlanetType.SubSubGasGiant;
				}
				else if ((planet.MassSM * GlobalConstants.SUN_MASS_IN_EARTH_MASSES) < 20.0)
				{
					planet.Type = PlanetType.SubGasGiant;
				}
				else
				{
					planet.Type = PlanetType.GasGiant;
				}
			}
			else // If not, it's rocky.
			{
				planet.RadiusKM = Environment.KothariRadius(planet.MassSM, false, planet.OrbitZone);
				planet.DensityGCC = Environment.VolumeDensity(planet.MassSM, planet.RadiusKM);

				planet.SurfaceAccelerationCMSec2 = Environment.Acceleration(planet.MassSM, planet.RadiusKM);
				planet.SurfaceGravityG = Environment.Gravity(planet.SurfaceAccelerationCMSec2);

				if ((planet.GasMassSM / planet.MassSM) > 0.000001)
				{
					var h2Mass = planet.GasMassSM * 0.85;
					var heMass = (planet.GasMassSM - h2Mass) * 0.999;

					var h2Life = Environment.GasLife(GlobalConstants.MOL_HYDROGEN, planet.ExosphereTempKelvin,
						planet.SurfaceGravityG, planet.RadiusKM);
					var heLife = Environment.GasLife(GlobalConstants.HELIUM, planet.ExosphereTempKelvin,
						planet.SurfaceGravityG, planet.RadiusKM);

					if (h2Life < sun.AgeYears)
					{
						var h2Loss = ((1.0 - (1.0 / Math.Exp(sun.AgeYears / h2Life))) * h2Mass);

						planet.GasMassSM -= h2Loss;
						planet.MassSM -= h2Loss;

						planet.SurfaceAccelerationCMSec2 = Environment.Acceleration(planet.MassSM, planet.RadiusKM);
						planet.SurfaceGravityG = Environment.Gravity(planet.SurfaceAccelerationCMSec2);
					}

					if (heLife < sun.AgeYears)
					{
						var heLoss = ((1.0 - (1.0 / Math.Exp(sun.AgeYears / heLife))) * heMass);

						planet.GasMassSM -= heLoss;
						planet.MassSM -= heLoss;

						planet.SurfaceAccelerationCMSec2 = Environment.Acceleration(planet.MassSM, planet.RadiusKM);
						planet.SurfaceGravityG = Environment.Gravity(planet.SurfaceAccelerationCMSec2);
					}
				}
			}

			planet.AngularVelocityRadSec = Environment.AngularVelocity(planet);
			planet.DayLengthHours = Environment.DayLength(planet.AngularVelocityRadSec, planet.OrbitalPeriodDays,
				planet.Eccentricity);
			planet.HasResonantPeriod = Environment.HasResonantPeriod(planet.AngularVelocityRadSec,
				planet.DayLengthHours, planet.OrbitalPeriodDays, planet.Eccentricity);
			planet.EscapeVelocityCMSec = Environment.EscapeVelocity(planet.MassSM, planet.RadiusKM);
			planet.HillSphereKM = Environment.SimplifiedHillSphereKM(sun.Mass, planet.MassSM, planet.SemiMajorAxisAU);

			if (planet.IsGasGiant)
			{
				planet.HasGreenhouseEffect = false;
				planet.VolatileGasInventory = GlobalConstants.NOT_APPLICABLE;
				// This has been moved to Atmosphere default constructor and have a value of 0.0.
				//planet.Atmosphere.SurfacePressure = GlobalConstants.NOT_APPLICABLE;

				planet.BoilingPointWaterKelvin = GlobalConstants.NOT_APPLICABLE;

				planet.SurfaceTempKelvin = GlobalConstants.NOT_APPLICABLE;
				planet.GreenhouseRiseKelvin = 0;
				planet.Albedo = Utilities.About(GlobalConstants.GAS_GIANT_ALBEDO, 0.1);
				planet.WaterCoverFraction = 1.0;
				planet.CloudCoverFraction = 1.0;
				planet.IceCoverFraction = 0.0;
				planet.SurfaceGravityG = Environment.Gravity(planet.SurfaceAccelerationCMSec2);
				planet.MolecularWeightRetained = Environment.MinMolecularWeight(planet);
				planet.SurfaceGravityG = GlobalConstants.NOT_APPLICABLE;
			}
			else
			{
				planet.SurfaceGravityG = Environment.Gravity(planet.SurfaceAccelerationCMSec2);
				planet.MolecularWeightRetained = Environment.MinMolecularWeight(planet);

				planet.HasGreenhouseEffect = Environment.Greenhouse(sun.EcosphereRadiusAU, planet.SemiMajorAxisAU);
				planet.VolatileGasInventory = Environment.VolatileInventory(
					planet.MassSM, planet.EscapeVelocityCMSec, planet.RMSVelocityCMSec, sun.Mass,
					planet.OrbitZone, planet.HasGreenhouseEffect, (planet.GasMassSM / planet.MassSM) > 0.000001);

				Atmosphere = new Atmosphere(planet, genOptions.GasTable);

				/*planet.Atmosphere.SurfacePressure = Environment.Pressure(
					planet.VolatileGasInventory, planet.RadiusKM, planet.SurfaceGravityG);*/

				planet.BoilingPointWaterKelvin = Math.Abs(planet.Atmosphere.SurfacePressure) < 0.001
					? 0.0
					: Environment.BoilingPoint(planet.Atmosphere.SurfacePressure);

				// Sets: planet.surf_temp, planet.greenhs_rise, planet.albedo, planet.hydrosphere,
				// planet.cloud_cover, planet.ice_cover
				IterateSurfaceTemp();

				planet.IsTidallyLocked = Environment.IsTidallyLocked(planet);

				// Assign planet type
				if (planet.Atmosphere.SurfacePressure < 1.0)
				{
					if (!isMoon && ((planet.MassSM * GlobalConstants.SUN_MASS_IN_EARTH_MASSES) < GlobalConstants.ASTEROID_MASS_LIMIT))
					{
						planet.Type = PlanetType.Asteroids;
					}
					else
					{
						planet.Type = PlanetType.Barren;
					}
				}
				else if ((planet.Atmosphere.SurfacePressure > 6000.0) && (planet.MolecularWeightRetained <= 2.0)) // Retains Hydrogen
				{
					planet.Type = PlanetType.SubSubGasGiant;
					
					// Not necessary. Is already initialized in Atmosphere constructor.
					//planet.Atmosphere.Composition = new List<Gas>();
				}
				else
				{
					// Atmospheres:
					// TODO remove PlanetType enum entirely and replace it with a more flexible classification systme
					if (planet.WaterCoverFraction >= 0.95) // >95% water
					{
						planet.Type = PlanetType.Water;
					}
					else if (planet.IceCoverFraction >= 0.95) // >95% ice
					{
						planet.Type = PlanetType.Ice;
					}
					else if (planet.WaterCoverFraction > 0.05) // Terrestrial
					{
						planet.Type = PlanetType.Terrestrial;
					}
					else if (planet.MaxTempKelvin > planet.BoilingPointWaterKelvin) // Hot = Venusian
					{
						planet.Type = PlanetType.Venusian;
					}
					else if ((planet.GasMassSM / planet.MassSM) > 0.0001) // Accreted gas, but no greenhouse or liquid water make it an ice world
					{
						planet.Type = PlanetType.Ice;
						planet.IceCoverFraction = 1.0;
					}
					else if (planet.Atmosphere.SurfacePressure <= 250.0) // Thin air = Martian
					{
						planet.Type = PlanetType.Martian;
					}
					else if (planet.SurfaceTempKelvin < GlobalConstants.FREEZING_POINT_OF_WATER)
					{
						planet.Type = PlanetType.Ice;
					}
					else
					{
						planet.Type = PlanetType.Undefined;
					}
				}
			}

			// Generate moons
			planet.Moons = new List<Planet>();
			if (!isMoon)
			{
				var curMoon = seed.FirstMoon;
				var n = 0;
				while (curMoon != null)
				{
					if (curMoon.Mass * GlobalConstants.SUN_MASS_IN_EARTH_MASSES > .000001)
					{
						curMoon.SemiMajorAxisAU = planet.SemiMajorAxisAU;
						curMoon.Eccentricity = planet.Eccentricity;

						n++;

						string moon_id = String.Format("{0}.{1}", planetID, n);

						var generatedMoon = new Planet(curMoon, sun, n, useRandomTilt, moon_id, true, genOptions);

						double roche_limit = 2.44 * planet.RadiusKM * Math.Pow((planet.DensityGCC / generatedMoon.DensityGCC), (1.0 / 3.0));
						double hill_sphere = planet.SemiMajorAxisAU * GlobalConstants.KM_PER_AU * Math.Pow((planet.MassSM / (3.0 * sun.Mass)), (1.0 / 3.0));

						if ((roche_limit * 3.0) < hill_sphere)
						{
							generatedMoon.MoonSemiMajorAxisAU = Utilities.RandomNumber(roche_limit * 1.5, hill_sphere / 2.0) / GlobalConstants.KM_PER_AU;
							generatedMoon.MoonEccentricity = Utilities.RandomEccentricity();
						}
						else
						{
							generatedMoon.MoonSemiMajorAxisAU = 0;
							generatedMoon.MoonEccentricity = 0;
						}
						planet.Moons.Add(generatedMoon);
					}
					curMoon = curMoon.NextPlanet;
				}
			}
		}

		// TODO write summary
		// TODO parameter for number of iterations? does it matter?
		/// <summary>
		/// 
		/// </summary>
		/// <param name="planet"></param>
		private void IterateSurfaceTemp()
		{
			var initTemp = Environment.EstTemp(Star.EcosphereRadiusAU, SemiMajorAxisAU, Albedo);

			//var h2Life = GasLife(GlobalConstants.MOL_HYDROGEN, planet);
			//var h2oLife = GasLife(GlobalConstants.WATER_VAPOR, planet);
			//var n2Life = GasLife(GlobalConstants.MOL_NITROGEN, planet);
			//var nLife = GasLife(GlobalConstants.ATOMIC_NITROGEN, planet);

			CalculateSurfaceTemperature(true, 0, 0, 0, 0, 0);

			for (var count = 0; count <= 25; count++)
			{
				var lastWater = WaterCoverFraction;
				var lastClouds = CloudCoverFraction;
				var lastIce = IceCoverFraction;
				var lastTemp = SurfaceTempKelvin;
				var lastAlbedo = Albedo;

				CalculateSurfaceTemperature(false, lastWater, lastClouds, lastIce, lastTemp, lastAlbedo);

				if (Math.Abs(SurfaceTempKelvin - lastTemp) < 0.25)
					break;
			}

			GreenhouseRiseKelvin = SurfaceTempKelvin - initTemp;
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
		private void CalculateSurfaceTemperature(bool first, double last_water, double last_clouds, double last_ice, double last_temp, double last_albedo)
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
														 planet.Atmosphere.SurfacePressure),
												 effectiveTemp,
												 planet.Atmosphere.SurfacePressure);
				planet.SurfaceTempKelvin = effectiveTemp + greenhouseTemp;

				SetTempRange();
			}

			if (planet.HasGreenhouseEffect && planet.MaxTempKelvin < planet.BoilingPointWaterKelvin)
			{
				planet.HasGreenhouseEffect = false;

				planet.VolatileGasInventory = Environment.VolatileInventory(planet.MassSM,
					planet.EscapeVelocityCMSec, planet.RMSVelocityCMSec, planet.Star.Mass,
					planet.OrbitZone, planet.HasGreenhouseEffect, (planet.GasMassSM / planet.MassSM) > 0.000001);
				//planet.Atmosphere.SurfacePressure = Pressure(planet.VolatileGasInventory, planet.RadiusKM, planet.SurfaceGravityG);

				planet.BoilingPointWaterKelvin = Environment.BoilingPoint(planet.Atmosphere.SurfacePressure);
			}

			waterRaw = planet.WaterCoverFraction = Environment.HydroFraction(planet.VolatileGasInventory, planet.RadiusKM);
			cloudsRaw = planet.CloudCoverFraction = Environment.CloudFraction(planet.SurfaceTempKelvin,
													 planet.MolecularWeightRetained,
													 planet.RadiusKM,
													 planet.WaterCoverFraction);
			planet.IceCoverFraction = Environment.IceFraction(planet.WaterCoverFraction, planet.SurfaceTempKelvin);

			if ((planet.HasGreenhouseEffect) && (planet.Atmosphere.SurfacePressure > 0.0))
			{
				planet.CloudCoverFraction = 1.0;
			}

			if ((planet.DaytimeTempKelvin >= planet.BoilingPointWaterKelvin) && (!first) && !(Environment.IsTidallyLocked(planet) || planet.HasResonantPeriod))
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

			planet.Albedo = Environment.PlanetAlbedo(planet.WaterCoverFraction, planet.CloudCoverFraction, planet.IceCoverFraction, planet.Atmosphere.SurfacePressure);

			effectiveTemp = Environment.EffTemp(planet.Star.EcosphereRadiusAU, planet.SemiMajorAxisAU, planet.Albedo);
			greenhouseTemp = Environment.GreenRise(
				Environment.Opacity(planet.MolecularWeightRetained, planet.Atmosphere.SurfacePressure),
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

			SetTempRange();
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

		private void SetTempRange()
		{
			var planet = this;

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

		public bool Equals(Planet other)
        {
            return Position == other.Position &&
                Utilities.AlmostEqual(SemiMajorAxisAU, other.SemiMajorAxisAU) &&
                Utilities.AlmostEqual(Eccentricity, other.Eccentricity) &&
                Utilities.AlmostEqual(AxialTiltDegrees, other.AxialTiltDegrees) &&
                OrbitZone == other.OrbitZone &&
                Utilities.AlmostEqual(OrbitalPeriodDays, other.OrbitalPeriodDays) &&
                Utilities.AlmostEqual(DayLengthHours, other.DayLengthHours) &&
                Utilities.AlmostEqual(HillSphereKM, other.HillSphereKM) &&
                Utilities.AlmostEqual(MassSM, other.MassSM) &&
                Utilities.AlmostEqual(DustMassSM, other.DustMassSM) &&
                Utilities.AlmostEqual(GasMassSM, other.GasMassSM) &&
                Utilities.AlmostEqual(EscapeVelocityCMSec, other.EscapeVelocityCMSec) &&
                Utilities.AlmostEqual(SurfaceAccelerationCMSec2, other.SurfaceAccelerationCMSec2) &&
                Utilities.AlmostEqual(SurfaceGravityG, other.SurfaceGravityG) &&
                Utilities.AlmostEqual(CoreRadiusKM, other.CoreRadiusKM) &&
                Utilities.AlmostEqual(RadiusKM, other.RadiusKM) &&
                Utilities.AlmostEqual(DensityGCC, other.DensityGCC) &&
                Moons.Count == other.Moons.Count &&
                Utilities.AlmostEqual(RMSVelocityCMSec, other.RMSVelocityCMSec) &&
                Utilities.AlmostEqual(MolecularWeightRetained, other.MolecularWeightRetained) &&
                Utilities.AlmostEqual(VolatileGasInventory, other.VolatileGasInventory) &&
                Utilities.AlmostEqual(BoilingPointWaterKelvin, other.BoilingPointWaterKelvin) &&
                Utilities.AlmostEqual(Albedo, other.Albedo) &&
                Utilities.AlmostEqual(Illumination, other.Illumination) &&
                Utilities.AlmostEqual(ExosphereTempKelvin, other.ExosphereTempKelvin) &&
                Utilities.AlmostEqual(SurfaceTempKelvin, other.SurfaceTempKelvin) &&
                Utilities.AlmostEqual(GreenhouseRiseKelvin, other.GreenhouseRiseKelvin) &&
                Utilities.AlmostEqual(DaytimeTempKelvin, other.DaytimeTempKelvin) &&
                Utilities.AlmostEqual(NighttimeTempKelvin, other.NighttimeTempKelvin) &&
                Utilities.AlmostEqual(MaxTempKelvin, other.MaxTempKelvin) &&
                Utilities.AlmostEqual(MinTempKelvin, other.MinTempKelvin) &&
                Utilities.AlmostEqual(WaterCoverFraction, other.WaterCoverFraction) &&
                Utilities.AlmostEqual(CloudCoverFraction, other.CloudCoverFraction) &&
                Utilities.AlmostEqual(IceCoverFraction, other.IceCoverFraction);
        }
    }
}
