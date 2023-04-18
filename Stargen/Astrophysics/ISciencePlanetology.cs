using Primoris.Universe.Stargen.Bodies;
using UnitsNet;

namespace Primoris.Universe.Stargen.Astrophysics;

public interface ISciencePlanetology
{
	/// <summary>
	/// Returns the type of the body given certain characteristics.
	/// </summary>
	/// <param name="massSM">Mass of the body,</param>
	/// <param name="gasMassSM">Gas mass of the body.</param>
	/// <param name="molecularWeightRetained">Molecular weight (mass) retained.</param>
	/// <param name="surfacePressure">Pressure at the surface of the body, for body with atmosphere.</param>
	/// <param name="waterCoverFraction">Pourcentage of water covering the surface.</param>
	/// <param name="iceCoverFraction">Pourcentage of ice covering the surface.</param>
	/// <param name="maxTemperature">Maximal temperature at the surface.</param>
	/// <param name="boilingPointWater">Boiling temperature of water.</param>
	/// <param name="surfaceTemperature">Mean surface temperature.</param>
	/// <returns>Body type.</returns>
	BodyType GetBodyType(Mass massSM,
				   Mass gasMassSM,
				   Mass molecularWeightRetained,
				   Pressure surfacePressure,
				   Ratio waterCoverFraction,
				   Ratio iceCoverFraction,
				   Temperature maxTemperature,
				   Temperature boilingPointWater,
				   Temperature surfaceTemperature);

	/// <summary>
	/// Estimates of the SatelliteBody core radius given a mass and orbitzone.
	/// </summary>
	/// <param name="massSM">Mass of the body.</param>
	/// <param name="orbitZone">Orbit zone of the body.</param>
	/// <param name="giant">True if the body is a gas giant.</param>
	/// <returns></returns>
	Length GetCoreRadius(Mass massSM, int orbitZone, bool giant);

        /// <summary>
        /// Calculates the radius of a planet.
        /// </summary>
        /// <param name="mass">Mass of the body.</param>
        /// <param name="density">Density of the whole body.</param>
        /// <returns>Radius of the body.</returns>
        Length GetRadius(Mass mass, Density density);

        /// <summary>
        /// Returns the dimensionless quantity of optical depth, which is useful in determing the amount
        /// of greenhouse effect on a planet.
        /// </summary>
        /// <param name="molecularWeight">Molecular wieght retained.</param>
        /// <param name="surfPressure">Surface pressure on the body with an atmosphere.</param>
        /// <returns></returns>
        Ratio GetOpacity(Mass molecularWeight, Pressure surfPressure);


	/// <summary>
	/// Given the volatile gas inventory and
	/// planetary radius of a planet, this function returns the
	/// fraction of the planet covered with water.	
	/// </summary>
	/// <param name="volatileGasInventory">Unitless volatile gas inventory.</param>
	/// <param name="planetRadius">Radius of the body.</param>
	/// <returns>Fraction of the planet covered in water.</returns>
	Ratio GetWaterFraction(Ratio volatileGasInventory, Length planetRadius);

	/// <summary>
	/// Returns the fraction of cloud cover available.
	/// </summary>
	/// <param name="surfaceTemp">Mean surface temperature.</param>
	/// <param name="smallestMWRetained">Smallest molecular weight retained.</param>
	/// <param name="equatorialRadius">Radius of the body.</param>
	/// <param name="hydroFraction"></param>
	/// <returns></returns>
	Ratio GetCloudFraction(Temperature surfaceTemp, Mass smallestMWRetained, Length equatorialRadius, Ratio hydroFraction);

	/// <summary>
	/// Given the surface temperature of a planet (in Kelvin), this function
	///	returns the fraction of the planet's surface covered by ice.
	/// </summary>
	/// <param name="hydroFraction">Fraction of the body covered with water.</param>
	/// <param name="surfTemp">Surface temperature of the body.</param>
	/// <returns>Fraction of the planet's surface covered in ice.</returns>
	Ratio GetIceFraction(Ratio hydroFraction, Temperature surfTemp);

	/// <summary>
	/// Calculates the albedo of a planetary body.
	/// </summary>
	/// <param name="waterFraction">Fraction of surface covered by water.</param>
	/// <param name="cloudFraction">Fraction of planet covered by clouds.</param>
	/// <param name="iceFraction">Fraction of planet covered by ice.</param>
	/// <param name="surfPressure">Surface pressure.</param>
	/// <returns>Average overall albedo of the body.</returns>
	Ratio GetAlbedo(Ratio waterFraction, Ratio cloudFraction, Ratio iceFraction, Pressure surfPressure);

	/// <summary>
	/// Determine if the body has a greenhouse effect.
	/// </summary>
	/// <param name="ecosphereRadius">Ecophere radius of the parent StellarBody.</param>
	/// <param name="semiAxisMajorAU">Smie major axis of the orbit of the body.</param>
	/// <returns>True if there's a greenhouse effect.</returns>
	bool TestHasGreenhouseEffect(Length ecosphereRadius, Length semiAxisMajorAU);

	/// <summary>
	/// Checks if a planet's rotation is in resonance with its orbit
	/// </summary>
	/// <param name="angularVelocity">The planet's angular velocity.</param>
	/// <param name="dayInHours">The length of the planet.</param>
	/// <param name="orbitalPeriodDays">The orbital period of the planet.</param>
	/// <param name="ecc">The eccentricity of the planet's orbit</param>
	/// <returns>True if the planet is in a resonant orbit.</returns>
	bool TestHasResonantPeriod(RotationalSpeed angularVelocityRadSec,
						 Duration dayLength,
						 Duration orbitalPeriod,
						 Ratio eccentricity);
	
	/// <summary>
	/// Checks if a planet is similar to Earth.
	/// </summary>
	/// <param name="surfaceTemperature">Mean surface temperature of the body.</param>
	/// <param name="waterCoverFraction">Fraction of the surface covered with water.</param>
	/// <param name="cloudCoverFraction">Fraction of the atmosphere covered with clouds.</param>
	/// <param name="iceCoverFraction">Fraction of the surface covered with ice.</param>
	/// <param name="surfacePressure">Surface pressure of the atmosphere of the body.</param>
	/// <param name="surfaceGravityG">Surface acceleration of the body.</param>
	/// <param name="breathability">Breathability of the body's atmosphere.</param>
	/// <param name="type">Type of body.</param>
	/// <returns>True if the planet is earth like.</returns>
	bool TestIsEarthLike(Temperature surfaceTemperature,
				   Ratio waterCoverFraction,
				   Ratio cloudCoverFraction,
				   Ratio iceCoverFraction,
				   Pressure surfacePressure,
				   Acceleration surfaceGravityG,
				   Breathability breathability,
				   BodyType type);
	
	/// <summary>
	/// Checks if a planet is a gas giant.
	/// </summary>
	/// <param name="massSM">Total mass of the body.</param>
	/// <param name="gasMassSM">Gas mass of the body,</param>
	/// <param name="molecularWeightRetained">Molecular weight retained by the body.</param>
	/// <returns>True is the body is a gas giant.</returns>
	bool TestIsGasGiant(Mass massSM,
				  Mass gasMassSM,
				  Mass molecularWeightRetained);
	
	/// <summary>
	/// Checks if a body is habitable by humans.
	/// </summary>
	/// <param name="dayLength">Length of a day.</param>
	/// <param name="orbitalPeriod">Length of a complete orbit..</param>
	/// <param name="breathability">Breathability of the atmosphere.</param>
	/// <param name="hasResonantPeriod">True if there's a resonant period.</param>
	/// <param name="isTidallyLocked">True if the body is tidally locked.</param>
	/// <returns>True if the body is habitable y humans.</returns>
	bool TestIsHabitable(Duration dayLength,
				   Duration orbitalPeriod,
				   Breathability breathability,
				   bool hasResonantPeriod,
				   bool isTidallyLocked);
	
	/// <summary>
	/// Checks if a body is tidally locked.
	/// </summary>
	/// <param name="dayLength">Length of a single day.</param>
	/// <param name="orbitalPeriod">Length of a complete orbit.</param>
	/// <returns>True if the body is tidally locked.</returns>
	bool TestIsTidallyLocked(Duration dayLength,
					   Duration orbitalPeriod);
}

