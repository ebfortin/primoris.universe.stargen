using System;
using System.Text;


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

		public override string ToString()
		{
			return GasType.DisplaySymbol + "[" + SurfacePressure.ToString("F5") + "]";
		}
	}
}
