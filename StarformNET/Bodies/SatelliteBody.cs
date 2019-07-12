using System;
using System.Linq;
using System.Collections.Generic;
using Primoris.Universe.Stargen.Astrophysics;
using Primoris.Universe.Stargen.Systems;
using Environment = Primoris.Universe.Stargen.Astrophysics.Environment;
using Primoris.Universe.Stargen.Astrophysics.Burrows;

namespace Primoris.Universe.Stargen.Bodies
{

	// TODO break this class up
	// TODO orbit zone is supposedly no longer used anywhere. Check references and possibly remove.

	[Serializable]
	public abstract class SatelliteBody : Body, IEquatable<SatelliteBody>
	{
		public IBodyPhysics Physics { get; set; }

		public int Position { get; protected set; }
		//private readonly double SemiAxisMajorAU;

		public Star Star { get; }
		public Atmosphere Atmosphere { get; protected set; }

		#region Orbit data

		/// <summary>
		/// Semi-major axis of the body's orbit in astronomical units (au).
		/// </summary>
		public double SemiMajorAxisAU { get; protected set; }

		public double SemiMajorAxis { get => SemiMajorAxisAU * GlobalConstants.ASTRONOMICAL_UNIT_KM; }

		/// <summary>
		/// Eccentricity of the body's orbit.
		/// </summary>
		public double Eccentricity { get; protected set; }

		/// <summary>
		/// Axial tilt of the planet expressed in degrees.
		/// </summary>
		public double AxialTilt { get; protected set; }

		/// <summary>
		/// Orbital zone the planet is located in. Value is 1, 2, or 3. Used in
		/// radius and volatile inventory calculations.
		/// </summary>
		public int OrbitZone { get; protected set; }

		/// <summary>
		/// The length of the planet's year in days.
		/// </summary>
		public double OrbitalPeriod { get; protected set; }

		/// <summary>
		/// Angular velocity about the planet's axis in radians/sec.
		/// </summary>
		public double AngularVelocityRadSec { get; protected set; }

		/// <summary>
		/// The length of the planet's day in hours.
		/// </summary>
		public double DayLength { get; protected set; }

		/// <summary>
		/// The Hill sphere of the planet expressed in km.
		/// </summary>
		public double HillSphere { get; protected set; }

		#endregion

		#region Size & mass data

		/// <summary>
		/// The mass of the planet in units of Solar mass.
		/// </summary>
		public override double MassSM { get; protected set; }

		public double MassKg { get => MassSM * GlobalConstants.SOLAR_MASS_IN_KILOGRAMS; }

		/// <summary>
		/// The mass of dust retained by the planet (ie, the mass of the planet
		/// sans atmosphere). Given in units of Solar mass.
		/// </summary>
		public double DustMassSM { get; protected set; }

		public double DustMass { get => DustMassSM * GlobalConstants.SOLAR_MASS_IN_KILOGRAMS; }

		/// <summary>
		/// The mass of gas retained by the planet (ie, the mass of its
		/// atmosphere). Given in units of Solar mass.
		/// </summary>
		public double GasMassSM { get; protected set; }

		public double GasMass { get => GasMassSM * GlobalConstants.SOLAR_MASS_IN_KILOGRAMS; }

		/// <summary>
		/// The velocity required to escape from the body given in cm/sec.
		/// </summary>
		public double EscapeVelocityCMSec { get; protected set; }

		public double EscapeVelocity { get => EscapeVelocityCMSec * GlobalConstants.CMSEC_TO_MSEC; }

		/// <summary>
		/// The gravitational acceleration felt at the surface of the planet. Given in cm/sec^2
		/// </summary>
		public double SurfaceAccelerationCMSec2 { get; protected set; }

		public double SurfaceAcceleration { get => SurfaceAccelerationCMSec2 * GlobalConstants.CMSEC2_TO_MSEC2; }

		/// <summary>
		/// The gravitational acceleration felt at the surface of the planet. Given as a fraction of Earth gravity (Gs).
		/// </summary>
		public double SurfaceGravityG { get; protected set; }

		/// <summary>
		/// The radius of the planet's core in km.
		/// </summary>
		public double CoreRadius { get; protected set; }

		/// <summary>
		/// The radius of the planet's surface in km.
		/// </summary>
		public double Radius { get; protected set; }

		/// <summary>
		/// The density of the planet given in g/cc. 
		/// </summary>
		public double DensityGCC { get; protected set; }

		public double Density { get => DensityGCC * GlobalConstants.GCM3_TO_KGM3; }

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

		#region Satellites data

		public IEnumerable<SatelliteBody> Satellites { get; protected set; }

		public double MoonSemiMajorAxisAU { get; protected set; }

		public double MoonEccentricity { get; protected set; }

		#endregion

		#region Atmospheric data
		/// <summary>
		/// The root-mean-square velocity of N2 at the planet's exosphere given
		/// in cm/sec. Used to determine where or not a planet is capable of
		/// retaining an atmosphere.
		/// </summary>
		public double RMSVelocityCMSec { get; protected set; }

		public double RMSVelocity { get => RMSVelocityCMSec * GlobalConstants.CMSEC_TO_MSEC; }

		/// <summary>
		/// The smallest molecular weight the planet is capable of retaining.
		/// I believe this is in g/mol.
		/// </summary>
		public double MolecularWeightRetained { get; protected set; }

		/// <summary>
		/// Unitless value for the inventory of volatile gases that result from
		/// outgassing. Used in the calculation of surface pressure. See Fogg
		/// eq. 16. 
		/// </summary>
		public double VolatileGasInventory { get; protected set; }

		/// <summary>
		/// Boiling point of water on the planet given in Kelvin.
		/// </summary>
		public double BoilingPointWater { get; protected set; }

		/// <summary>
		/// Planetary albedo. Unitless value between 0 (no reflection) and 1 
		/// (completely reflective).
		/// </summary>
		public double Albedo { get; protected set; }

		#endregion

		#region Temperature data
		/// <summary>
		/// Illumination received by the body at at the farthest point of its
		/// orbit. 1.0 is the amount of illumination received by an object 1 au
		/// from the Sun.
		/// </summary>
		public double Illumination { get; protected set; }

		/// <summary>
		/// Temperature at the body's exosphere given in Kelvin.
		/// </summary>
		public double ExosphereTemperature { get; protected set; }

		/// <summary>
		/// Temperature at the body's surface given in Kelvin.
		/// </summary>
		public double SurfaceTemperature { get; protected set; }

		/// <summary>
		/// Amount (in Kelvin) that the planet's surface temperature is being
		/// increased by a runaway greenhouse effect.
		/// </summary>
		public double GreenhouseRiseTemperature { get; protected set; }

		/// <summary>
		/// Average daytime temperature in Kelvin.
		/// </summary>
		public double DaytimeTemperature { get; protected set; }

		/// <summary>
		/// Average nighttime temperature in Kelvin.
		/// </summary>
		public double NighttimeTemperature { get; protected set; }

		/// <summary>
		/// Maximum (summer/day) temperature in Kelvin.
		/// </summary>
		public double MaxTemperature { get; protected set; }

		/// <summary>
		/// Minimum (winter/night) temperature in Kelvin.
		/// </summary>
		public double MinTemperature { get; protected set; }

		#endregion

		#region Surface coverage

		/// <summary>
		/// Amount of the body's surface that is covered in water. Given as a
		/// value between 0 (no water) and 1 (completely covered).
		/// </summary>
		public double WaterCoverFraction { get; protected set; }

		/// <summary>
		/// Amount of the body's surface that is obscured by cloud cover. Given
		/// as a value between 0 (no cloud coverage) and 1 (surface not visible
		/// at all).
		/// </summary>
		public double CloudCoverFraction { get; protected set; }

		/// <summary>
		/// Amount of the body's surface that is covered in ice. Given as a 
		/// value between 0 (no ice) and 1 (completely covered).
		/// </summary>
		public double IceCoverFraction { get; protected set; }

		#endregion

		public SatelliteBody(IBodyPhysics phys, Star sun, Atmosphere atmos)
		{
			Physics = phys;

			Star = sun;
			Atmosphere = atmos;
			atmos.Planet = this;
			Check();
		}

		public SatelliteBody(IBodyPhysics phys,
					  Star sun,
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
			Physics = phys;

			Star = sun;

			SemiMajorAxisAU = semiMajorAxisAU;
			Eccentricity = eccentricity;
			AxialTilt = axialTilt;
			OrbitZone = Physics.GetOrbitalZone(Star.LuminositySM, SemiMajorAxisAU);
			DayLength = dayLengthHours;
			OrbitalPeriod = orbitalPeriodDays;

			MassSM = massSM;
			GasMassSM = gasMassSM;
			DustMassSM = MassSM - GasMassSM;
			Radius = radius;
			DensityGCC = Physics.GetDensityFromStar(MassSM, SemiMajorAxisAU, Star.EcosphereRadiusAU, true);
			ExosphereTemperature = GlobalConstants.EARTH_EXOSPHERE_TEMP / Utilities.Pow2(SemiMajorAxisAU / Star.EcosphereRadiusAU);
			SurfaceAccelerationCMSec2 = Environment.Acceleration(MassSM, Radius);
			EscapeVelocityCMSec = Physics.GetEscapeVelocity(MassSM, Radius);

			DaytimeTemperature = dayTimeTempK;
			NighttimeTemperature = nightTimeTempK;
			SurfaceTemperature = surfTempK;
			SurfaceGravityG = surfGrav;
			MolecularWeightRetained = Environment.MinMolecularWeight(this);

			Atmosphere = new Atmosphere(this, surfPressure);

			AdjustSurfaceTemperatures(surfPressure);
			Check();
		}

		/// <summary>
		/// TODO: This constructor do not work!!!
		/// </summary>
		/// <param name="phys"></param>
		/// <param name="star"></param>
		public SatelliteBody(IBodyPhysics phys, Star star)
		{
			Physics = phys;

			Star = star;
			Check();
		}


		public SatelliteBody(IBodyPhysics phys, Seed seed, Star star, int num, bool useRandomTilt, string planetID, SystemGenerationOptions genOptions)
		{
			Physics = phys;

			Star = star;
			Position = num;
			SemiMajorAxisAU = seed.SemiMajorAxisAU;
			Eccentricity = seed.Eccentricity;
			MassSM = seed.Mass;
			DustMassSM = seed.DustMass;
			GasMassSM = seed.GasMass;

			Generate(seed, num, star, useRandomTilt, planetID, genOptions);
			Satellites = GenerateSatellites(seed.FirstSatellite, Star, this, useRandomTilt, genOptions);

			Check();
		}

		private void Check()
		{
			Atmosphere ??= new Atmosphere(this);

			Illumination = Physics.GetMinimumIllumination(SemiMajorAxisAU, Star.LuminositySM);
			IsHabitable = Physics.TestIsHabitable(DayLength, OrbitalPeriod, Atmosphere.Breathability, HasResonantPeriod, IsTidallyLocked);
			IsEarthlike = Physics.TestIsEarthLike(SurfaceTemperature, WaterCoverFraction, CloudCoverFraction, IceCoverFraction, Atmosphere.SurfacePressure, SurfaceGravityG, Atmosphere.Breathability, Type);
		}

		protected abstract void AdjustPropertiesForRockyBody();


		protected abstract void AdjustPropertiesForGasBody();


		protected abstract void Generate(Seed seed, int planetNo, Star sun, bool useRandomTilt, string planetID, SystemGenerationOptions genOptions);

		protected abstract IEnumerable<SatelliteBody> GenerateSatellites(Seed seed, Star star, SatelliteBody parentBody, bool useRandomTilt, SystemGenerationOptions genOptions);


		// TODO write summary
		// TODO parameter for number of iterations? does it matter?
		/// <summary>
		/// 
		/// </summary>
		/// <param name="planet"></param>
		protected abstract void AdjustSurfaceTemperatures(double surfpres);
		

		public void RecalculateGases(ChemType[] gasTable)
		{
			Atmosphere.RecalculateGases(gasTable);
		}

		public bool Equals(SatelliteBody other)
		{
			return Position == other.Position &&
				Utilities.AlmostEqual(SemiMajorAxisAU, other.SemiMajorAxisAU) &&
				Utilities.AlmostEqual(Eccentricity, other.Eccentricity) &&
				Utilities.AlmostEqual(AxialTilt, other.AxialTilt) &&
				OrbitZone == other.OrbitZone &&
				Utilities.AlmostEqual(OrbitalPeriod, other.OrbitalPeriod) &&
				Utilities.AlmostEqual(DayLength, other.DayLength) &&
				Utilities.AlmostEqual(HillSphere, other.HillSphere) &&
				Utilities.AlmostEqual(MassSM, other.MassSM) &&
				Utilities.AlmostEqual(DustMassSM, other.DustMassSM) &&
				Utilities.AlmostEqual(GasMassSM, other.GasMassSM) &&
				Utilities.AlmostEqual(EscapeVelocityCMSec, other.EscapeVelocityCMSec) &&
				Utilities.AlmostEqual(SurfaceAccelerationCMSec2, other.SurfaceAccelerationCMSec2) &&
				Utilities.AlmostEqual(SurfaceGravityG, other.SurfaceGravityG) &&
				Utilities.AlmostEqual(CoreRadius, other.CoreRadius) &&
				Utilities.AlmostEqual(Radius, other.Radius) &&
				Utilities.AlmostEqual(DensityGCC, other.DensityGCC) &&
				Satellites.Count() == other.Satellites.Count() &&
				Utilities.AlmostEqual(RMSVelocityCMSec, other.RMSVelocityCMSec) &&
				Utilities.AlmostEqual(MolecularWeightRetained, other.MolecularWeightRetained) &&
				Utilities.AlmostEqual(VolatileGasInventory, other.VolatileGasInventory) &&
				Utilities.AlmostEqual(BoilingPointWater, other.BoilingPointWater) &&
				Utilities.AlmostEqual(Albedo, other.Albedo) &&
				Utilities.AlmostEqual(Illumination, other.Illumination) &&
				Utilities.AlmostEqual(ExosphereTemperature, other.ExosphereTemperature) &&
				Utilities.AlmostEqual(SurfaceTemperature, other.SurfaceTemperature) &&
				Utilities.AlmostEqual(GreenhouseRiseTemperature, other.GreenhouseRiseTemperature) &&
				Utilities.AlmostEqual(DaytimeTemperature, other.DaytimeTemperature) &&
				Utilities.AlmostEqual(NighttimeTemperature, other.NighttimeTemperature) &&
				Utilities.AlmostEqual(MaxTemperature, other.MaxTemperature) &&
				Utilities.AlmostEqual(MinTemperature, other.MinTemperature) &&
				Utilities.AlmostEqual(WaterCoverFraction, other.WaterCoverFraction) &&
				Utilities.AlmostEqual(CloudCoverFraction, other.CloudCoverFraction) &&
				Utilities.AlmostEqual(IceCoverFraction, other.IceCoverFraction);
		}
	}
}
