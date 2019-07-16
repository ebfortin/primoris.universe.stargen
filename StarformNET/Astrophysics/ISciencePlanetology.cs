using Primoris.Universe.Stargen.Bodies;

namespace Primoris.Universe.Stargen.Astrophysics
{
	public interface ISciencePlanetology
	{
		BodyType GetBodyType(double massSM, double gasMassSM, double molecularWeightRetained, double surfacePressure, double waterCoverFraction, double iceCoverFraction, double maxTemperature, double boilingPointWater, double surfaceTemperature);
		double GetCoreRadius(double massSM, int orbitZone, bool giant);
		bool TestHasGreenhouseEffect(double ecosphereRadius, double semiAxisMajorAU);

		/// <summary>
		/// Checks if a planet's rotation is in resonance with its orbit
		/// </summary>
		/// <param name="angularVelocity">The planet's angular velocity in radians/sec</param>
		/// <param name="dayInHours">The length of the planet's day in hours</param>
		/// <param name="orbitalPeriodDays">The orbital period of the planet in days</param>
		/// <param name="ecc">The eccentricity of the planet's orbit</param>
		/// <returns>True if the planet is in a resonant orbit</returns>
		bool TestHasResonantPeriod(double angularVelocityRadSec, double dayLength, double orbitalPeriod, double eccentricity);
		bool TestIsEarthLike(double surfaceTemperature, double waterCoverFraction, double cloudCoverFraction, double iceCoverFraction, double surfacePressure, double surfaceGravityG, Breathability breathability, BodyType type);
		bool TestIsGasGiant(double massSM, double gasMassSM, double molecularWeightRetained);
		bool TestIsHabitable(double dayLength, double orbitalPeriod, Breathability breathability, bool hasResonantPeriod, bool isTidallyLocked);
		bool TestIsTidallyLocked(double dayLength, double orbitalPeriod);
	}
}