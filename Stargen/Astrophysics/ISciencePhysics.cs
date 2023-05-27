namespace Primoris.Universe.Stargen.Astrophysics;

public interface ISciencePhysics
{
    /// <summary>
    /// Returns the density of a Body given it's mass and radius.
    /// </summary>
    /// <param name="massSM">Mass of the body.</param>
    /// <param name="radius">Radius of the body.</param>
    /// <returns>Density of the body.</returns>
    Density GetDensityFromBody(Mass massSM, Length radius);

    /// <summary>
    /// Returns an approximation of the density of a Body given its StellarBody parent.
    /// </summary>
    /// <param name="massSM">Mass of the StellarBody.</param>
    /// <param name="semiMajorAxisAU">Semi major axis of the Body.</param>
    /// <param name="ecosphereRadiusAU">Ecosphere radius of the StellarBody.</param>
    /// <param name="isGasGiant">True if the Body is a Gas Giant.</param>
    /// <returns>Density approximation from StellarBody characteristics.</returns>
    Density GetDensityFromStar(Mass massSM, Length semiMajorAxisAU, Length ecosphereRadiusAU, bool isGasGiant);

    /// <summary>
    /// Returns the smallest 
    /// </summary>
    /// <param name="surfGrav"></param>
    /// <param name="mass"></param>
    /// <param name="radius"></param>
    /// <param name="exosphereTemp"></param>
    /// <param name="sunAge"></param>
    /// <returns></returns>
    Mass GetMolecularWeightRetained(Acceleration surfGrav, Mass mass, Length radius, Temperature exosphereTemp, Duration sunAge);

    /// <summary>
    /// Calculates the number of years it takes for 1/e of a gas to escape from a
    /// planet's atmosphere
    /// </summary>
    /// <param name="molecularWeight">Molecular weight of the gas</param>
    /// <param name="exoTempKelvin">Exosphere temperature of the planet in Kelvin</param>
    /// <param name="surfGravG">Surface gravity of the planet in G</param>
    /// <param name="radiusKM">Radius of the planet in km</param>
    /// <returns></returns>
    Duration GetGasLife(Mass molecularWeight, Temperature exoTempKelvin, Acceleration surfGravG, Length radiusKM);

    /// <summary>
    /// Returns the smallest molecular weight retained by the body, which is useful
    /// for determining atmospheric composition.
    /// </summary>
    /// <remarks>
    /// This method should be used for the initial value of an iterative process to find a good value
    /// for molecular weight retained.
    /// </remarks>
    /// <param name="mass">Mass of the SatelliteBody.</param>
    /// <param name="radius">Equatorial radius of the SatelliteBody.</param>
    /// <param name="exoTemp">Temperature of the exosphere.</param>
    /// <returns></returns>
    Mass GetInitialMolecularWeightRetained(Mass mass, Length radius, Temperature exoTemp);


    /// <summary>
    /// Calculates the root-mean-square velocity of particles in a gas.
    /// </summary>
    /// <param name="molecularWeight">Mass of gas for each mol.</param>
    /// <param name="exoTemp">Temperature of the exosphere.</param>
    /// <returns>Returns RMS velocity.</returns>
    Speed GetRMSVelocity(Mass molecularWeight, Temperature exoTemp);

    /// <summary>
    /// Calculate the Roche Limit of two bodies.
    /// </summary>
    /// <param name="bodyRadius">Radius of the main body.</param>
    /// <param name="bodyDensity">Density of the main body.</param>
    /// <param name="satelliteDensity">Density of the Satellite Body.</param>
    /// <returns>Roche limit from the main body.</returns>
    Length GetRocheLimit(Length bodyRadius, Density bodyDensity, Density satelliteDensity);

    /// <summary>
    /// Get the surface pressure of a body with atmosphere.
    /// </summary>
    /// <param name="volatileGasInventory">Volatile gas inventory unitless value.</param>
    /// <param name="radius">Radius of the body.</param>
    /// <param name="surfaceGravity">Surface gravity of the body.</param>
    /// <returns>Surface pressure of the atmosphere.</returns>
    Pressure GetSurfacePressure(Ratio volatileGasInventory, Length radius, Acceleration surfaceGravity);

    /// <summary>
    /// Returns the volatile gas inventory unitless value.
    /// </summary>
    /// <param name="massSM">Mass of the body.</param>
    /// <param name="escapeVelocity">Escape velocity of the body.</param>
    /// <param name="rmsVelocity">RMS velocity of gases.</param>
    /// <param name="sunMass">Mass of the StellarBody of the system.</param>
    /// <param name="gasMassSM">Mass of gas for the body.</param>
    /// <param name="orbitZone">Orbital zone as given by GetOrbitZone().</param>
    /// <param name="hasGreenhouse">True if the Body has a greenhouse temperature rise effect.</param>
    /// <returns>Unitless volatile gas inventory.</returns>
    Ratio GetVolatileGasInventory(Mass massSM, Speed escapeVelocity, Speed rmsVelocity, Mass sunMass, Mass gasMassSM, int orbitZone, bool hasGreenhouse);

    /// <summary>
    /// Calculates the surface acceleration of the planet.
    /// </summary>
    /// <param name="mass">Mass of the planet.</param>
    /// <param name="radius">Radius of the planet.</param>
    /// <returns>Acceleration at the surface of the planet.</returns>
    Acceleration GetAcceleration(Mass mass, Length radius);

    /// <summary>
    /// Inspired partial pressure, taking into account humidification of the air in the nasal
    /// passage and throat.
    /// </summary>
    /// <param name="surf_pressure">Total atmospheric surface pressure.</param>
    /// <param name="gas_pressure">Partial gas pressure.</param>
    /// <returns>Inspired partial pressure.</returns>
    Pressure GetInspiredPartialPressure(Pressure surf_pressure, Pressure gas_pressure);
}