using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Primoris.Universe.Stargen.Bodies;

public class NullBodyFormationAlgorithm : IBodyFormationAlgorithm
{
	public Ratio DustDensityCoefficient => Ratio.Zero;

	public Ratio CloudEccentricity => Ratio.Zero;

	public Ratio GasDustRatio => Ratio.Zero;

	public IEnumerable<Seed> CreateSeeds(Mass stellarMassRatio, Luminosity stellarLumRatio, Length innerDust, Length outerDust, Length outerPlanetLimit, Length semiMajorAxisAU)
	{
		return Array.Empty<Seed>();
	}
}
