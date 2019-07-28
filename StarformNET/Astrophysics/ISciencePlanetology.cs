using Primoris.Universe.Stargen.Bodies;
using UnitsNet;

namespace Primoris.Universe.Stargen.Astrophysics
{
	public interface ISciencePlanetology
	{
		BodyType GetBodyType(Mass massSM,
					   Mass gasMassSM,
					   Mass molecularWeightRetained,
					   Pressure surfacePressure,
					   Ratio waterCoverFraction,
					   Ratio iceCoverFraction,
					   Temperature maxTemperature,
					   Temperature boilingPointWater,
					   Temperature surfaceTemperature);

		Length GetCoreRadius(Mass massSM, int orbitZone, bool giant);

        /// <summary>
        /// Calculates the radius of a planet.
        /// </summary>
        /// <param name="mass">Mass in units of solar masses</param>
        /// <param name="density">Density in units of grams/cc</param>
        /// <returns>Radius in units of km</returns>
        Length GetRadius(Mass mass, Density density);

        /// <summary>
        /// Returns the dimensionless quantity of optical depth, which is useful in determing the amount
        /// of greenhouse effect on a planet.
        /// </summary>
        /// <param name="molecularWeight"></param>
        /// <param name="surfPressure"></param>
        /// <returns></returns>
        Ratio GetOpacity(Mass molecularWeight, Pressure surfPressure);


		/// <summary>
		/// Given the volatile gas inventory and
		/// planetary radius of a planet (in Km), this function returns the
		/// fraction of the planet covered with water.	
		/// </summary>
		/// <param name="volatileGasInventory"></param>
		/// <param name="planetRadius">Radius in km</param>
		/// <returns>Fraction of the planet covered in water</returns>
		Ratio GetWaterFraction(Ratio volatileGasInventory, Length planetRadius);

		/// <summary>
		/// Returns the fraction of cloud cover available.
		/// </summary>
		/// <remarks>
		/// 
		/// </remarks>
		Ratio GetCloudFraction(Temperature surfaceTemp, Mass smallestMWRetained, Length equatorialRadius, Ratio hydroFraction);

		/// <summary>
		/// Given the surface temperature of a planet (in Kelvin), this function
		///	returns the fraction of the planet's surface covered by ice.
		/// </summary>
		/// <returns>Fraction of the planet's surface covered in ice</returns>
		Ratio GetIceFraction(Ratio hydroFraction, Temperature surfTemp);

		/// <summary>
		/// Calculates the albedo of a planetary body.
		/// </summary>
		/// <param name="waterFraction">Fraction of surface covered by water.</param>
		/// <param name="cloudFraction">Fraction of planet covered by clouds.</param>
		/// <param name="iceFraction">Fraction of planet covered by ice.</param>
		/// <param name="surfPressure">Surface pressure in mb.</param>
		/// <returns>Average overall albedo of the body.</returns>
		Ratio GetAlbedo(Ratio waterFraction, Ratio cloudFraction, Ratio iceFraction, Pressure surfPressure);

		bool TestHasGreenhouseEffect(Length ecosphereRadius, Length semiAxisMajorAU);

		/// <summary>
		/// Checks if a planet's rotation is in resonance with its orbit
		/// </summary>
		/// <param name="angularVelocity">The planet's angular velocity in radians/sec</param>
		/// <param name="dayInHours">The length of the planet's day in hours</param>
		/// <param name="orbitalPeriodDays">The orbital period of the planet in days</param>
		/// <param name="ecc">The eccentricity of the planet's orbit</param>
		/// <returns>True if the planet is in a resonant orbit</returns>
		bool TestHasResonantPeriod(RotationalSpeed angularVelocityRadSec,
							 Duration dayLength,
							 Duration orbitalPeriod,
							 Ratio eccentricity);
		bool TestIsEarthLike(Temperature surfaceTemperature,
					   Ratio waterCoverFraction,
					   Ratio cloudCoverFraction,
					   Ratio iceCoverFraction,
					   Pressure surfacePressure,
					   Acceleration surfaceGravityG,
					   Breathability breathability,
					   BodyType type);
		bool TestIsGasGiant(Mass massSM,
					  Mass gasMassSM,
					  Mass molecularWeightRetained);
		bool TestIsHabitable(Duration dayLength,
					   Duration orbitalPeriod,
					   Breathability breathability,
					   bool hasResonantPeriod,
					   bool isTidallyLocked);
		bool TestIsTidallyLocked(Duration dayLength,
						   Duration orbitalPeriod);
	}
}