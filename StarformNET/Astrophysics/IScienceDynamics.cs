using UnitsNet;

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
		RotationalSpeed GetAngularVelocity(Mass massSM, Length radiusKM, Density densityGCC, Length semiMajorAxisAU, bool isGasGiant, Mass largeMassSM, Duration largeAgeYears);

		/// <summary>
		/// Provides an approximation of angular velocity for non-tidally decelerated
		/// planets.
		/// </summary>
		/// <param name="massSM">Mass of the body in solar masses</param>
		/// <param name="radiusKM">Radius of the body in km</param>
		/// <param name="isGasGiant">Is the body a gas giant?</param>
		/// <returns>Angular velocity in rad/sec</returns>
		RotationalSpeed GetBaseAngularVelocity(Mass massSM, Length radiusKM, bool isGasGiant);

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
		RotationalSpeed GetChangeInAngularVelocity(Density densityGCC, Mass massSM, Length radiusKM, Length semiMajorAxisAU, Mass largeMassSM);

		/// <summary>
		/// Returns escape velocity in cm/sec
		/// </summary>
		/// <param name="mass">Mass in units of solar mass</param>
		/// <param name="radius">Radius in km</param>
		/// <returns></returns>
		Speed GetEscapeVelocity(Mass massSM, Length radius);

	}
}