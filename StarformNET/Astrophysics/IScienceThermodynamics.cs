namespace Primoris.Universe.Stargen.Astrophysics
{
	public interface IScienceThermodynamics
	{
		double GetBoilingPointWater(double surfpres);
		double GetExosphereTemperature(double semiMajorAxisAu, double ecosphereRadiusAU, double sunTemperature);

		// TODO look up Fogg's eq.19. and write a summary
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ecosphereRadius">Ecosphere radius in AU</param>
		/// <param name="orbitalRadius">Orbital radius in AU</param>
		/// <param name="albedo"></param>
		/// <returns>Temperature in Kelvin</returns>
		public double GetEffectiveTemperature(double ecosphereRadius, double orbitalRadius, double albedo);

		/// <summary>
		/// Returns the rise in temperature produced by the greenhouse effect.
		/// </summary>
		/// <param name="opticalDepth"></param>
		/// <param name="effectiveTemp">Effective temperature in units of Kelvin</param>
		/// <param name="surfPressure"></param>
		/// <returns>Rise in temperature in Kelvin</returns>
		public double GetGreenhouseTemperatureRise(double opticalDepth, double effectiveTemp, double surfPressure);
	}
}