using UnitsNet;

namespace Primoris.Universe.Stargen.Astrophysics
{
	public interface IScienceDynamics
	{
		/// <summary>
		/// Returns an approximation of a planet's angular velocity.
		/// </summary>
		/// <param name="massSM">Mass of the planet.</param>
		/// <param name="radiusKM">Radius of the planet.</param>
		/// <param name="densityGCC">Density of the planet.</param>
		/// <param name="semiMajorAxisAU">Semi-major axis of the planet.</param>
		/// <param name="isGasGiant">Is the planet a gas giant?</param>
		/// <param name="largeMassSM">Mass of the large mass.</param>
		/// <param name="largeAgeYears">Age of the large mass.</param>
		/// <returns>Angular velocity.</returns>
		RotationalSpeed GetAngularVelocity(Mass massSM,
                                     Length radiusKM,
                                     Density densityGCC,
                                     Length semiMajorAxisAU,
                                     bool isGasGiant,
                                     Mass largeMassSM,
                                     Duration largeAgeYears);

		/// <summary>
		/// Provides an approximation of angular velocity for non-tidally decelerated
		/// planets.
		/// </summary>
		/// <param name="massSM">Mass of the body.</param>
		/// <param name="radiusKM">Radius of the body.</param>
		/// <param name="isGasGiant">Is the body a gas giant?</param>
		/// <returns>Angular velocity.</returns>
		RotationalSpeed GetBaseAngularVelocity(Mass massSM, Length radiusKM, bool isGasGiant);

		/// <summary>
		/// Provides an approximation of braking due to tidal forces as a ratio to the
		/// effect on Earth.
		/// </summary>
		/// <param name="densityGCC">Density of the body.</param>
		/// <param name="massSM">Mass of the body.</param>
		/// <param name="radiusKM">Radius of the body.</param>
		/// <param name="semiMajorAxisAU">Semi-major axis of the body's orbit.</param>
		/// <param name="largeMassSM">Mass of the large mass.</param>
		/// <returns>Breaking rotational speed.</returns>
		RotationalSpeed GetChangeInAngularVelocity(Density densityGCC, Mass massSM, Length radiusKM, Length semiMajorAxisAU, Mass largeMassSM);

		/// <summary>
		/// Returns escape velocity.
		/// </summary>
		/// <param name="massSM">Mass of Body.</param>
		/// <param name="radius">Radius of Body.</param>
		/// <returns>Escape velocity.</returns>
		Speed GetEscapeVelocity(Mass massSM, Length radius);

	}
}