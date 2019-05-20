using System;


namespace Primoris.Universe.Stargen.Data
{
    [Serializable]
    public class Gas
    {
        public ChemType GasType { get; set; }
        public double SurfacePressure { get; set; }

        public Gas(ChemType gType, double pressure)
        {
            GasType = gType;
            SurfacePressure = pressure;
        }
    }
}
