using System;
using System.Collections.Generic;
using Main = Primoris.Universe.Stargen;
using Primoris.Universe.Stargen.Systems;
using System.Drawing;
using UnitsNet;
using Primoris.Universe.Stargen.Astrophysics;
using Environment = Primoris.Universe.Stargen.Astrophysics.Environment;

namespace Primoris.Universe.Stargen.Bodies
{

    // UGLY Not comfortable with binary systems just having a second mass value

    [Serializable]
    public abstract class StellarBody : Body
    {
        public static readonly Duration MinSunAge = Duration.FromYears365(1.0E9);
        public static readonly Duration MaxSunAge = Duration.FromYears365(6.0E9);

        public StellarBody(IScienceAstrophysics phy) : this(phy, Mass.FromSolarMasses(Utilities.RandomNumber(0.7, 1.4))) { }

        public StellarBody(IScienceAstrophysics phy, Mass mass) : this(phy, mass, Luminosity.Zero, Duration.FromYears365(double.MaxValue)) { }

        public StellarBody(IScienceAstrophysics phy, Mass mass, Luminosity lum, Duration age)
        {
            Parent = null;
            Astro = phy;

            if (mass.SolarMasses < 0.2 || mass.SolarMasses > 1.5)
            {
                mass = Mass.FromSolarMasses(Utilities.RandomNumber(0.7, 1.4));
            }

            if (lum.SolarLuminosities == 0.0)
            {
                lum = Luminosity.FromSolarLuminosities(Environment.MassToLuminosity(mass));
            }

            StellarType = StellarType.FromLuminosityAndRadius(lum, Length.FromSolarRadiuses(1.0));

            //EcosphereRadiusAU = Math.Sqrt(lum);
            Life = Duration.FromYears365(1.0E10 * (Mass.SolarMasses / Luminosity.SolarLuminosities));

            if (age.Years365 == double.MaxValue)
                Age = Duration.FromYears365(Utilities.RandomNumber(MinSunAge.Years365, Life < MaxSunAge ? Life.Years365 : MaxSunAge.Years365));
            else
                Age = age;

            Mass = StellarType.Mass;
            Radius = StellarType.Radius;
            Luminosity = StellarType.Luminosity;
            Temperature = StellarType.Temperature;
            EscapeVelocity = Astro.Dynamics.GetEscapeVelocity(Mass, Radius);
        }

        public StellarBody(IScienceAstrophysics phy, StellarType st)
        {
            Parent = null;
            Astro = phy;
            StellarType = st;
            Life = Duration.FromYears365(1.0E10 * (st.Mass.SolarMasses / st.Luminosity.SolarLuminosities));
            Age = Duration.FromYears365(Utilities.RandomNumber(MinSunAge.Years365, Life < MaxSunAge ? Life.Years365 : MaxSunAge.Years365));

            Mass = StellarType.Mass;
            Radius = StellarType.Radius;
            Luminosity = StellarType.Luminosity;
            Temperature = StellarType.Temperature;
            EscapeVelocity = Astro.Dynamics.GetEscapeVelocity(Mass, Radius);
        }

        public StellarBody(IScienceAstrophysics phy, StellarType st, string name) : this(phy, st)
        {
            Name = name;
        }

        //public override Body Parent { get => null; protected set { } }

        public StellarType StellarType { get; protected set; }
        public Color Color { get => StellarType.Color; }
        public double DistanceFromTypical
        {
            get
            {
                var st = StellarType.FromString(StellarType.ToString());
                return Math.Sqrt(Math.Pow(Luminosity.SolarLuminosities - st.Luminosity.SolarLuminosities, 2.0) +
                                 Math.Pow(Radius.SolarRadiuses - st.Radius.SolarRadiuses, 2.0) +
                                 Math.Pow(Mass.SolarMasses - st.Mass.SolarMasses, 2.0) +
                                 Math.Pow(Temperature.Kelvins / GlobalConstants.EARTH_SUN_TEMPERATURE - st.Temperature.Kelvins / GlobalConstants.EARTH_SUN_TEMPERATURE, 2.0));

            }
        }

        /// <summary>
        /// Age of the star in years.
        /// </summary>
        //public Duration Age { get; protected set; }

        /// <summary>
        /// The maximum lifetime of the star in years.
        /// </summary>
        public Duration Life { get; protected set; }

        public Length EcosphereRadius { get => Astro.Astronomy.GetEcosphereRadius(Mass, Luminosity); }

        /// <summary>
        /// Luminosity of the star in solar luminosity units (L<sub>☉</sub>).
        /// The luminosity of the sun is 1.0.
        /// </summary>
        public Luminosity Luminosity { get; protected set; }



        /// <summary>
        /// The mass of this star's companion star (if any) in solar mass
        /// units (M<sub>☉</sub>). 
        /// </summary>
        public Mass BinaryMass { get; protected set; } = Mass.Zero;

        /// <summary>
        /// The semi-major axis of the companion star in au.
        /// </summary>
        public Length BinarySemiMajorAxis { get; protected set; } = Length.Zero;

        /// <summary>
        /// The eccentricity of the companion star's orbit.
        /// </summary>
        public Ratio BinaryEccentricity { get; protected set; } = Ratio.Zero;

        public virtual void GenerateSystem(IBodyFormationAlgorithm frm, CreateSatelliteBodyDelegate createFunc, SystemGenerationOptions genOptions)
        {
            var phy = Astro;

            genOptions ??= new SystemGenerationOptions();
            var sun = this;
            var useRandomTilt = true;

            Length outer_planet_limit = phy.Astronomy.GetOuterLimit(Mass, BinaryMass, BinarySemiMajorAxis, BinaryEccentricity);
            Length outer_dust_limit = phy.Astronomy.GetStellarDustLimit(Mass);
            var seedSystem = frm.CreateSeeds(sun.Mass,
                            sun.Luminosity,
                            Length.Zero,
                            outer_dust_limit,
                            outer_planet_limit,
                            Ratio.FromDecimalFractions(genOptions.DustDensityCoeff),
                            Length.Zero,
                            Ratio.Zero);

            // Todo: swing that to Star.
            Satellites = GenerateSatellites(seedSystem, createFunc, useRandomTilt, genOptions);
        }

        protected abstract IEnumerable<SatelliteBody> GenerateSatellites(IEnumerable<Seed> seeds, CreateSatelliteBodyDelegate createFunc, bool useRandomTilt, SystemGenerationOptions genOptions);

        public override string ToString()
        {
            return Name + " (" + StellarType + ")";
        }
    }
}
