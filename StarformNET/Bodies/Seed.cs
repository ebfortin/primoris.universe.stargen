using System;
using System.Collections.Generic;
using UnitsNet;


namespace Primoris.Universe.Stargen.Bodies
{
	public class Seed
	{
		//public Seed NextBody { get; set; } = null;
		//public Seed FirstSatellite { get; set; } = null;

		public IEnumerable<Seed> Satellites { get; set; } = new Seed[0];

		public Length SemiMajorAxis { get; set; } = Length.Zero;
		public Ratio Eccentricity { get; set; } = Ratio.Zero;
		public Mass Mass { get; set; } = Mass.Zero;
		public Mass DustMass { get; set; } = Mass.Zero;
		public Mass GasMass { get; set; } = Mass.Zero;
		public bool IsGasGiant { get; set; } = false;

		public Seed(Length a, Ratio e, Mass mass, Mass dMass, Mass gMass)
		{
			SemiMajorAxis = a;
			Eccentricity = e;
			Mass = mass;
			DustMass = dMass;
			GasMass = gMass;
		}
	}
}