namespace Primoris.Universe.Stargen.Astrophysics
{
	public interface IScienceThermodynamics
	{
		double GetBoilingPointWater(double surfpres);
		double GetExosphereTemperature(double semiMajorAxisAu, double ecosphereRadiusAU, double sunTemperature);
	}
}