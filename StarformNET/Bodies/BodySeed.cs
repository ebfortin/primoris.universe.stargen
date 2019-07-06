namespace Primoris.Universe.Stargen.Bodies
{
	// TODO: Transform to collection and enumerators. 
	public class BodySeed
	{
		public BodySeed NextBody { get; set; } = null;
		public BodySeed FirstSatellite { get; set; } = null;

		public double SemiMajorAxisAU { get; set; }
		public double Eccentricity { get; set; }
		public double Mass { get; set; }
		public double DustMass { get; set; }
		public double GasMass { get; set; }
		public bool IsGasGiant { get; set; } = false;

		public BodySeed(double a, double e, double mass, double dMass, double gMass)
		{
			SemiMajorAxisAU = a;
			Eccentricity = e;
			Mass = mass;
			DustMass = dMass;
			GasMass = gMass;
		}
	}
}