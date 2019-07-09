using Primoris.Universe.Stargen.Bodies;

namespace Primoris.Universe.Stargen.Astrophysics
{
	public interface IBodyPhysics
	{
		/// <summary>
		/// Provides an approximation of angular velocity for non-tidally decelerated
		/// planets.
		/// </summary>
		/// <param name="massSM">Mass of the body in solar masses</param>
		/// <param name="radiusKM">Radius of the body in km</param>
		/// <param name="isGasGiant">Is the body a gas giant?</param>
		/// <returns>Angular velocity in rad/sec</returns>
		double GetBaseAngularVelocity(double massSM,
								double radiusKM,
								bool isGasGiant);

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
		double GetChangeInAngularVelocity(double densityGCC,
									double massSM,
									double radiusKM,
									double semiMajorAxisAU,
									double largeMassSM);

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
		double GetAngularVelocity(double massSM,
							double radiusKM,
							double densityGCC,
							double semiMajorAxisAU,
							bool isGasGiant,
							double largeMassSM,
							double largeAgeYears);
		double GetBoilingPointWater(double surfpres);

		/// <summary>
		/// Returns the length of a planet's day in hours.
		/// </summary>
		/// <param name="angularVelocity">The planet's angular velocity in radians/sec</param>
		/// <param name="orbitalPeriodDays">The planet's orbital period in days</param>
		/// <param name="ecc">The eccentricity of the planet's orbit</param>
		/// <returns>Length of day in hours</returns>
		double GetDayLength(double angularVelocityRadSec,
					  double orbitalPeriod,
					  double eccentricity);
		double GetDensityFromBody(double massSM,
							double radius);
		double GetDensityFromStar(double massSM,
							double semiMajorAxisAU,
							double ecosphereRadiusAU,
							bool isGasGiant);

		/// <summary>
		/// Returns escape velocity in cm/sec
		/// </summary>
		/// <param name="mass">Mass in units of solar mass</param>
		/// <param name="radius">Radius in km</param>
		/// <returns></returns>
		double GetEscapeVelocity(double massSM,
						   double radius);
		double GetExosphereTemperature(double semiMajorAxisAu,
								 double ecosphereRadiusAU,
								 double sunTemperature);
		double GetHillSphere(double sunMass,
					   double massSM,
					   double semiMajorAxisSM);
		double GetMolecularWeightRetained(double surfGrav,
									double mass,
									double radius,
									double exosphereTemp,
									double sunAge);
		BodyType GetBodyType(double massSM,
					   double gasMassSM,
					   double molecularWeightRetained,
					   double surfacePressure,
					   double waterCoverFraction,
					   double iceCoverFraction,
					   double maxTemperature,
					   double boilingPointWater,
					   double surfaceTemperature);
		double GetRadius(double massSM,
				   int orbitZone,
				   bool giant);
		double GetRMSVelocityCMSec(double exoTemp);
		double GetSurfacePressure(double volatileGasInventory,
							double radius,
							double surfaceGravity);
		double GetVolatileGasInventory(double massSM,
								 double escapeVelocity,
								 double rmsVelocity,
								 double sunMass,
								 double gasMassSM,
								 int orbitZone,
								 bool hasGreenhouse,
								 bool hasAccretedGas);
		bool TestHasGreenhouseEffect(double ecosphereRadius,
							   double semiAxisMajorAU);

		/// <summary>
		/// Checks if a planet's rotation is in resonance with its orbit
		/// </summary>
		/// <param name="angularVelocity">The planet's angular velocity in radians/sec</param>
		/// <param name="dayInHours">The length of the planet's day in hours</param>
		/// <param name="orbitalPeriodDays">The orbital period of the planet in days</param>
		/// <param name="ecc">The eccentricity of the planet's orbit</param>
		/// <returns>True if the planet is in a resonant orbit</returns>
		bool TestHasResonantPeriod(double angularVelocityRadSec,
							 double dayLength,
							 double orbitalPeriod,
							 double eccentricity);
		bool TestIsGasGiant(double massSM,
					  double gasMassSM,
					  double molecularWeightRetained);
		bool TestIsTidallyLocked(double dayLength,
						   double orbitalPeriod);
		double GetMinimumIllumination(double a,
								double l);
		double GetRocheLimit(double bodyRadius,
					   double bodyDensity,
					   double satelliteDensity);
		bool TestIsHabitable(double dayLength,
					   double orbitalPeriod,
					   Breathability breathability,
					   bool hasResonantPeriod,
					   bool isTidallyLocked);
		int GetOrbitalZone(double luminosity, double orbitalRadius);
		bool TestIsEarthLike(double surfaceTemperature,
					   double waterCoverFraction,
					   double cloudCoverFraction,
					   double iceCoverFraction,
					   double surfacePressure,
					   double surfaceGravityG,
					   Breathability breathability,
					   BodyType type);

		/// <summary>
		/// Returns a planet's period in Earth days
		/// </summary>
		/// <param name="separation">Separation in units of AU</param>
		/// <param name="smallMass">Small mass in Units of solar masses</param>
		/// <param name="largeMass">Large mass in Units of solar masses</param>
		/// <returns>Period in Earth days</returns>
		double GetPeriod(double separation,
				   double smallMass,
				   double largeMass);
	}
}