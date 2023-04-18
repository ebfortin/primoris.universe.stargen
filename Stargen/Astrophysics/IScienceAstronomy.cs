using UnitsNet;

namespace Primoris.Universe.Stargen.Astrophysics;

public interface IScienceAstronomy
{
	/// <summary>
	/// Returns the length of a planet's day.
	/// </summary>
	/// <param name="angularVelocity">The planet's angular velocity.</param>
	/// <param name="orbitalPeriodDays">The planet's orbital period.</param>
	/// <param name="ecc">The eccentricity of the planet's orbit</param>
	/// <returns>Length of day.</returns>
	Duration GetDayLength(RotationalSpeed angularVelocityRadSec, Duration orbitalPeriod, Ratio eccentricity);

	/// <summary>
	/// The Hill sphere of an astronomical body is the region in which it dominates the attraction of satellites. 
	/// The outer shell of that region constitutes a zero-velocity surface. To be retained by a planet, a moon must 
	/// have an orbit that lies within the planet's Hill sphere.
	/// </summary>
	/// <param name="sunMass">Mass of the bigger object.</param>
	/// <param name="massSM">Mass of the smaller object.</param>
	/// <param name="semiMajorAxisAU">Major axis length.</param>
	/// <returns>Hill Sphere radius.</returns>
	Length GetHillSphere(Mass sunMass, Mass massSM, Length semiMajorAxisAU);

	/// <summary>
	/// Returns the radius (center?) of a star's ecosphere.
	/// </summary>
	/// <param name="luminosity">Luminosity of the star.</param>
	/// <returns>Star's ecosphere radius.</returns>
	Length GetEcosphereRadius(Mass mass, Luminosity lum);

	/// <summary>
	/// Get the luminosity perceived by a body at a distance from a StellarBody.
	/// </summary>
	/// <param name="a">Distance to StellarBody.</param>
	/// <param name="l">Luminosity of the StellarBody.</param>
	/// <returns></returns>
	Ratio GetMinimumIllumination(Length a, Luminosity l);

	/// <summary>
	/// Orbital zone based on illumination.
	/// </summary>
	/// <remarks>
	/// The size of each zone is algorithm specific.
	/// </remarks>
	/// <param name="luminosity">Luminosity of parent star.</param>
	/// <param name="orbitalRadius">Orbit major axis length.</param>
	/// <returns>The orbital zones can be one of 1, 2 or 3.</returns>
	int GetOrbitalZone(Luminosity luminosity, Length orbitalRadius);

	/// <summary>
	/// Returns a planet's period.
	/// </summary>
	/// <param name="separation">Separation between masses</param>
	/// <param name="smallMass">Small mass.</param>
	/// <param name="largeMass">Large mass.</param>
	/// <returns>Period.</returns>
	Duration GetPeriod(Length separation, Mass smallMass, Mass largeMass);

	/// <summary>
	/// Gets the outer limit at which a SatelliteBody orbit is stable in a Binary System. 
	/// </summary>
	/// <param name="mass">The mass.</param>
	/// <param name="otherMass">The other mass.</param>
	/// <param name="otherSemiMajorAxis">The other semi major axis.</param>
	/// <param name="ecc">The eccentricity of the body.</param>
	/// <returns>Distance at which a SatelliteBody orbit is stable.</returns>
	Length GetOuterLimit(Mass mass, Mass otherMass, Length otherSemiMajorAxis, Ratio ecc);

	/// <summary>
	/// Given a StellarBody mass, returns the limit on which it retains dust.
	/// </summary>
	/// <param name="mass">Mass of the StellarBody.</param>
	/// <returns>Distance from the StellarBody dust is retained.</returns>
        Length GetStellarDustLimit(Mass mass);

	/// <summary>
	/// Returns the luminosity of a star using the Mass-Luminosity relationship.
	/// </summary>
	/// <param name="massRatio">Mass of the StellarBody.</param>
	/// <returns>StellarBody luminosity.</returns>
	Luminosity GetLuminosityFromMass(Mass mass);
    }