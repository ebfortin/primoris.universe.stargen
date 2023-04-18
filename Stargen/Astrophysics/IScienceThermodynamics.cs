using System;
using UnitsNet;

namespace Primoris.Universe.Stargen.Astrophysics;

public interface IScienceThermodynamics
{
	/// <summary>
	/// Returns the boiling point of water given a SatelliteBody surface pressure.
	/// </summary>
	/// <param name="surfpres">Surface pressure.</param>
	/// <returns>Boiling point (temperature) of water (H2O) in the SatelliteBody situation.</returns>
	Temperature GetBoilingPointWater(Pressure surfpres);

	/// <summary>
	/// Returns an estimate of the exosphere of a SatelliteBody.
	/// </summary>
	/// <param name="semiMajorAxisAu">Semi major axis.</param>
	/// <param name="ecosphereRadiusAU">Ecosphere of the StellarBody.</param>
	/// <param name="sunTemperature">Temperature of the StellarBody.</param>
	/// <returns>Estimated Exosphere temperature.</returns>
	Temperature GetEstimatedExosphereTemperature(Length semiMajorAxisAu, Length ecosphereRadiusAU, Temperature sunTemperature);

	/// <summary>
	/// Returns the estimated temperature based on orbit position, albedo, ecosphere radius.
	/// </summary>
	/// <param name="ecosphereRadius">Ecosphere radius in AU</param>
	/// <param name="orbitalRadius">Orbital radius in AU</param>
	/// <param name="albedo"></param>
	/// <returns>Estimated effective temperature.</returns>
	Temperature GetEstimatedEffectiveTemperature(Length ecosphereRadius, Length orbitalRadius, Ratio albedo);

	/// <summary>
	/// Returns the rise in temperature produced by the greenhouse effect.
	/// </summary>
	/// <param name="opticalDepth">Optical depth of the Atmosphere.</param>
	/// <param name="effectiveTemp">Effective temperature at the surface.</param>
	/// <param name="surfPressure">Surface pressure.</param>
	/// <returns>Rise in temperature in Kelvin</returns>
	TemperatureDelta GetGreenhouseTemperatureRise(Ratio opticalDepth, Temperature effectiveTemp, Pressure surfPressure);

	/// <summary>
	/// Returns the estimated temperature based on orbit position, albedo, ecosphere radius.
	/// </summary>
	/// <param name="ecosphereRadius">Ecophere radius of the StellarBody.</param>
	/// <param name="orbitalRadius">Semi major axis of orbit.</param>
	/// <param name="albedo">SatelliteBody albedo.</param>
	/// <returns>Estimated average temperature.</returns>
	Temperature GetEstimatedAverageTemperature(Length ecosphereRadius, Length orbitalRadius, Ratio albedo);
}