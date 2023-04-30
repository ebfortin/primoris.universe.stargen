using Primoris.Universe.Stargen.Bodies;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Primoris.Universe.Stargen.Astrophysics.Singularity;

public class SingularityPhysics : IScienceAstrophysics, IScienceAstronomy, IScienceDynamics, ISciencePhysics, ISciencePlanetology, IScienceThermodynamics
{
    public IScienceAstronomy Astronomy => this;

    public IScienceDynamics Dynamics => this;

    public ISciencePhysics Physics => this;

    public ISciencePlanetology Planetology => this;

    public IScienceThermodynamics Thermodynamics => this;




    public Ratio GetAlbedo(Ratio waterFraction, Ratio cloudFraction, Ratio iceFraction, Pressure surfPressure)
    {
        return Ratio.Zero;
    }

    public Acceleration GetAcceleration(Mass mass, Length radius)
    {
        return Acceleration.Zero;
    }

    public RotationalSpeed GetAngularVelocity(Mass massSM, Length radiusKM, Density densityGCC, Length semiMajorAxisAU, bool isGasGiant, Mass largeMassSM, Duration largeAgeYears)
    {
        return RotationalSpeed.Zero;
    }

    public RotationalSpeed GetBaseAngularVelocity(Mass massSM, Length radiusKM, bool isGasGiant)
    {
        return RotationalSpeed.Zero;
    }

    public BodyType GetBodyType(Mass massSM, Mass gasMassSM, Mass molecularWeightRetained, Pressure surfacePressure, Ratio waterCoverFraction, Ratio iceCoverFraction, Temperature maxTemperature, Temperature boilingPointWater, Temperature surfaceTemperature)
    {
        return BodyType.Undefined;
    }

    public RotationalSpeed GetChangeInAngularVelocity(Density densityGCC, Mass massSM, Length radiusKM, Length semiMajorAxisAU, Mass largeMassSM)
    {
        return RotationalSpeed.Zero;
    }

    public Duration GetDayLength(RotationalSpeed angularVelocityRadSec, Duration orbitalPeriod, Ratio eccentricity)
    {
        return Duration.Zero;
    }

    public Density GetDensityFromBody(Mass massSM, Length radius)
    {
        return Density.Zero;
    }

    public Density GetDensityFromStar(Mass massSM, Length semiMajorAxisAU, Length ecosphereRadiusAU, bool isGasGiant)
    {
        return Density.Zero;
    }

    public Length GetEcosphereRadius(Mass mass, Luminosity lum)
    {
        return Length.Zero;
    }

    public Speed GetEscapeVelocity(Mass massSM, Length radius)
    {
        return Speed.FromKilometersPerSecond(double.PositiveInfinity);
    }

    public Duration GetGasLife(Mass molecularWeight, Temperature exoTempKelvin, Acceleration surfGravG, Length radiusKM)
    {
        return Duration.FromYears365(double.PositiveInfinity);
    }

    public Length GetHillSphere(Mass sunMass, Mass massSM, Length semiMajorAxisAU)
    {
        return Length.Zero;
    }

    public Ratio GetIceFraction(Ratio hydroFraction, Temperature surfTemp)
    {
        return Ratio.Zero;
    }

    public Mass GetInitialMolecularWeightRetained(Mass mass, Length radius, Temperature exoTemp)
    {
        return Mass.FromKilograms(double.PositiveInfinity);
    }

    public Pressure GetInspiredPartialPressure(Pressure surf_pressure, Pressure gas_pressure)
    {
        return Pressure.FromAtmospheres(double.PositiveInfinity);
    }

    public Luminosity GetLuminosityFromMass(Mass mass)
    {
        return Luminosity.Zero;
    }

    public Ratio GetMinimumIllumination(Length a, Luminosity l)
    {
        return Ratio.Zero;
    }

    public Mass GetMolecularWeightRetained(Acceleration surfGrav, Mass mass, Length radius, Temperature exosphereTemp, Duration sunAge)
    {
        return Mass.FromKilograms(double.PositiveInfinity);
    }

    public Ratio GetOpacity(Mass molecularWeight, Pressure surfPressure)
    {
        return Ratio.FromPercent(100.0);
    }

    public int GetOrbitalZone(Luminosity luminosity, Length orbitalRadius)
    {
        return 0;
    }

    public Length GetOuterLimit(Mass mass, Mass otherMass, Length otherSemiMajorAxis, Ratio ecc)
    {
        return Length.Zero;
    }

    public Duration GetPeriod(Length separation, Mass smallMass, Mass largeMass)
    {
        return Duration.Zero;
    }

    public Length GetRadius(Mass mass, Density density)
    {
        return Length.Zero;
    }

    public Speed GetRMSVelocity(Mass molecularWeight, Temperature exoTemp)
    {
        return Speed.Zero;
    }

    public Length GetRocheLimit(Length bodyRadius, Density bodyDensity, Density satelliteDensity)
    {
        return Length.Zero;
    }

    public Length GetStellarDustLimit(Mass mass)
    {
        return Length.Zero;
    }

    public Pressure GetSurfacePressure(Ratio volatileGasInventory, Length radius, Acceleration surfaceGravity)
    {
        return Pressure.FromAtmospheres(double.PositiveInfinity);
    }

    public Ratio GetVolatileGasInventory(Mass massSM, Speed escapeVelocity, Speed rmsVelocity, Mass sunMass, Mass gasMassSM, int orbitZone, bool hasGreenhouse)
    {
        return Ratio.Zero;
    }

    public Ratio GetWaterFraction(Ratio volatileGasInventory, Length planetRadius)
    {
        return Ratio.Zero;
    }

    public bool TestHasGreenhouseEffect(Length ecosphereRadius, Length semiAxisMajorAU)
    {
        return false;
    }

    public bool TestHasResonantPeriod(RotationalSpeed angularVelocityRadSec, Duration dayLength, Duration orbitalPeriod, Ratio eccentricity)
    {
        return false;
    }

    public bool TestIsEarthLike(Temperature surfaceTemperature, Ratio waterCoverFraction, Ratio cloudCoverFraction, Ratio iceCoverFraction, Pressure surfacePressure, Acceleration surfaceGravityG, Breathability breathability, BodyType type)
    {
        return false;
    }

    public bool TestIsGasGiant(Mass massSM, Mass gasMassSM, Mass molecularWeightRetained)
    {
        return false;
    }

    public bool TestIsHabitable(Duration dayLength, Duration orbitalPeriod, Breathability breathability, bool hasResonantPeriod, bool isTidallyLocked)
    {
        return false;
    }

    public bool TestIsTidallyLocked(Duration dayLength, Duration orbitalPeriod)
    {
        return false;
    }

    public Length GetCoreRadius(Mass massSM, int orbitZone, bool giant)
    {
        return Length.Zero;
    }

    public Ratio GetCloudFraction(Temperature surfaceTemp, Mass smallestMWRetained, Length equatorialRadius, Ratio hydroFraction)
    {
        return Ratio.Zero;
    }

    public Temperature GetBoilingPointWater(Pressure surfpres)
    {
        return Temperature.Zero;
    }

    public Temperature GetEstimatedExosphereTemperature(Length semiMajorAxisAu, Length ecosphereRadiusAU, Temperature sunTemperature)
    {
        return Temperature.Zero;
    }

    public Temperature GetEstimatedEffectiveTemperature(Length ecosphereRadius, Length orbitalRadius, Ratio albedo)
    {
        return Temperature.Zero;
    }

    public TemperatureDelta GetGreenhouseTemperatureRise(Ratio opticalDepth, Temperature effectiveTemp, Pressure surfPressure)
    {
        return TemperatureDelta.Zero;
    }

    public Temperature GetEstimatedAverageTemperature(Length ecosphereRadius, Length orbitalRadius, Ratio albedo)
    {
        return Temperature.Zero;
    }


}
