using System;
using System.Linq;
using System.Collections.Generic;
using Primoris.Universe.Stargen.Astrophysics;
using Primoris.Universe.Stargen.Systems;
using Primoris.Universe.Stargen.Services;
using Primoris.Universe.Stargen.Astrophysics.Burrows;
using UnitsNet;


namespace Primoris.Universe.Stargen.Bodies
{

    public delegate SatelliteBody CreateSatelliteBodyDelegate(Seed seed,
                                                            StellarBody star,
															int pos,
                                                            string planetID);


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

		protected Seed Seed { get; set; }

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

		public override Mass Mass
		{
			get
			{
				if (!IsForming)
					return ConsolidateMass(Core.Union(Atmosphere));
				else
					return base.Mass;
			}
			protected set
			{
				base.Mass = value;
			}
		}

        /// <summary>
        /// The gravitational acceleration felt at the surface of the planet. Given as a fraction of Earth gravity (Gs).
        /// </summary>
        public Acceleration SurfaceAcceleration { get; protected set; }

		/// <summary>
		/// The radius of the planet's core in km.
		/// </summary>
		public Length CoreRadius { get; protected set; }

		/// <summary>
		/// Mean overall Density for all of the SatelliteBody solid layers.
		/// </summary>
		public Density Density { get; protected set; }

		public LayerStack Layers { get; protected set; }

		public IEnumerable<Layer> Core { get => from l in Layers where l is SolidLayer select l; }

		public IEnumerable<(Chemical, Ratio)> CoreComposition => ConsolidateComposition(Core);

		#endregion

		#region Planet properties

		public BodyType Type { get; protected set; }

		public bool IsForming { get; protected set; } = true;

		public bool IsGasGiant => Type == BodyType.GasGiant ||
								  Type == BodyType.SubGasGiant ||
								  Type == BodyType.SubSubGasGiant;

		public bool IsTidallyLocked { get; protected set; }

		public bool IsEarthlike => Science.Planetology.TestIsEarthLike(Temperature,
																 WaterCoverFraction,
																 CloudCoverFraction,
																 IceCoverFraction,
																 SurfacePressure,
																 SurfaceAcceleration,
																 Breathability,
																 Type);

		public bool IsHabitable => Science.Planetology.TestIsHabitable(DayLength, OrbitalPeriod, Breathability, HasResonantPeriod, IsTidallyLocked);

		public bool HasResonantPeriod { get; protected set; }

		public bool HasGreenhouseEffect { get; protected set; }

        #endregion


        #region Atmospheric data

        public IEnumerable<Layer> Atmosphere { get => from l in Layers where l is GaseousLayer select l; }

		/// <summary>
		/// TODO: Create Unit Test.
		/// </summary>
		public IEnumerable<(Chemical, Ratio)> AtmosphereComposition => ConsolidateComposition(Atmosphere);

		public IEnumerable<(Chemical, Ratio)> AtmospherePoisonousComposition => ConsolidatePoisonousComposition(from GaseousLayer l in Atmosphere where l.PoisonousComposition.Count() > 0 select l);

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

		public Pressure SurfacePressure { get; protected set; }

		public Breathability Breathability => (from l in Layers where l is GaseousLayer select (l as GaseousLayer).Breathability).FirstOrDefault();

		#endregion

		#region Temperature data
		/// <summary>
		/// Illumination received by the body at at the farthest point of its
		/// orbit. 1.0 is the amount of illumination received by an object 1 au
		/// from the Sun.
		/// </summary>
		public Ratio Illumination => Science.Astronomy.GetMinimumIllumination(SemiMajorAxis, StellarBody.Luminosity);

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


		public SatelliteBody(Seed seed, StellarBody star, Body parentBody) : this(null, seed, star, parentBody) { }

		public SatelliteBody(IScienceAstrophysics phy, Seed seed, StellarBody star, Body parentBody)
		{
			Science = phy;

			StellarBody = star;
			Parent = parentBody;

			Seed = seed;

			Mass = seed.Mass;
			GasMass = seed.GasMass;
			DustMass = seed.DustMass;
			Eccentricity = seed.Eccentricity;
			SemiMajorAxis = seed.SemiMajorAxis;

			Layers = new LayerStack(this);
		}

		public SatelliteBody(Seed seed, StellarBody star, Body parentBody, IEnumerable<Layer> layers) : this(null, seed, star, parentBody, layers) { }
		public SatelliteBody(IScienceAstrophysics phy, Seed seed, StellarBody star, Body parentBody, IEnumerable<Layer> layers) : this(seed, star, parentBody)
		{
			Science = phy;

			Layers.Clear();
			Layers.AddMany(layers);
		}

		private IEnumerable<(Chemical, Ratio)> ConsolidateComposition(IEnumerable<Layer> layers)
		{
			var totmass = Mass.FromSolarMasses((from l in layers select l.Mass.SolarMasses).Sum(x => x));
			if (totmass == Mass.Zero)
				return new (Chemical, Ratio)[0];

			var amounts = (from l in layers
						   select l.Composition.Select(
										x =>
										{
											var chem = x.Item1;
											var ratio = Ratio.FromDecimalFractions(Mass.FromSolarMasses(x.Item2.DecimalFractions * l.Mass.SolarMasses) / totmass);

											return (chem, ratio);
										})).SelectMany(x => x);

			var grouped = from a in amounts
						  group a by a.Item1 into g
						  select (g.First().Item1, Ratio.FromDecimalFractions(g.Sum(x => x.Item2.DecimalFractions)));
			return grouped;
		}

		private IEnumerable<(Chemical, Ratio)> ConsolidatePoisonousComposition(IEnumerable<Layer> layers)
		{
			var totmass = Mass.FromSolarMasses((from l in layers select l.Mass.SolarMasses).Sum(x => x));
			var amounts = (from GaseousLayer l in layers
						   select l.PoisonousComposition.Select(
										x =>
										{
											var chem = x.Item1;
											var ratio = Ratio.FromDecimalFractions(Mass.FromSolarMasses(x.Item2.DecimalFractions * l.Mass.SolarMasses) / totmass);

											return (chem, ratio);
										})).SelectMany(x => x);

			var grouped = from a in amounts
						  group a by a.Item1 into g
						  select (g.First().Item1, Ratio.FromDecimalFractions(g.Sum(x => x.Item2.DecimalFractions)));
			return grouped;
		}

		private Mass ConsolidateMass(IEnumerable<Layer> layers)
		{
			var totmass = Mass.FromSolarMasses((from l in layers select l.Mass.SolarMasses).Sum(x => x));
			return totmass;
		}

		protected abstract void Generate();

		protected abstract IEnumerable<SatelliteBody> GenerateSatellites(Seed seed);
		
		protected void Evolve(Duration time)
		{
			if (IsForming)
				throw new InvalidBodyOperationException("Body is still in formation stage.");

			OnEvolve(time);
		}

		protected virtual void OnEvolve(Duration time) { }

		public bool Equals(SatelliteBody other)
		{
			return Position == other.Position &&
				Extensions.AlmostEqual(SemiMajorAxis.Value, other.SemiMajorAxis.Value) &&
				Extensions.AlmostEqual(Eccentricity.Value, other.Eccentricity.Value) &&
				Extensions.AlmostEqual(AxialTilt.Value, other.AxialTilt.Value) &&
				OrbitZone == other.OrbitZone &&
				Extensions.AlmostEqual(OrbitalPeriod.Value, other.OrbitalPeriod.Value) &&
				Extensions.AlmostEqual(DayLength.Value, other.DayLength.Value) &&
				Extensions.AlmostEqual(HillSphere.Value, other.HillSphere.Value) &&
				Extensions.AlmostEqual(Mass.Value, other.Mass.Value) &&
				Extensions.AlmostEqual(DustMass.Value, other.DustMass.Value) &&
				Extensions.AlmostEqual(GasMass.Value, other.GasMass.Value) &&
				Extensions.AlmostEqual(EscapeVelocity.CentimetersPerSecond, other.EscapeVelocity.CentimetersPerSecond) &&
				Extensions.AlmostEqual(SurfaceAcceleration.CentimetersPerSecondSquared, other.SurfaceAcceleration.CentimetersPerSecondSquared) &&
				Extensions.AlmostEqual(SurfaceAcceleration.StandardGravity, other.SurfaceAcceleration.StandardGravity) &&
				Extensions.AlmostEqual(CoreRadius.Kilometers, other.CoreRadius.Kilometers) &&
				Extensions.AlmostEqual(Radius.Kilometers, other.Radius.Kilometers) &&
				Extensions.AlmostEqual(Density.GramsPerCubicCentimeter, other.Density.GramsPerCubicCentimeter) &&
				Satellites.Count() == other.Satellites.Count() &&
				Extensions.AlmostEqual(RMSVelocity.CentimetersPerSecond, other.RMSVelocity.CentimetersPerSecond) &&
				Extensions.AlmostEqual(MolecularWeightRetained.Kilograms, other.MolecularWeightRetained.Kilograms) &&
				Extensions.AlmostEqual(VolatileGasInventory.Value, other.VolatileGasInventory.Value) &&
				Extensions.AlmostEqual(BoilingPointWater.Kelvins, other.BoilingPointWater.Kelvins) &&
				Extensions.AlmostEqual(Albedo.Value, other.Albedo.Value) &&
				Extensions.AlmostEqual(Illumination.Value, other.Illumination.Value) &&
				Extensions.AlmostEqual(ExosphereTemperature.Kelvins, other.ExosphereTemperature.Kelvins) &&
				Extensions.AlmostEqual(Temperature.Kelvins, other.Temperature.Kelvins) &&
				Extensions.AlmostEqual(GreenhouseRiseTemperature.Kelvins, other.GreenhouseRiseTemperature.Kelvins) &&
				Extensions.AlmostEqual(DaytimeTemperature.Kelvins, other.DaytimeTemperature.Kelvins) &&
				Extensions.AlmostEqual(NighttimeTemperature.Kelvins, other.NighttimeTemperature.Kelvins) &&
				Extensions.AlmostEqual(MaxTemperature.Kelvins, other.MaxTemperature.Kelvins) &&
				Extensions.AlmostEqual(MinTemperature.Kelvins, other.MinTemperature.Kelvins) &&
				Extensions.AlmostEqual(WaterCoverFraction.Value, other.WaterCoverFraction.Value) &&
				Extensions.AlmostEqual(CloudCoverFraction.Value, other.CloudCoverFraction.Value) &&
				Extensions.AlmostEqual(IceCoverFraction.Value, other.IceCoverFraction.Value);
		}
	}
}
