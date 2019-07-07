using Primoris.Universe.Stargen.Data;

namespace Primoris.Universe.Stargen.Physics
{
	public interface IBodyPhysics
	{
		double GetAngularVelocity(double massSM, double radiusKM, double densityGCC, double semiMajorAxisAU, bool isGasGiant, double starMassSM, double starAgeYears);
		double GetBoilingPointWater(double surfpres);
		double GetDayLength(double angularVelocityRadSec, double orbitalPeriod, double eccentricity);
		double GetDensityFromBody(double massSM, double radius);
		double GetDensityFromStar(double massSM, double semiMajorAxisAU, double ecosphereRadiusAU, bool isGasGiant);
		double GetEscapeVelocity(double massSM, double radius);
		double GetExosphereTemperature(double semiMajorAxisAu, double ecosphereRadiusAU, double sunTemperature);
		double GetHillSphere(double sunMass, double massSM, double semiMajorAxisSM);
		double GetMolecularWeightRetained(double surfGrav, double mass, double radius, double exosphereTemp, double sunAge);
		BodyType GetBodyType(double massSM, double gasMassSM, double molecularWeightRetained, double surfacePressure, double waterCoverFraction, double iceCoverFraction, double maxTemperature, double boilingPointWater, double surfaceTemperature);
		double GetRadius(double massSM, int orbitZone);
		double GetRMSVelocityCMSec(double exoTemp);
		double GetSurfacePressure(double volatileGasInventory, double radius, double surfaceGravity);
		double GetVolatileGasInventory(double massSM, double escapeVelocity, double rmsVelocity, double sunMass, double gasMassSM, int orbitZone, bool hasGreenhouse, bool hasAccretedGas);
		bool TestHasGreenhouseEffect(double ecosphereRadius, double semiAxisMajorAU);
		bool TestHasResonantPeriod(double angularVelocityRadSec, double dayLength, double orbitalPeriod, double eccentricity);
		bool TestIsGasGiant(double massSM, double gasMassSM, double molecularWeightRetained);
		bool TestIsTidallyLocked(double dayLength, double orbitalPeriod);
	}
}