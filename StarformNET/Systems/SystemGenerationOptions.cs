using System;
using System.Linq;
using Primoris.Universe.Stargen.Astrophysics;

namespace Primoris.Universe.Stargen.Systems
{

	[Obsolete]
	public class SystemGenerationOptions
	{
		public static SystemGenerationOptions DefaultOptions = new SystemGenerationOptions();

		public SystemGenerationOptions(double dust = GlobalConstants.DUST_DENSITY_COEFF,
									   double ecc = GlobalConstants.CLOUD_ECCENTRICITY,
									   double dens = GlobalConstants.K)
		{
			DustDensityCoeff = dust;
			CloudEccentricity = ecc;
			GasDensityRatio = dens;

			GasTable = Chemical.All.Values.ToArray();
		}

		public double DustDensityCoeff { get; set; }
		public double CloudEccentricity { get; set; }
		public double GasDensityRatio { get; set; }

		public Chemical[] GasTable { get; set; } = new Chemical[0];
	}
}
