
using System;
using System.Collections.Generic;
using Primoris.Universe.Stargen.Bodies;

namespace Primoris.Universe.Stargen.Data
{


	[Serializable]
    public class StellarSystem
    {
        public string Name { get; set; }
        public Star Star { get; set; }
        public IEnumerable<Body> Planets { get; set; }
        public SystemGenerationOptions Options { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
