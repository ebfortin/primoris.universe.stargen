namespace Primoris.Universe.Stargen
{
    using Data;

    public class SystemGenerationOptions
    {
        public static SystemGenerationOptions DefaultOptions = new SystemGenerationOptions();

        public double DustDensityCoeff = GlobalConstants.DUST_DENSITY_COEFF;
        public double CloudEccentricity = GlobalConstants.CLOUD_ECCENTRICITY;
        public double GasDensityRatio = GlobalConstants.K;

        public ChemType[] GasTable = new ChemType[0];
    }
}
