using UnitsNet;

namespace Primoris.Universe.Stargen.Astrophysics
{
	public interface IScienceAstronomy
	{
		/// <summary>
		/// Returns the length of a planet's day in hours.
		/// </summary>
		/// <param name="angularVelocity">The planet's angular velocity in radians/sec</param>
		/// <param name="orbitalPeriodDays">The planet's orbital period in days</param>
		/// <param name="ecc">The eccentricity of the planet's orbit</param>
		/// <returns>Length of day in hours</returns>
		Duration GetDayLength(RotationalSpeed angularVelocityRadSec, Duration orbitalPeriod, Ratio eccentricity);

		/// <summary>
		/// The Hill sphere of an astronomical body is the region in which it dominates the attraction of satellites. 
		/// The outer shell of that region constitutes a zero-velocity surface. To be retained by a planet, a moon must 
		/// have an orbit that lies within the planet's Hill sphere.
		/// </summary>
		/// <param name="sunMass">Mass of the bigger object in solar mass ratio..</param>
		/// <param name="massSM">Mass of the smaller object in solar mass ratio.</param>
		/// <param name="semiMajorAxisAU">Major axis length in AU.</param>
		/// <returns>Hill Sphere radius in AU.</returns>
		Length GetHillSphere(Mass sunMass, Mass massSM, Length semiMajorAxisAU);

        Length GetEcosphereRadius(Mass mass, Luminosity lum);

		Ratio GetMinimumIllumination(Length a, Luminosity l);

		/// <summary>
		/// Orbital zone based on illumination.
		/// </summary>
		/// <remarks>
		/// The size of each zone is algorithm specific.
		/// </remarks>
		/// <param name="luminosity">Luminosity of parent star in solar luminosity ratio.</param>
		/// <param name="orbitalRadius">Orbit major axis length in AU.</param>
		/// <returns>The orbital zones can be one of 1, 2 or 3.</returns>
		int GetOrbitalZone(Luminosity luminosity, Length orbitalRadius);

		/// <summary>
		/// Returns a planet's period in Earth days
		/// </summary>
		/// <param name="separation">Separation in units of AU</param>
		/// <param name="smallMass">Small mass in Units of solar masses</param>
		/// <param name="largeMass">Large mass in Units of solar masses</param>
		/// <returns>Period in Earth days</returns>
		Duration GetPeriod(Length separation, Mass smallMass, Mass largeMass);

        Length GetOuterLimit(Mass mass, Mass otherMass, Length otherSemiMajorAxis, Ratio ecc);

        Length GetStellarDustLimit(Mass mass);
    }
}