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
	/// <summary>
	/// Delegate used for body creation functions given a StellarBody and a Seed.
	/// </summary>
	/// <remarks>
	/// This delegate is referenced by the StellarBody class. It's used only to create satellites of StellarBody, not satellites of 
	/// SatelliteBody. 
	/// </remarks>
	/// <param name="seed">The seed.</param>
	/// <param name="star">The star.</param>
	/// <param name="pos">The position.</param>
	/// <param name="planetID">The planet identifier.</param>
	/// <returns>A SatelliteBody created.</returns>
	public delegate SatelliteBody CreateSatelliteBodyDelegate(Seed seed,
                                                            StellarBody star,
															int pos,
                                                            string planetID);

	/// <summary>
	/// A Body orbiting another Body. 
	/// </summary>
	/// <seealso cref="Primoris.Universe.Stargen.Bodies.Body" />
	/// <seealso cref="System.IEquatable{Primoris.Universe.Stargen.Bodies.SatelliteBody}" />
	[Serializable]
	public abstract class SatelliteBody : Body, IEquatable<SatelliteBody>
	{


		#region Orbit data

		/// <summary>
		/// Gets or sets the semi major axis.
		/// </summary>
		/// <value>
		/// The semi major axis. Default to one AU.
		/// </value>
		public Length SemiMajorAxis { get; protected set; } = Length.FromAstronomicalUnits(1.0);

		/// <summary>
		/// Eccentricity of the body's orbit.
		/// </summary>
		/// <see cref="http://astronomy.swin.edu.au/cosmos/O/Orbital+Eccentricity"/>
		/// <value>
		/// Body orbit eccentricity. Default to Zero.
		/// </value>
		public Ratio Eccentricity { get; protected set; } = Ratio.Zero;

		/// <summary>
		/// Axial tilt of the Body.
		/// </summary>
		/// <see cref="https://en.wikipedia.org/wiki/Axial_tilt"/>
		/// <value>
		/// Axial tilt. Default to Zero.
		/// </value>
		public Angle AxialTilt { get; protected set; } = Angle.Zero;

		/// <summary>
		/// Orbital zone the planet is located in. Value is 1, 2, or 3. 
		/// </summary>
		/// <remarks>
		/// Meaning of the orbital zone is implementation specific. In broad terms (1) is for the region before the habitable zone closer to the Star,
		/// (2) is the habitable zone and (3) is the region pass the habitable zone up to the end of the planetary system. 
		/// </remarks>
		/// <value>
		/// Orbital zone.
		/// </value>
		public int OrbitZone { get; protected set; } = 1;

		/// <summary>
		/// The length of the Body's year.
		/// </summary>
		/// <value>
		/// Duration of a year.
		/// </value>
		public Duration OrbitalPeriod { get; protected set; }

		/// <summary>
		/// Angular velocity about the planet's axis.
		/// </summary>
		/// <value>
		/// 
		/// Angular speed at which the Body rotate.</value>
		public RotationalSpeed AngularVelocity { get; protected set; }

		/// <summary>
		/// The length of the Body's day.
		/// </summary>
		/// <value>
		/// Duration to get a full revolution of the Body around its axis.
		/// </value>
		public Duration DayLength { get; protected set; }

		/// <summary>
		/// The Hill sphere of the Body.
		/// </summary>
		/// <value>
		/// Hill sphere length.
		/// </value>
		public Length HillSphere { get; protected set; }

		#endregion

		#region Size & mass data

		/// <summary>
		/// Gets or sets the seed that was used to created the Body.
		/// </summary>
		/// <value>
		/// The seed.
		/// </value>
		protected Seed Seed { get; set; }

		/// <summary>
		/// The mass of dust retained by the planet (ie, the mass of the planet
		/// without atmosphere).
		/// </summary>
		/// <value>
		/// Dust mass.
		/// </value>
		public Mass DustMass { get; protected set; }

		/// <summary>
		/// The mass of gas retained by the planet (ie, the mass of its
		/// atmosphere). Given in units of Solar mass.
		/// </summary>
		/// <value>
		/// Gas mass.
		/// </value>
		public Mass GasMass { get; protected set; }

		/// <summary>
		/// Gets or sets the mass.
		/// </summary>
		/// <value>
		/// The mass.
		/// </value>
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
        /// The gravitational acceleration felt at the surface of the planet.
        /// </summary>
		/// <value>
		/// Acceleration felt at the Body surface.
		/// </value>
        public Acceleration SurfaceAcceleration { get; protected set; }

		Length _initialRadius = default;
		public override Length Radius
		{
			get
			{
				if (Layers.Count == 0)
					return _initialRadius;

				Length radius = Length.Zero;
				foreach (var l in Layers)
				{
					radius += l.Thickness;
				}
				return radius;
			}

			protected set
			{
				if (Layers.Count == 0)
					_initialRadius = value;
			}
		}

		// TODO: Integrates it into layers.
		/// <summary>
		/// The radius of the planet's core in km.
		/// </summary>
		public Length CoreRadius 
		{ 
			get
			{
				Length radius = Length.Zero;
				foreach (var l in Layers)
				{
					if(l is SolidLayer)
						radius += l.Thickness;
				}
				return radius;
			} 
		}

		// TODO Integrates it into layers.
		/// <summary>
		/// Mean overall Density for all of the SatelliteBody solid layers.
		/// </summary>
		public Density Density { get; protected set; }

		/// <summary>
		/// Gets or sets the layers.
		/// </summary>
		/// <value>
		/// The layers.
		/// </value>
		public LayerStack Layers { get; protected set; }

		/// <summary>
		/// Gets the core layers.
		/// </summary>
		/// <value>
		/// The core layers IEnumerable.
		/// </value>
		public IEnumerable<Layer> Core { get => from l in Layers where l is SolidLayer select l; }

		/// <summary>
		/// Gets the core composition.
		/// </summary>
		/// <value>
		/// The core composition IEnumerable of (Chemical, Ratio) tuple.
		/// </value>
		public IEnumerable<(Chemical, Ratio)> CoreComposition => ConsolidateComposition(Core);

		#endregion

		#region Planet properties

		/// <summary>
		/// Gets or sets the parent.
		/// </summary>
		/// <remarks>
		/// For a planet this would be a StellarBody. For a Satellite this would be a planet.
		/// </remarks>
		/// <value>
		/// The parent.
		/// </value>
		public Body Parent { get; protected set; }

		/// <summary>
		/// Gets or sets the stellar body.
		/// </summary>
		/// <remarks>
		/// The StellarBody is the body at the center of the system.
		/// </remarks>
		/// <value>
		/// The stellar body.
		/// </value>
		public StellarBody StellarBody { get; protected set; }

		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>
		/// The type.
		/// </value>
		public BodyType Type { get; protected set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is forming.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is forming; otherwise, <c>false</c>.
		/// </value>
		public bool IsForming { get; protected set; } = true;

		/// <summary>
		/// Gets a value indicating whether this instance is gas giant.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is gas giant; otherwise, <c>false</c>.
		/// </value>
		public bool IsGasGiant => Type == BodyType.GasGiant ||
								  Type == BodyType.SubGasGiant ||
								  Type == BodyType.SubSubGasGiant;

		/// <summary>
		/// Gets or sets a value indicating whether this instance is tidally locked.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is tidally locked; otherwise, <c>false</c>.
		/// </value>
		public bool IsTidallyLocked { get; protected set; }

		public bool IsEarthlike => Science.Planetology.TestIsEarthLike(Temperature,
																 WaterCoverFraction,
																 CloudCoverFraction,
																 IceCoverFraction,
																 SurfacePressure,
																 SurfaceAcceleration,
																 Breathability,
																 Type);

		/// <summary>
		/// Gets a value indicating whether this instance is habitable.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is habitable; otherwise, <c>false</c>.
		/// </value>
		public bool IsHabitable => Science.Planetology.TestIsHabitable(DayLength, OrbitalPeriod, Breathability, HasResonantPeriod, IsTidallyLocked);

		/// <summary>
		/// Gets or sets a value indicating whether this instance has resonant period.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance has resonant period; otherwise, <c>false</c>.
		/// </value>
		public bool HasResonantPeriod { get; protected set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance has greenhouse effect.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance has greenhouse effect; otherwise, <c>false</c>.
		/// </value>
		public bool HasGreenhouseEffect { get; protected set; }

		#endregion


		#region Atmospheric data

		/// <summary>
		/// Gets the atmosphere layers.
		/// </summary>
		/// <value>
		/// The atmosphere layers IEnumerable.
		/// </value>
		public IEnumerable<Layer> Atmosphere { get => from l in Layers where l is GaseousLayer select l; }

		// TODO: Create Unit Tests.		
		/// <summary>
		/// Gets the atmosphere composition.
		/// </summary>
		/// <value>
		/// The atmosphere composition IEnumerable of (Chemical, Ratio) tuple.
		/// </value>
		public IEnumerable<(Chemical, Ratio)> AtmosphereComposition => ConsolidateComposition(Atmosphere);

		/// <summary>
		/// Gets the atmosphere poisonous composition.
		/// </summary>
		/// <value>
		/// The atmosphere poisonous composition IEnumerable of (Chemical, Ratio) tuple.
		/// </value>
		public IEnumerable<(Chemical, Ratio)> AtmospherePoisonousComposition => ConsolidatePoisonousComposition(from GaseousLayer l in Atmosphere where l.PoisonousComposition.Count() > 0 select l);

		/// <summary>
		/// The root-mean-square velocity of N2 at the planet's exosphere. 
		/// </summary>
		/// <remarks>
		/// Used to determine where or not a planet is capable of
		/// retaining an atmosphere.
		/// </remarks>
		public Speed RMSVelocity { get; protected set; }

		/// <summary>
		/// The smallest molecular weight the planet is capable of retaining.
		/// </summary>
		public Mass MolecularWeightRetained { get; protected set; }

		/// <summary>
		/// Unitless value for the inventory of volatile gases that result from
		/// outgassing. 
		/// </summary>
		/// <remarks>
		/// Used in the calculation of surface pressure. See Fogg
		/// eq. 16. 
		/// </remarks>
		public Ratio VolatileGasInventory { get; protected set; }

		/// <summary>
		/// Boiling point of water on the Body.
		/// </summary>
		/// <remarks>
		/// Body without atmosphere returns 0.
		/// </remarks>
		public Temperature BoilingPointWater { get; protected set; }

		/// <summary>
		/// Planetary albedo. Unitless value between 0 (no reflection) and 1 
		/// (completely reflective).
		/// </summary>
		public Ratio Albedo { get; protected set; }

		// TODO: Insert in layers system.		
		/// <summary>
		/// Gets or sets the surface pressure.
		/// </summary>
		/// <remarks>
		/// Body without Atmosphere returns Zero.
		/// </remarks>
		/// <value>
		/// The pressure felt at the surface of the Body.
		/// </value>
		public Pressure SurfacePressure { get; protected set; }

		/// <summary>
		/// Gets the breathability.
		/// </summary>
		/// <value>
		/// The breathability of the Body.
		/// </value>
		public Breathability Breathability => (from l in Layers where l is GaseousLayer select ((GaseousLayer)l).Breathability).FirstOrDefault();

		#endregion

		#region Temperature data
		/// <summary>
		/// Illumination received by the body at at the farthest point of its
		/// orbit. 1.0 is the amount of illumination received by an object 1 au
		/// from the Sun.
		/// </summary>
		/// <value>
		/// Illumination Ratio.
		/// </value>
		public Ratio Illumination => Science.Astronomy.GetMinimumIllumination(SemiMajorAxis, StellarBody.Luminosity);

		/// <summary>
		/// Temperature at the body's exosphere.
		/// </summary>
		/// <value>
		/// Exosphere Temperature.
		/// </value>
		public Temperature ExosphereTemperature { get; protected set; }

		/// <summary>
		/// Temperature at the body's surface given in Kelvin.
		/// </summary>
		//public override Temperature Temperature { get; protected set; }

		/// <summary>
		/// Amount that the planet's surface temperature is being increased by a runaway greenhouse effect.
		/// </summary>
		/// <value>
		/// TemperatureDelta caused by Greenhouse Effect.
		/// </value>
		public TemperatureDelta GreenhouseRiseTemperature { get; protected set; }

		/// <summary>
		/// Average daytime temperature.
		/// </summary>
		/// <value>
		/// Average daytime Temperature.
		/// </value>
		public Temperature DaytimeTemperature { get; protected set; }

		/// <summary>
		/// Average nighttime temperature.
		/// </summary>
		/// <value>
		/// Average night time Temperature.
		/// </value>
		public Temperature NighttimeTemperature { get; protected set; }

		/// <summary>
		/// Maximum (summer/day) temperature.
		/// </summary>
		/// <value>
		/// Maximum Temperature encountered at the surface.
		/// </value>
		public Temperature MaxTemperature { get; protected set; }

		/// <summary>
		/// Minimum (winter/night) temperature.
		/// </summary>
		/// <value>
		/// Minimum Temperature encountered at the surface.
		/// </value>
		public Temperature MinTemperature { get; protected set; }

		#endregion

		#region Surface coverage

		/// <summary>
		/// Amount of the body's surface that is covered in water. Given as a
		/// value between 0 (no water) and 1 (completely covered).
		/// </summary>
		/// <value>
		/// Ratio of the surface covered by water.
		/// </value>
		public Ratio WaterCoverFraction { get; protected set; }

		/// <summary>
		/// Amount of the body's surface that is obscured by cloud cover. Given
		/// as a value between 0 (no cloud coverage) and 1 (surface not visible
		/// at all).
		/// </summary>
		/// <value>
		/// Ratio of the Atmosphere covered by clouds.
		/// </value>
		public Ratio CloudCoverFraction { get; protected set; }

		/// <summary>
		/// Amount of the body's surface that is covered in ice. Given as a 
		/// value between 0 (no ice) and 1 (completely covered).
		/// </summary>
		/// <value>
		/// Ratio of the surface covered by ice.
		/// </value>
		public Ratio IceCoverFraction { get; protected set; }

        #endregion

		/// <summary>
		/// Construct a new SatelliteBody.
		/// </summary>
		/// <param name="seed">Source Seed to create the Body.</param>
		/// <param name="star">Parent Star of the Body.</param>
		/// <param name="parentBody">Parent Body of constructed SatelliteBody. If the constructed Body is a Planet, then this is the same as Star.</param>
		public SatelliteBody(Seed seed, StellarBody star, Body parentBody)  
		{
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="phy">Science interface to use to construct the Body.</param>
		/// <param name="seed">Source Seed to create the Body.</param>
		/// <param name="star">Parent Star of the Body.</param>
		/// <param name="parentBody">Parent Body of constructed SatelliteBody. If the constructed Body is a Planet, then this is the same as Star.</param>
		public SatelliteBody(IScienceAstrophysics phy, Seed seed, StellarBody star, Body parentBody) : this(seed, star, parentBody)
		{
			Science = phy;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="seed">Source Seed to create the Body.</param>
		/// <param name="star">Parent Star of the Body.</param>
		/// <param name="parentBody">Parent Body of constructed SatelliteBody. If the constructed Body is a Planet, then this is the same as Star.</param>
		/// <param name="layers">Layers to construct the Body with.</param>
		public SatelliteBody(Seed seed, StellarBody star, Body parentBody, IEnumerable<Layer> layers) : this(seed, star, parentBody) 
		{
			Layers.Clear();
			Layers.AddMany(layers);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="phy">Science interface to use to construct the Body.</param>
		/// <param name="seed">Source Seed to create the Body.</param>
		/// <param name="star">Parent Star of the Body.</param>
		/// <param name="parentBody">Parent Body of constructed SatelliteBody. If the constructed Body is a Planet, then this is the same as Star.</param>
		/// <param name="layers">Layers to construct the Body with.</param>
		public SatelliteBody(IScienceAstrophysics phy, Seed seed, StellarBody star, Body parentBody, IEnumerable<Layer> layers) : this(seed, star, parentBody, layers)
		{
			Science = phy;
		}

		private IEnumerable<(Chemical, Ratio)> ConsolidateComposition(IEnumerable<Layer> layers)
		{
			var totmass = Mass.FromSolarMasses((from l in layers select l.Mass.SolarMasses).Sum(x => x));
			if (totmass.Equals(Mass.Zero, Extensions.Epsilon, ComparisonType.Relative))
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

		/// <summary>
		/// Generate the Body given known parameters at construction.
		/// </summary>
		protected abstract void Generate();

		/// <summary>
		/// Generate Body satellites given a start Seed.
		/// </summary>
		/// <param name="seed">Starting Seed of the satellites of the current Body.</param>
		/// <returns></returns>
		protected abstract IEnumerable<SatelliteBody> GenerateSatellites(Seed seed);
		
		/// <summary>
		/// Change characteristics of the current Body given time elapsed.
		/// </summary>
		/// <param name="time"></param>
		protected void Evolve(Duration time)
		{
			if (IsForming)
				throw new InvalidBodyOperationException("Body is still in formation stage.");

			OnEvolve(time);
		}

		/// <summary>
		/// Executed in a derived class when <see cref="Evolve(Duration)"/> is called.
		/// </summary>
		/// <param name="time"></param>
		protected virtual void OnEvolve(Duration time) { }

		/// <summary>
		/// Check to see if a SatelliteBody is equals to the current one.
		/// </summary>
		/// <param name="other">Other Body to compare this with.</param>
		/// <returns>True if both Bodies are equals, false otherwise.</returns>
		public bool Equals(SatelliteBody? other)
		{
			if (other is null)
				return false;

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
