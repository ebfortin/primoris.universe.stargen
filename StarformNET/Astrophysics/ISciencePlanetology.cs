using Primoris.Universe.Stargen.Bodies;

namespace Primoris.Universe.Stargen.Astrophysics
{
	public interface ISciencePlanetology
	{
		BodyType GetBodyType(double massSM, double gasMassSM, double molecularWeightRetained, double surfacePressure, double waterCoverFraction, double iceCoverFraction, double maxTemperature, double boilingPointWater, double surfaceTemperature);
		double GetCoreRadius(double massSM, int orbitZone, bool giant);

		/// <summary>
		/// Returns the dimensionless quantity of optical depth, which is useful in determing the amount
		/// of greenhouse effect on a planet.
		/// </summary>
		/// <param name="molecularWeight"></param>
		/// <param name="surfPressure"></param>
		/// <returns></returns>
		public double GetOpacity(double molecularWeight, double surfPressure);


		/// <summary>
		/// Given the volatile gas inventory and
		/// planetary radius of a planet (in Km), this function returns the
		/// fraction of the planet covered with water.	
		/// </summary>
		/// <param name="volatileGasInventory"></param>
		/// <param name="planetRadius">Radius in km</param>
		/// <returns>Fraction of the planet covered in water</returns>
		public double GetWaterFraction(double volatileGasInventory, double planetRadius);

		/// <summary>
		/// Returns the fraction of cloud cover available.
		/// </summary>
		/// <remarks>
		/// 
		/// </remarks>
		public double GetCloudFraction(double surfaceTemp, double smallestMWRetained, double equatorialRadius, double hydroFraction);

		/// <summary>
		/// Given the surface temperature of a planet (in Kelvin), this function
		///	returns the fraction of the planet's surface covered by ice.
		/// </summary>
		/// <returns>Fraction of the planet's surface covered in ice</returns>
		public double GetIceFraction(double hydroFraction, double surfTemp);

		/// <summary>
		/// Calculates the albedo of a planetary body.
		/// </summary>
		/// <param name="waterFraction">Fraction of surface covered by water.</param>
		/// <param name="cloudFraction">Fraction of planet covered by clouds.</param>
		/// <param name="iceFraction">Fraction of planet covered by ice.</param>
		/// <param name="surfPressure">Surface pressure in mb.</param>
		/// <returns>Average overall albedo of the body.</returns>
		public double GetAlbedo(double waterFraction, double cloudFraction, double iceFraction, double surfPressure);

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