namespace Primoris.Universe.Stargen.Astrophysics
{
	public interface IScienceDynamics
	{
		/// <summary>
		/// Returns an approximation of a planet's angular velocity in radians/sec
		/// </summary>
		/// <param name="massSM">Mass of the planet in solar masses</param>
		/// <param name="radiusKM">Radius of the planet in km</param>
		/// <param name="densityGCC">Density of the planet in grams/cm3</param>
		/// <param name="semiMajorAxisAU">Semi-major axis of the planet's orbit
		/// in AU</param>
		/// <param name="isGasGiant">Is the planet a gas giant?</param>
		/// <param name="largeMassSM">Mass of the large mass in solar masses</param>
		/// <param name="largeAgeYears">Age of the large mass in years</param>
		/// <returns>Angular velocity in radians/sec</returns>
		double GetAngularVelocity(double massSM, double radiusKM, double densityGCC, double semiMajorAxisAU, bool isGasGiant, double largeMassSM, double largeAgeYears);

		/// <summary>
		/// Provides an approximation of angular velocity for non-tidally decelerated
		/// planets.
		/// </summary>
		/// <param name="massSM">Mass of the body in solar masses</param>
		/// <param name="radiusKM">Radius of the body in km</param>
		/// <param name="isGasGiant">Is the body a gas giant?</param>
		/// <returns>Angular velocity in rad/sec</returns>
		double GetBaseAngularVelocity(double massSM, double radiusKM, bool isGasGiant);

		/// <summary>
		/// Provides an approximation of braking due to tidal forces as a ratio to the
		/// effect on Earth.
		/// </summary>
		/// <param name="densityGCC">Density of the body in grams/cm3</param>
		/// <param name="massSM">Mass of the body in solar masses</param>
		/// <param name="radiusKM">Radius of the body in KM</param>
		/// <param name="semiMajorAxisAU">Semi-major axis of the body's orbit in AU</param>
		/// <param name="largeMassSM">Mass of the large mass in solar masses</param>
		/// <returns></returns>
		double GetChangeInAngularVelocity(double densityGCC, double massSM, double radiusKM, double semiMajorAxisAU, double largeMassSM);

		/// <summary>
		/// Returns escape velocity in cm/sec
		/// </summary>
		/// <param name="mass">Mass in units of solar mass</param>
		/// <param name="radius">Radius in km</param>
		/// <returns></returns>
		double GetEscapeVelocity(double massSM, double radius);

	}
}