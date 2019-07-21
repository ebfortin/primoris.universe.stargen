using System;
using System.Text;
using UnitsNet;


namespace Primoris.Universe.Stargen.Astrophysics
{
	[Serializable]
	public class Gas
	{
		public ChemType GasType { get; private set; }
		public Pressure SurfacePressure { get; private set; }

		public Gas(ChemType gType, Pressure pressure)
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
