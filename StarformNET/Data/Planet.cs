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
        public Star Star { get; set; }
        public Atmosphere Atmosphere = new Atmosphere();

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

        public Planet(PlanetSeed seed, Star star, int num)
        {
            Star = star;
            Position = num;
            SemiMajorAxisAU = seed.SemiMajorAxisAU;
            Eccentricity = seed.Eccentricity;
            MassSM = seed.Mass;
            DustMassSM = seed.DustMass;
            GasMassSM = seed.GasMass;
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
