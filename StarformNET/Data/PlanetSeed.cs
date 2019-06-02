namespace Primoris.Universe.Stargen.Data
{
    public class PlanetSeed
    {
        public PlanetSeed NextPlanet { get; set; } = null;
        public PlanetSeed FirstMoon { get; set; } = null;

        public double SemiMajorAxisAU { get; set; }
        public double Eccentricity { get; set; }
        public double Mass { get; set; }
        public double DustMass { get; set; }
        public double GasMass { get; set; }
        public bool IsGasGiant { get; set; } = false;

        public PlanetSeed(double a, double e, double mass, double dMass, double gMass)
        {
            SemiMajorAxisAU = a;
            Eccentricity = e;
            Mass = mass;
            DustMass = dMass;
            GasMass = gMass;
        }
    }
}