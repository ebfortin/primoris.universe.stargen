using System.Collections.Generic;
using UnitsNet;

namespace Primoris.Universe.Stargen.Bodies
{
	public interface IBodyFormationAlgorithm
	{
		Ratio DustDensityCoefficient { get; }
		Ratio CloudEccentricity { get; }
		Ratio GasDustRatio { get; }

		IEnumerable<Seed> CreateSeeds(Mass stellarMassRatio,
								Luminosity stellarLumRatio,
								Length innerDust,
								Length outerDust,
								Length outerPlanetLimit,
								Length semiMajorAxisAU);
	}
}