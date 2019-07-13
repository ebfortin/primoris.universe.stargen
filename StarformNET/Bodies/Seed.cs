using System;
using System.Collections.Generic;


namespace Primoris.Universe.Stargen.Bodies
{
	// TODO: Transform to collection and enumerators. 
	public class Seed
	{
		//public Seed NextBody { get; set; } = null;
		//public Seed FirstSatellite { get; set; } = null;

		public IEnumerable<Seed> Satellites { get; set; } = new Seed[0];

		public double SemiMajorAxisAU { get; set; }
		public double Eccentricity { get; set; }
		public double Mass { get; set; }
		public double DustMass { get; set; }
		public double GasMass { get; set; }
		public bool IsGasGiant { get; set; } = false;

		public Seed(double a, double e, double mass, double dMass, double gMass)
		{
			SemiMajorAxisAU = a;
			Eccentricity = e;
			Mass = mass;
			DustMass = dMass;
			GasMass = gMass;
		}
	}
}