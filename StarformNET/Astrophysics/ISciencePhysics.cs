using UnitsNet;

namespace Primoris.Universe.Stargen.Astrophysics
{
	public interface ISciencePhysics
	{
		Density GetDensityFromBody(Mass massSM, Length radius);
		Density GetDensityFromStar(Mass massSM, Length semiMajorAxisAU, Length ecosphereRadiusAU, bool isGasGiant);
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
        /// <param name="mass">Mass in solar masses</param>
        /// <param name="radius">Equatorial radius in units of kilometers</param>
        /// <param name="exoTemp"></param>
        /// <returns></returns>
        Mass GetInitialMolecularWeightRetained(Mass mass, Length radius, Temperature exoTemp);


		/// <summary>
		/// Calculates the root-mean-square velocity of particles in a gas.
		/// </summary>
		/// <param name="molecularWeight">Mass of gas in g/mol</param>
		/// <param name="exoTemp">Temperature in K</param>
		/// <returns>Returns RMS velocity in m/sec</returns>
		Speed GetRMSVelocity(Mass molecularWeight, Temperature exoTemp);
		Length GetRocheLimit(Length bodyRadius, Density bodyDensity, Density satelliteDensity);
		Pressure GetSurfacePressure(Ratio volatileGasInventory, Length radius, Acceleration surfaceGravity);
		Ratio GetVolatileGasInventory(Mass massSM, Speed escapeVelocity, Speed rmsVelocity, Mass sunMass, Mass gasMassSM, int orbitZone, bool hasGreenhouse);

		/// <summary>
		/// Calculates the surface acceleration of the planet.
		/// </summary>
		/// <param name="mass">Mass of the planet in solar masses</param>
		/// <param name="radius">Radius of the planet in km</param>
		/// <returns>Acceleration returned in units of cm/sec2</returns>
		Acceleration GetAcceleration(Mass mass, Length radius);

		/// <summary>
		/// Inspired partial pressure, taking into account humidification of the air in the nasal
		/// passage and throat.
		/// </summary>
		/// <param name="surf_pressure">Total atmospheric surface pressure in millibars</param>
		/// <param name="gas_pressure">Partial gas pressure in millibars</param>
		/// <returns>Inspired partial pressure in millibars</returns>
		Pressure GetInspiredPartialPressure(Pressure surf_pressure, Pressure gas_pressure);
	}
}