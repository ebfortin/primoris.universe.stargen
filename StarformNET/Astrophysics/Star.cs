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
        public Star() : base()
        {
        }

        public Star(Mass mass) : base(mass)
        {
        }

        public Star(StellarType st) : base(st)
        {
        }

        public Star(StellarType st, string name) : base(st, name)
        {
        }

        public Star(Mass mass, Luminosity lum, Duration age) : base(mass, lum, age)
        {
        }

        public override int Position { get => 0; protected set { } }

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
