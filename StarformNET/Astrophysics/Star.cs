using System;
using System.Collections.Generic;
using System.Text;
using Primoris.Universe.Stargen.Bodies;
using UnitsNet;
using Primoris.Universe.Stargen.Systems;
using Primoris.Universe.Stargen.Services;


namespace Primoris.Universe.Stargen.Astrophysics
{
    public class Star : StellarBody
    {
        public Star() : base() { }
        public Star(IScienceAstrophysics phy) : base(phy) { }

        public Star(Mass mass) : base(mass) { }
        public Star(IScienceAstrophysics phy, Mass mass) : base(phy, mass) { }

        public Star(StellarType st) : base(st) { }
        public Star(IScienceAstrophysics phy, StellarType st) : base(phy, st) { }

        public Star(StellarType st, string name) : base(st, name) { }
        public Star(IScienceAstrophysics phy, StellarType st, string name) : base(phy, st, name) { }

		public Star(Mass mass, Luminosity lum, Duration age) : base(mass, lum, age) { }
        public Star(IScienceAstrophysics phy, Mass mass, Luminosity lum, Duration age) : base(phy, mass, lum, age) { }

        protected override IEnumerable<SatelliteBody> GenerateSatellites(IEnumerable<Seed> seeds, CreateSatelliteBodyDelegate createFunc)
        {
            var planets = new List<SatelliteBody>();
            var i = 0;
            foreach (var seed in seeds)
            {
                var planetNo = i + 1; // start counting planets at 1
                i += 1;

                string planet_id = planetNo.ToString();

                var planet = createFunc(seed, this, i, planet_id);
                planets.Add(planet);
            }

            return planets;
        }

    }
}
