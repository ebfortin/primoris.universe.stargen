using System.Collections.Generic;
using UnitsNet;

namespace Primoris.Universe.Stargen.Bodies
{
	public interface IBodyFormationAlgorithm
	{
		double CloudEccentricity { get; set; }
		double GasDustRatio { get; set; }

		IEnumerable<Seed> CreateSeeds(Mass stellarMassRatio,
								Luminosity stellarLumRatio,
								Length innerDust,
								Length outerDust,
								Length outerPlanetLimit,
								Ratio dustDensityCoeff,
								Length semiMajorAxisAU,
								Ratio ecc);
	}
}