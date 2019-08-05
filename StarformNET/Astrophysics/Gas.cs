using System;
using System.Text;
using UnitsNet;


namespace Primoris.Universe.Stargen.Astrophysics
{
	[Serializable]
	public class Gas
	{
		public Chemical GasType { get; private set; }
		public Pressure SurfacePressure { get; private set; }

		public Gas(Chemical gType, Pressure pressure)
		{
			GasType = gType;
			SurfacePressure = pressure;
		}

		public override string ToString()
		{
			return GasType.DisplaySymbol + "[" + SurfacePressure + "]";
		}
	}
}
