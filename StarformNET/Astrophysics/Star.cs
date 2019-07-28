using System;
using System.Collections.Generic;
using System.Text;
using Primoris.Universe.Stargen.Bodies;
using UnitsNet;
using Primoris.Universe.Stargen.Systems;


namespace Primoris.Universe.Stargen.Astrophysics
{
    public class Star : StellarBody
    {
        public Star(IScienceAstrophysics phy) : base(phy)
        {
        }

        public Star(IScienceAstrophysics phy, Mass mass) : base(phy, mass)
        {
        }

        public Star(IScienceAstrophysics phy, StellarType st) : base(phy, st)
        {
        }

        public Star(IScienceAstrophysics phy, StellarType st, string name) : base(phy, st, name)
        {
        }

        public Star(IScienceAstrophysics phy, Mass mass, Luminosity lum, Duration age) : base(phy, mass, lum, age)
        {
        }

        protected override IEnumerable<SatelliteBody> GenerateSatellites(IEnumerable<Seed> seeds, CreateSatelliteBodyDelegate createFunc, bool useRandomTilt, SystemGenerationOptions genOptions)
        {
            var planets = new List<SatelliteBody>();
            var i = 0;
            foreach (var seed in seeds)
            {
                var planetNo = i + 1; // start counting planets at 1
                i += 1;

                string planet_id = planetNo.ToString();

                var planet = createFunc(seed, this, useRandomTilt, planet_id, genOptions);
                planets.Add(planet);
            }

            return planets;
        }

    }
}
