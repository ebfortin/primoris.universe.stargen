using CsvHelper.Configuration.Attributes;

using Primoris.Universe.Stargen.Astrophysics.Burrows;
using Primoris.Universe.Stargen.Bodies;

namespace Primoris.Universe.Stargen.Astrophysics;

/// <summary>
/// Main interface of science providers.
/// </summary>
public interface IScienceAstrophysics
{
	static IScienceAstrophysics Default { get; } = new BodyPhysics();

	IScienceAstronomy Astronomy { get; }

	IScienceDynamics Dynamics { get; }

	ISciencePhysics Physics { get; }

	ISciencePlanetology Planetology { get; }

	IScienceThermodynamics Thermodynamics { get; }
}