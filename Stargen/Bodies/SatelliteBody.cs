using Primoris.Universe.Stargen.Astrophysics;
using Primoris.Universe.Stargen.Astrophysics.Singularity;
using Primoris.Universe.Stargen.Bodies.Burrows;

using UnitsNet;

namespace Primoris.Universe.Stargen.Bodies;


/// <summary>
/// A Body orbiting another Body. 
/// </summary>
/// <remarks>
/// Properties that need to be sety in a subclass:
/// * Albedo
/// * WaterCoverFraction
/// * IceCoverFraction
/// * CloudCoverFraction
/// * MolecularWeightRetained
/// * Temperature
/// * AxialTilt
/// 
/// Properties to be set at the begining of formation.
/// * Radius (Total SatelliteBody radius to be used while IsForming == true).
/// * AngularVelocity (value to be used while IsForming == true).
/// 
/// Properties that can't be get during Formation. These properties will throw.
/// * Core
/// * 
/// 
/// </remarks>
/// <seealso cref="Primoris.Universe.Stargen.Bodies.Body" />
/// <seealso cref="System.IEquatable{Primoris.Universe.Stargen.Bodies.SatelliteBody}" />
[Serializable]
public abstract class SatelliteBody : Body, IEquatable<SatelliteBody>
{
    public static readonly new SatelliteBody Null = new EmptySatelliteBody();


    #region Orbit data

    /// <summary>
    /// Gets or sets the semi major axis.
    /// </summary>
    /// <value>
    /// The semi major axis. Default to one AU.
    /// </value>
    public Length SemiMajorAxis => Seed.SemiMajorAxis;

    /// <summary>
    /// Eccentricity of the body's orbit.
    /// </summary>
    /// <see cref="http://astronomy.swin.edu.au/cosmos/O/Orbital+Eccentricity"/>
    /// <value>
    /// Body orbit eccentricity. Default to Zero.
    /// </value>
    public Ratio Eccentricity => Seed.Eccentricity;

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
    public int OrbitZone => Science.Astronomy.GetOrbitalZone(StellarBody.Luminosity, SemiMajorAxis);

    /// <summary>
    /// The length of the Body's year.
    /// </summary>
    /// <value>
    /// Duration of a year.
    /// </value>
    public Duration OrbitalPeriod => Science.Astronomy.GetPeriod(SemiMajorAxis, DustMass + GasMass, Parent.Mass);

    /// <summary>
    /// Angular velocity about the planet's axis.
    /// </summary>
    /// <value>
    /// Angular speed at which the Body rotate.
    /// </value>
    public override RotationalSpeed AngularVelocity
    {
        get
        {
            if (!base.AngularVelocity.Equals(RotationalSpeed.Zero, 1e-6, ComparisonType.Relative))
                return base.AngularVelocity;

            return Science.Dynamics.GetAngularVelocity(Mass,
                                            Radius,
                                            Density,
                                            SemiMajorAxis,
                                            Science.Planetology.TestIsGasGiant(Mass, GasMass, MolecularWeightRetained),
                                            StellarBody.Mass,
                                            StellarBody.Age);
        }

        protected set
        {
            base.AngularVelocity = value;
        }
    }


    /// <summary>
    /// The length of the Body's day.
    /// </summary>
    /// <value>
    /// Duration to get a full revolution of the Body around its axis.
    /// </value>
    public Duration DayLength => Science.Astronomy.GetDayLength(AngularVelocity, OrbitalPeriod, Eccentricity);

    /// <summary>
    /// The Hill sphere of the Body.
    /// </summary>
    /// <value>
    /// Hill sphere length.
    /// </value>
    public Length HillSphere => Science.Astronomy.GetHillSphere(StellarBody.Mass, Mass, SemiMajorAxis);

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
    public Mass DustMass => Seed.DustMass;

    /// <summary>
    /// The mass of gas retained by the planet (ie, the mass of its
    /// atmosphere). Given in units of Solar mass.
    /// </summary>
    /// <value>
    /// Gas mass.
    /// </value>
    public Mass GasMass => Seed.GasMass;


    /// <summary>
    /// Gets or sets the total mass.
    /// </summary>
    /// <remarks>
    /// This property returns DustMass + GasMass when IsForming == true;
    /// </remarks>
    /// <value>
    /// The total mass of this SatelliteBody.
    /// </value>
    public override Mass Mass => Stack.Count > 0 && !IsForming ? Mass.FromEarthMasses((from l in Stack select l.Mass.EarthMasses).Sum()) : DustMass + GasMass;

    /// <summary>
    /// The gravitational acceleration felt at the surface of the planet.
    /// </summary>
    /// <remarks>
    /// For an all gas planet the gravitational acceleration felt at the surface, the center of the Body, is Zero.
    /// </remarks>
    /// <value>
    /// Acceleration felt at the Body surface.
    /// </value>
    public Acceleration SurfaceAcceleration => Science.Physics.GetAcceleration(CoreMass, CoreRadius);

    /// <summary>
    /// Total radius of the SatelliteBody.
    /// </summary>
    /// <remarks>
    /// This property returns hte value given at the begining of formation is IsForming == true. Otherwise
    /// returns the compute thinkness of all Layers.
    /// </remarks>
    public override Length Radius
    {
        get
        {
            if (Stack.Count == 0 || IsForming)
                return base.Radius;

            Length radius = Length.Zero;
            foreach (var l in Stack)
            {
                radius += l.Thickness;
            }
            return radius;
        }

        protected set
        {
            if (Stack.Count == 0 || IsForming)
                base.Radius = value;
        }
    }

    /// <summary>
    /// The radius of the planet's core in km.
    /// </summary>
    public Length CoreRadius
    {
        get
        {
            if (IsForming)
                return Science.Planetology.GetCoreRadius(Mass, OrbitZone, Science.Planetology.TestIsGasGiant(Mass, GasMass, MolecularWeightRetained));

            Length radius = default;
            foreach (var l in Stack)
            {
                if (l is SolidLayer)
                    radius += l.Thickness;
            }
            return radius;
        }
    }

    /// <summary>
    /// Mean overall Density for all of the SatelliteBody solid layers.
    /// </summary>
    public Density Density => Science.Physics.GetDensityFromBody(Mass, Radius);

    /// <summary>
    /// Gets or sets the layers.
    /// </summary>
    /// <value>
    /// The layers.
    /// </value>
    protected LayerStack Stack { get; set; }

    /// <summary>
    /// Get all the layers of this SatelliteBody.
    /// </summary>
    public IEnumerable<Layer> Layers => Stack;

    /// <summary>
    /// Gets the core layers.
    /// </summary>
    /// <value>
    /// The core layers IEnumerable.
    /// </value>
    public IEnumerable<Layer> Core => from l in Stack where l is SolidLayer select l;

    /// <summary>
    /// Get the mass of the SatelliteBody core.
    /// </summary>
    public Mass CoreMass
    {
        get
        {
            if (IsForming)
                return Seed.DustMass;

            return Mass.FromEarthMasses((from l in Stack where l is SolidLayer select l.Mass.EarthMasses).Sum());
        }
    }

    /// <summary>
    /// Gets the core composition.
    /// </summary>
    /// <value>
    /// The core composition IEnumerable of (Chemical, Ratio) tuple.
    /// </value>
    public IEnumerable<(Chemical, Ratio)> CoreComposition => ConsolidateComposition(Core);

    #endregion

    #region Planet properties



    public bool IsForming { get; private set; } = true;

    /// <summary>
    /// Gets or sets the stellar body.
    /// </summary>
    /// <remarks>
    /// The StellarBody is the body at the center of the system.
    /// </remarks>
    /// <value>
    /// The stellar body.
    /// </value>
    public StellarBody StellarBody
    {
        get
        {
            switch (Parent)
            {
                case StellarBody stellarBody: return stellarBody;
                case SatelliteBody satelliteBody: return satelliteBody.StellarBody;
                default: throw new InvalidBodyException("Unknown Body type encountered.");
            }
        }
    }

    /// <summary>
    /// Gets or sets the type.
    /// </summary>
    /// <value>
    /// The type.
    /// </value>
    public BodyType Type => Science.Planetology.GetBodyType(Mass,
                                     GasMass,
                                     MolecularWeightRetained,
                                     SurfacePressure,
                                     WaterCoverFraction,
                                     IceCoverFraction,
                                     MaxTemperature,
                                     BoilingPointWater,
                                     Temperature);

    /// <summary>
    /// Returns true if there is at least one SolidLayer in this SatelliteBody.
    /// </summary>
    /// <value>
    ///     <c>true</c> if there's at least one solid layer; otherwise, <c>false</c>.
    /// </value>
    public bool HasRockyBody => (from l in Stack select l is SolidLayer).Any(x => x == true);

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
    public bool IsTidallyLocked => Science.Planetology.TestIsTidallyLocked(DayLength, OrbitalPeriod);

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
    public bool HasResonantPeriod => Science.Planetology.TestHasResonantPeriod(AngularVelocity, DayLength, OrbitalPeriod, Eccentricity);

    /// <summary>
    /// TODO: Broken. For Barren, for example, it gives a greenhouse effect.
    /// Gets or sets a value indicating whether this instance has greenhouse effect.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance has greenhouse effect; otherwise, <c>false</c>.
    /// </value>
    public bool HasGreenhouseEffect => Science.Planetology.TestHasGreenhouseEffect(StellarBody.EcosphereRadius, SemiMajorAxis);

    #endregion


    #region Atmospheric data

    /// <summary>
    /// Gets the atmosphere layers.
    /// </summary>
    /// <value>
    /// The atmosphere layers IEnumerable.
    /// </value>
    public IEnumerable<Layer> Atmosphere { get => from l in Stack where l is GaseousLayer select l; }

    public Mass AtmosphereMass => Mass.FromEarthMasses((from l in Stack where l is GaseousLayer select l.Mass.EarthMasses).Sum());


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
    /// Used to determine whether or not a planet is capable of retaining an atmosphere.
    /// </remarks>
    public Speed RMSVelocity => Science.Physics.GetRMSVelocity(Mass.FromGrams(GlobalConstants.MOL_NITROGEN), ExosphereTemperature);

    Mass _molecularWeightRetained = Mass.Zero;
    /// <summary>
    /// The smallest molecular weight the planet is capable of retaining.
    /// </summary>
    /// <remarks>
    /// Fundamental property that needs to be set in a subclass.
    /// </remarks>
    public Mass MolecularWeightRetained 
    { 
        get
        {
            if (_molecularWeightRetained <= Mass.Zero)
                throw new InvalidBodyOperationException("MolecularWeightRetained is used before being set.");

            return _molecularWeightRetained;
        }
        
        protected set
        {
            _molecularWeightRetained = value;
        }
    }


    /// <summary>
    /// Unitless value for the inventory of volatile gases that result from
    /// outgassing. 
    /// </summary>
    /// <remarks>
    /// Used in the calculation of surface pressure. See Fogg
    /// eq. 16. 
    /// </remarks>
    public Ratio VolatileGasInventory => Science.Physics.GetVolatileGasInventory(GasMass + DustMass,
                                                              EscapeVelocity,
                                                              RMSVelocity,
                                                              StellarBody.Mass,
                                                              GasMass,
                                                              OrbitZone,
                                                              HasGreenhouseEffect);

    /// <summary>
    /// Boiling point of water on the Body.
    /// </summary>
    /// <remarks>
    /// Body without atmosphere returns 0.
    /// Fundamental proeprty that needs to be set in a subclass.
    /// </remarks>
    public Temperature BoilingPointWater => Science.Thermodynamics.GetBoilingPointWater(SurfacePressure);

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
    /// TODO: GasGiant should return the pressure at the center of the body.
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
    public Breathability Breathability => (from l in Stack where l is GaseousLayer select ((GaseousLayer)l).Breathability).FirstOrDefault();

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
    /// <remarks>
    /// This property assuem that the SatelliteBody orbit a StellarBody. It needs to be overloaded
    /// for bodies that do not orbit the StellarBody of the system but somethign else. 
    /// </remarks>
    /// <value>
    /// Exosphere Temperature.
    /// </value>
    public virtual Temperature ExosphereTemperature => Science.Thermodynamics.GetEstimatedExosphereTemperature(SemiMajorAxis, StellarBody.EcosphereRadius, StellarBody.Temperature);

    /// <summary>
    /// Amount that the planet's surface temperature is being increased by a runaway greenhouse effect.
    /// </summary>
    /// <value>
    /// TemperatureDelta caused by Greenhouse Effect.
    /// </value>
    public TemperatureDelta GreenhouseRiseTemperature 
    { 
        get
        {
            var initTemp = Science.Thermodynamics.GetEstimatedAverageTemperature(StellarBody.EcosphereRadius, SemiMajorAxis, Albedo);
            var delta = Temperature - initTemp;

            if (delta.Kelvins > 0)
                return delta;
            else
                return TemperatureDelta.Zero;
        } 
    }

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
    /// Constructor used for creating special cases subclassed like an empty SatelliteBody.
    /// </summary>
    /// <param name="science">Astrophysics model to use.</param>
    internal SatelliteBody(IScienceAstrophysics science) : base(science)
    {
        Seed = new Seed();
        Parent = this;
        Stack = new LayerStack(this);
    }

    /// <summary>
    /// Construct a new SatelliteBody given an Astrophysics model, a Seed and a parent Body.
    /// </summary>
    /// <param name="seed">Source Seed to create the Body.</param>
    /// <param name="star">Parent Star of the Body.</param>
    /// <param name="parentBody">Parent Body of constructed SatelliteBody. If the constructed Body is a Planet, then this is the same as Star.</param>
    public SatelliteBody(IScienceAstrophysics science, Seed seed, Body parentBody) : base(science)
    {
        Parent = parentBody;
        Seed = seed with { };
        Stack = new LayerStack(this);
    }

    /// <summary>
    /// Construct a new SatelliteBody given a Seed and a parent Body.
    /// </summary>
    /// <param name="seed"></param>
    /// <param name="parentBody"></param>
    public SatelliteBody(Seed seed, Body parentBody) : this(parentBody.Science, seed, parentBody) { }

    /// <summary>
    /// Get the surface acceleration at the specified layer.
    /// </summary>
    /// <param name="layer">Layer to get acceleration at.</param>
    /// <returns>Acceleration at the inner area of the Layer.</returns>
    public Acceleration ComputeAccelerationAt(Layer layer)
    {
        return Stack.ComputeAccelerationAt(layer);
    }

    /// <summary>
    /// Compute the thickness below the specified Layer.
    /// </summary>
    /// <param name="layer">Layer to get thickness below.</param>
    /// <returns>Thickness below the specified Layer.</returns>
    public Length ComputeThicknessBelow(Layer layer)
    {
        return Stack.ComputeThicknessBelow(layer);
    }

    /// <summary>
    /// Compute the Mass below the specified Layer.
    /// </summary>
    /// <param name="layer">Layer to get Mass below.</param>
    /// <returns>Mass below the specified Layer.</returns>
    public Mass ComputeMassBelow(Layer layer)
    {
        return Stack.ComputeMassBelow(layer);
    }

    /// <summary>
    /// Create a new Layer.
    /// </summary>
    /// <remarks>
    /// The Layer class add the created Layer to the passed LayerStack automatically. So at the end of this method
    /// the Laye ris at the top of the stack.
    /// </remarks>
    /// <param name="layerCreator">Func that creates the Layer.</param>
    /// <exception cref="InvalidBodyException"></exception>
    public void CreateLayer(Action<LayerStack> layerCreator)
    {
        if (!IsForming)
            throw new InvalidBodyException("SatelliteBody is not in forming stage and so can't add layers.");

        layerCreator(Stack);
    }

    /// <summary>
    /// End the SatelliteBody formation period. 
    /// </summary>
    /// <remarks>
    /// This is an irreversible action.
    /// </remarks>
    public void EndForming()
    {
        if(OnEndForming())
            IsForming = false;
    }

    /// <summary>
    /// Called when EndForming() is called. 
    /// </summary>
    /// <remarks>
    /// The implementation in SatelliteBody always returns true. Can be overriden in
    /// a subclass.
    /// </remarks>
    /// <returns>True is forming can be ended. False otherwise.</returns>
    protected virtual bool OnEndForming()
    {
        return true;
    }

    /// <summary>
    /// Returns the Chemical ratios of an enumration of Layers.
    /// </summary>
    /// <param name="layers">Layer to get the composition.</param>
    /// <returns>An IEnumerable of a (Chemical, Ratio) Tuple.</returns>
    IEnumerable<(Chemical, Ratio)> ConsolidateComposition(IEnumerable<Layer> layers)
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

    /// <summary>
    /// Returns the Chemical ratios of the poisonous gas of an enumeration of GaseousLayer.
    /// </summary>
    /// <param name="layers">Layers to consolidate the composition.</param>
    /// <returns>An IEnumerable of a (Chemical, Ratio) Tuple.</returns>
    IEnumerable<(Chemical, Ratio)> ConsolidatePoisonousComposition(IEnumerable<Layer> layers)
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
    /// Check to see if a SatelliteBody is equals to the current one.
    /// </summary>
    /// <remarks>
    /// This is not a ReferenceEquals equivalent. It will check all the properties and returns
    /// true if and only if both have the same values given a certain tolerance.
    /// <seealso cref="Extensions.AlmostEqual(double, double, double)"/>
    /// </remarks>
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
            /*Extensions.AlmostEqual(VolatileGasInventory.Value, other.VolatileGasInventory.Value) &&*/
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

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return Equals(obj as SatelliteBody);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}


class EmptySatelliteBody : SatelliteBody
{
    public EmptySatelliteBody() : base(new SingularityPhysics())
    {
    }

    protected override void Generate()
    {
        return;
    }

    protected override IEnumerable<SatelliteBody> GenerateSatellites(Seed seed)
    {
        return Array.Empty<SatelliteBody>();
    }
}
