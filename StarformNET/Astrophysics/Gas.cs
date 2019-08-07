using System;
using System.Text;
using UnitsNet;


namespace Primoris.Universe.Stargen.Astrophysics
{
	[Serializable]
	public class Gas : Molecule
	{
		public Pressure SurfacePressure { get; protected set; }

		public Gas(Chemical gType, Pressure pressure)
		{
			Chemical = gType;
			SurfacePressure = pressure;
		}

		public override string ToString()
		{
			return Chemical.DisplaySymbol + "[" + SurfacePressure + "]";
		}
	}
}
