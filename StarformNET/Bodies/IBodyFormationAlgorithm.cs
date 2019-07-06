using System.Collections.Generic;

namespace Primoris.Universe.Stargen.Bodies
{
	public interface IBodyFormationAlgorithm
	{
		double CloudEccentricity { get; set; }
		double GasDustRatio { get; set; }

		List<BodySeed> CreateBodies(double stellarMassRatio,
								double stellarLumRatio,
								double innerDust,
								double outerDust,
								double outerPlanetLimit,
								double dustDensityCoeff,
								BodySeed seedSystem);
	}
}