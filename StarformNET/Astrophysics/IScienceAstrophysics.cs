using Primoris.Universe.Stargen.Bodies;

namespace Primoris.Universe.Stargen.Astrophysics
{
	public interface IScienceAstrophysics 
	{
		IScienceAstronomy Astronomy { get; }
		IScienceDynamics Dynamics { get; }
		ISciencePhysics Physics { get; }
		ISciencePlanetology Planetology { get; }
		IScienceThermodynamics Thermodynamics { get; }
	}
}