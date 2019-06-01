namespace Primoris.Universe.Stargen
{
    using Data;

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

			GasTable = ChemType.Load();
		}

        public double DustDensityCoeff { get; set; }
        public double CloudEccentricity { get; set; }
        public double GasDensityRatio { get; set; }

        public ChemType[] GasTable = new ChemType[0];
    }
}
