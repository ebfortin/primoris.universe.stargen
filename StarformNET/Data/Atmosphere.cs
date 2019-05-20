using System.Collections.Generic;
using System;


namespace Primoris.Universe.Stargen.Data
{
    [Serializable]
    public class Atmosphere
    {
        public double SurfacePressure { get; set; }

        public Breathability Breathability { get; set; }

        public List<Gas> Composition { get; set; }

        public List<Gas> PoisonousGases { get; set; }

        public Atmosphere()
        {
            Composition = new List<Gas>();
            PoisonousGases = new List<Gas>();
        }
    }
}
