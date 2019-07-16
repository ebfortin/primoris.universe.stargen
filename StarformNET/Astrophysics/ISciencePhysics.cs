namespace Primoris.Universe.Stargen.Astrophysics
{
	public interface ISciencePhysics
	{
		double GetDensityFromBody(double massSM, double radius);
		double GetDensityFromStar(double massSM, double semiMajorAxisAU, double ecosphereRadiusAU, bool isGasGiant);
		double GetMolecularWeightRetained(double surfGrav, double mass, double radius, double exosphereTemp, double sunAge);
		double GetRMSVelocityCMSec(double exoTemp);
		double GetRocheLimit(double bodyRadius, double bodyDensity, double satelliteDensity);
		double GetSurfacePressure(double volatileGasInventory, double radius, double surfaceGravity);
		double GetVolatileGasInventory(double massSM, double escapeVelocity, double rmsVelocity, double sunMass, double gasMassSM, int orbitZone, bool hasGreenhouse);
	}
}