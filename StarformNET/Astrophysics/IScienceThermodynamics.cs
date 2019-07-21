using System;
using UnitsNet;

namespace Primoris.Universe.Stargen.Astrophysics
{
	public interface IScienceThermodynamics
	{
		Temperature GetBoilingPointWater(Pressure surfpres);

		Temperature GetExosphereTemperature(Length semiMajorAxisAu, Length ecosphereRadiusAU, Temperature sunTemperature);

		// TODO look up Fogg's eq.19. and write a summary
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ecosphereRadius">Ecosphere radius in AU</param>
		/// <param name="orbitalRadius">Orbital radius in AU</param>
		/// <param name="albedo"></param>
		/// <returns>Temperature in Kelvin</returns>
		Temperature GetEffectiveTemperature(Length ecosphereRadius, Length orbitalRadius, Ratio albedo);

		/// <summary>
		/// Returns the rise in temperature produced by the greenhouse effect.
		/// </summary>
		/// <param name="opticalDepth"></param>
		/// <param name="effectiveTemp">Effective temperature in units of Kelvin</param>
		/// <param name="surfPressure"></param>
		/// <returns>Rise in temperature in Kelvin</returns>
		TemperatureDelta GetGreenhouseTemperatureRise(Ratio opticalDepth, Temperature effectiveTemp, Pressure surfPressure);

		// TODO figure out how this function differs from EffTemp, write summary
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ecosphereRadius"></param>
		/// <param name="orbitalRadius"></param>
		/// <param name="albedo"></param>
		/// <returns></returns>
		Temperature GetEstimatedTemperature(Length ecosphereRadius, Length orbitalRadius, Ratio albedo);
	}
}