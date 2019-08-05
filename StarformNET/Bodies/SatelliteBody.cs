using System;
using System.Linq;
using System.Collections.Generic;
using Primoris.Universe.Stargen.Astrophysics;
using Primoris.Universe.Stargen.Systems;
using Primoris.Universe.Stargen.Services;
using Environment = Primoris.Universe.Stargen.Astrophysics.Environment;
using Primoris.Universe.Stargen.Astrophysics.Burrows;
using UnitsNet;


namespace Primoris.Universe.Stargen.Bodies
{

    public delegate SatelliteBody CreateSatelliteBodyDelegate(Seed seed,
                                                            StellarBody star,
															int pos,
                                                            bool useRandomTilt,
                                                            string planetID,
                                                            SystemGenerationOptions genOptions);


	[Serializable]
	public abstract class SatelliteBody : Body, IEquatable<SatelliteBody>
	{


		#region Orbit data

		public Length SemiMajorAxis { get; protected set; } = Length.FromAstronomicalUnits(1.0);

		/// <summary>
		/// Eccentricity of the body's orbit.
		/// </summary>
		public Ratio Eccentricity { get; protected set; } = Ratio.Zero;

		/// <summary>
		/// Axial tilt of the planet expressed in degrees.
		/// </summary>
		public Angle AxialTilt { get; protected set; } = Angle.Zero;

		/// <summary>
		/// Orbital zone the planet is located in. Value is 1, 2, or 3. Used in
		/// radius and volatile inventory calculations.
		/// </summary>
		public int OrbitZone { get; protected set; } = 1;

		/// <summary>
		/// The length of the planet's year in days.
		/// </summary>
		public Duration OrbitalPeriod { get; protected set; }

		/// <summary>
		/// Angular velocity about the planet's axis in radians/sec.
		/// </summary>
		public RotationalSpeed AngularVelocity { get; protected set; }

		/// <summary>
		/// The length of the planet's day in hours.
		/// </summary>
		public Duration DayLength { get; protected set; }

		/// <summary>
		/// The Hill sphere of the planet expressed in km.
		/// </summary>
		public Length HillSphere { get; protected set; }

		#endregion

		#region Size & mass data

		/// <summary>
		/// The mass of the planet in units of Solar mass.
		/// </summary>
		//public override Mass Mass { get; protected set; }

		/// <summary>
		/// The mass of dust retained by the planet (ie, the mass of the planet
		/// sans atmosphere). Given in units of Solar mass.
		/// </summary>
		public Mass DustMass { get; protected set; }


		/// <summary>
		/// The mass of gas retained by the planet (ie, the mass of its
		/// atmosphere). Given in units of Solar mass.
		/// </summary>
		public Mass GasMass { get; protected set; }

        /// <summary>
        /// The velocity required to escape from the body given in cm/sec.
        /// </summary>
        //public double EscapeVelocityCMSec { get; protected set; }

        /// <summary>
        /// The gravitational acceleration felt at the surface of the planet. Given in cm/sec^2
        /// </summary>
        //public override Speed EscapeVelocity { get; protected set; }

        /// <summary>
        /// The gravitational acceleration felt at the surface of the planet. Given as a fraction of Earth gravity (Gs).
        /// </summary>
        public Acceleration SurfaceAcceleration { get; protected set; }

		/// <summary>
		/// The radius of the planet's core in km.
		/// </summary>
		public Length CoreRadius { get; protected set; }

		/// <summary>
		/// The radius of the planet's surface in km.
		/// </summary>
		//public override Length Radius { get; protected set; }

		/// <summary>
		/// The density of the planet given in g/cc. 
		/// </summary>
		//public double DensityGCC { get; protected set; }

		public Density Density { get; protected set; }

		#endregion

		#region Planet properties

		public BodyType Type { get; protected set; }

		public bool IsGasGiant => Type == BodyType.GasGiant ||
								  Type == BodyType.SubGasGiant ||
								  Type == BodyType.SubSubGasGiant;

		public bool IsTidallyLocked { get; protected set; }

		public bool IsEarthlike { get; protected set; }

		public bool IsHabitable { get; protected set; }

		public bool HasResonantPeriod { get; protected set; }

		public bool HasGreenhouseEffect { get; protected set; }

        #endregion


        #region Atmospheric data

        public Atmosphere Atmosphere { get; protected set; }

        /// <summary>
        /// The root-mean-square velocity of N2 at the planet's exosphere given
        /// in cm/sec. Used to determine where or not a planet is capable of
        /// retaining an atmosphere.
        /// </summary>
        public Speed RMSVelocity { get; protected set; }

		/// <summary>
		/// The smallest molecular weight the planet is capable of retaining.
		/// I believe this is in g/mol.
		/// </summary>
		public Mass MolecularWeightRetained { get; protected set; }

		/// <summary>
		/// Unitless value for the inventory of volatile gases that result from
		/// outgassing. Used in the calculation of surface pressure. See Fogg
		/// eq. 16. 
		/// </summary>
		public Ratio VolatileGasInventory { get; protected set; }

		/// <summary>
		/// Boiling point of water on the planet given in Kelvin.
		/// </summary>
		public Temperature BoilingPointWater { get; protected set; }

		/// <summary>
		/// Planetary albedo. Unitless value between 0 (no reflection) and 1 
		/// (completely reflective).
		/// </summary>
		public Ratio Albedo { get; protected set; }

		#endregion

		#region Temperature data
		/// <summary>
		/// Illumination received by the body at at the farthest point of its
		/// orbit. 1.0 is the amount of illumination received by an object 1 au
		/// from the Sun.
		/// </summary>
		public Ratio Illumination { get; protected set; }

		/// <summary>
		/// Temperature at the body's exosphere given in Kelvin.
		/// </summary>
		public Temperature ExosphereTemperature { get; protected set; }

		/// <summary>
		/// Temperature at the body's surface given in Kelvin.
		/// </summary>
		//public override Temperature Temperature { get; protected set; }

		/// <summary>
		/// Amount (in Kelvin) that the planet's surface temperature is being
		/// increased by a runaway greenhouse effect.
		/// </summary>
		public TemperatureDelta GreenhouseRiseTemperature { get; protected set; }

		/// <summary>
		/// Average daytime temperature in Kelvin.
		/// </summary>
		public Temperature DaytimeTemperature { get; protected set; }

		/// <summary>
		/// Average nighttime temperature in Kelvin.
		/// </summary>
		public Temperature NighttimeTemperature { get; protected set; }

		/// <summary>
		/// Maximum (summer/day) temperature in Kelvin.
		/// </summary>
		public Temperature MaxTemperature { get; protected set; }

		/// <summary>
		/// Minimum (winter/night) temperature in Kelvin.
		/// </summary>
		public Temperature MinTemperature { get; protected set; }

		#endregion

		#region Surface coverage

		/// <summary>
		/// Amount of the body's surface that is covered in water. Given as a
		/// value between 0 (no water) and 1 (completely covered).
		/// </summary>
		public Ratio WaterCoverFraction { get; protected set; }

		/// <summary>
		/// Amount of the body's surface that is obscured by cloud cover. Given
		/// as a value between 0 (no cloud coverage) and 1 (surface not visible
		/// at all).
		/// </summary>
		public Ratio CloudCoverFraction { get; protected set; }

		/// <summary>
		/// Amount of the body's surface that is covered in ice. Given as a 
		/// value between 0 (no ice) and 1 (completely covered).
		/// </summary>
		public Ratio IceCoverFraction { get; protected set; }

        #endregion


		public SatelliteBody(StellarBody sun, Body parentBody, Atmosphere atmos) : this(Provider.Use().GetService<IScienceAstrophysics>(), sun, parentBody, atmos) { }
        public SatelliteBody(IScienceAstrophysics phy, StellarBody sun, Body parentBody, Atmosphere atmos)
		{
            Science = phy;

			Parent = parentBody;
            StellarBody = sun;
			Atmosphere = atmos;
			atmos.Planet = this;
			Check();
		}

		public SatelliteBody(StellarBody sun,
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
					  Acceleration surfGrav) : this(Provider.Use().GetService<IScienceAstrophysics>(),
									 sun,
									 parentBody,
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
									 surfGrav) { } 
		public SatelliteBody(IScienceAstrophysics phy,
                      StellarBody sun,
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
					  Acceleration surfGrav)
		{
            Science = phy;

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
			DustMass = Mass - GasMass;
			Radius = radius;
			Density = Science.Physics.GetDensityFromStar(Mass, SemiMajorAxis, sun.EcosphereRadius, true);
			ExosphereTemperature = Temperature.FromKelvins(GlobalConstants.EARTH_EXOSPHERE_TEMP / Utilities.Pow2(SemiMajorAxis / sun.EcosphereRadius));
			SurfaceAcceleration = surfGrav; //Acceleration.FromCentimetersPerSecondSquared(GlobalConstants.GRAV_CONSTANT * massSM.Grams / Utilities.Pow2(radius.Centimeters));
			EscapeVelocity = Science.Dynamics.GetEscapeVelocity(Mass, Radius);

			DaytimeTemperature = dayTimeTempK;
			NighttimeTemperature = nightTimeTempK;
			Temperature = surfTempK;
			//SurfaceGravityG = surfGrav;
			MolecularWeightRetained = Science.Physics.GetMolecularWeightRetained(SurfaceAcceleration, Mass, Radius, ExosphereTemperature, sun.Age);

			Atmosphere = new Atmosphere(this, surfPressure);

			AdjustSurfaceTemperatures(surfPressure);
			Check();
		}

		public SatelliteBody(StellarBody star, Body parentBody) : this(Provider.Use().GetService<IScienceAstrophysics>(), star, parentBody) { }
		/// <summary>
		/// TODO: This constructor do not work!!!
		/// </summary>
		/// <param name="phys"></param>
		/// <param name="star"></param>
		public SatelliteBody(IScienceAstrophysics phy, StellarBody star, Body parentBody)
		{
            Science = phy;

            StellarBody = star;
			Parent = parentBody;

			Check();
		}

		public SatelliteBody(StellarBody star, Body parentBody, Gas[] atmosComp) : this(Provider.Use().GetService<IScienceAstrophysics>(), star, parentBody, atmosComp) { }
		public SatelliteBody(IScienceAstrophysics phy, StellarBody star, Body parentBody, Gas[] atmosComp)
		{
            Science = phy;

            StellarBody = star;
			Parent = parentBody;

			Atmosphere = new Atmosphere(this, atmosComp);

			Check();
		} 

		public SatelliteBody(Seed seed,
					   StellarBody star,
					   Body parentBody,
					   bool useRandomTilt,
					   string planetID,
					   SystemGenerationOptions genOptions) : this(Provider.Use().GetService<IScienceAstrophysics>(),
												   seed, 
												   star,
												   parentBody,
												   useRandomTilt,
												   planetID,
												   genOptions) { }
		public SatelliteBody(IScienceAstrophysics phy,
					   Seed seed,
					   StellarBody star,
					   Body parentBody,
					   bool useRandomTilt,
					   string planetID,
					   SystemGenerationOptions genOptions)
		{
            Science = phy;

            StellarBody = star;
			Parent = parentBody;

			SemiMajorAxis = seed.SemiMajorAxis;
			Eccentricity = seed.Eccentricity;
			Mass = seed.Mass;
			DustMass = seed.DustMass;
			GasMass = seed.GasMass;

			Generate(seed, star, useRandomTilt, planetID, genOptions);
			Satellites = GenerateSatellites(seed, star, this, useRandomTilt, genOptions);

			Check();
		}

        private void Check()
		{
			Atmosphere ??= new Atmosphere(this);

			Illumination = Science.Astronomy.GetMinimumIllumination(SemiMajorAxis, StellarBody.Luminosity);
			IsHabitable = Science.Planetology.TestIsHabitable(DayLength, OrbitalPeriod, Atmosphere.Breathability, HasResonantPeriod, IsTidallyLocked);
			IsEarthlike = Science.Planetology.TestIsEarthLike(Temperature,
												   WaterCoverFraction,
												   CloudCoverFraction,
												   IceCoverFraction,
												   Atmosphere.SurfacePressure,
												   SurfaceAcceleration,
												   Atmosphere.Breathability,
												   Type);
		}

		protected abstract void AdjustPropertiesForRockyBody();


		protected abstract void AdjustPropertiesForGasBody();


		protected abstract void Generate(Seed seed, StellarBody sun, bool useRandomTilt, string planetID, SystemGenerationOptions genOptions);

		protected abstract IEnumerable<SatelliteBody> GenerateSatellites(Seed seed, StellarBody star, SatelliteBody parentBody, bool useRandomTilt, SystemGenerationOptions genOptions);


		// TODO write summary
		// TODO parameter for number of iterations? does it matter?
		/// <summary>
		/// 
		/// </summary>
		/// <param name="planet"></param>
		protected abstract void AdjustSurfaceTemperatures(Pressure surfpres);
		

		public void RecalculateGases(Chemical[] gasTable)
		{
			Atmosphere.RecalculateGases(gasTable);
		}

		public bool Equals(SatelliteBody other)
		{
			return Position == other.Position &&
				Utilities.AlmostEqual(SemiMajorAxis.Value, other.SemiMajorAxis.Value) &&
				Utilities.AlmostEqual(Eccentricity.Value, other.Eccentricity.Value) &&
				Utilities.AlmostEqual(AxialTilt.Value, other.AxialTilt.Value) &&
				OrbitZone == other.OrbitZone &&
				Utilities.AlmostEqual(OrbitalPeriod.Value, other.OrbitalPeriod.Value) &&
				Utilities.AlmostEqual(DayLength.Value, other.DayLength.Value) &&
				Utilities.AlmostEqual(HillSphere.Value, other.HillSphere.Value) &&
				Utilities.AlmostEqual(Mass.Value, other.Mass.Value) &&
				Utilities.AlmostEqual(DustMass.Value, other.DustMass.Value) &&
				Utilities.AlmostEqual(GasMass.Value, other.GasMass.Value) &&
				Utilities.AlmostEqual(EscapeVelocity.CentimetersPerSecond, other.EscapeVelocity.CentimetersPerSecond) &&
				Utilities.AlmostEqual(SurfaceAcceleration.CentimetersPerSecondSquared, other.SurfaceAcceleration.CentimetersPerSecondSquared) &&
				Utilities.AlmostEqual(SurfaceAcceleration.StandardGravity, other.SurfaceAcceleration.StandardGravity) &&
				Utilities.AlmostEqual(CoreRadius.Kilometers, other.CoreRadius.Kilometers) &&
				Utilities.AlmostEqual(Radius.Kilometers, other.Radius.Kilometers) &&
				Utilities.AlmostEqual(Density.GramsPerCubicCentimeter, other.Density.GramsPerCubicCentimeter) &&
				Satellites.Count() == other.Satellites.Count() &&
				Utilities.AlmostEqual(RMSVelocity.CentimetersPerSecond, other.RMSVelocity.CentimetersPerSecond) &&
				Utilities.AlmostEqual(MolecularWeightRetained.Kilograms, other.MolecularWeightRetained.Kilograms) &&
				Utilities.AlmostEqual(VolatileGasInventory.Value, other.VolatileGasInventory.Value) &&
				Utilities.AlmostEqual(BoilingPointWater.Kelvins, other.BoilingPointWater.Kelvins) &&
				Utilities.AlmostEqual(Albedo.Value, other.Albedo.Value) &&
				Utilities.AlmostEqual(Illumination.Value, other.Illumination.Value) &&
				Utilities.AlmostEqual(ExosphereTemperature.Kelvins, other.ExosphereTemperature.Kelvins) &&
				Utilities.AlmostEqual(Temperature.Kelvins, other.Temperature.Kelvins) &&
				Utilities.AlmostEqual(GreenhouseRiseTemperature.Kelvins, other.GreenhouseRiseTemperature.Kelvins) &&
				Utilities.AlmostEqual(DaytimeTemperature.Kelvins, other.DaytimeTemperature.Kelvins) &&
				Utilities.AlmostEqual(NighttimeTemperature.Kelvins, other.NighttimeTemperature.Kelvins) &&
				Utilities.AlmostEqual(MaxTemperature.Kelvins, other.MaxTemperature.Kelvins) &&
				Utilities.AlmostEqual(MinTemperature.Kelvins, other.MinTemperature.Kelvins) &&
				Utilities.AlmostEqual(WaterCoverFraction.Value, other.WaterCoverFraction.Value) &&
				Utilities.AlmostEqual(CloudCoverFraction.Value, other.CloudCoverFraction.Value) &&
				Utilities.AlmostEqual(IceCoverFraction.Value, other.IceCoverFraction.Value);
		}
	}
}
