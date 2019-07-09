using System;
using System.Text;


namespace Primoris.Universe.Stargen.Astrophysics
{
	[Serializable]
	public class Gas
	{
		public ChemType GasType { get; private set; }
		public double SurfacePressure { get; private set; }

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
