using System;
using System.Collections.Generic;
using Primoris.Universe.Stargen.Data;


namespace Primoris.Universe.Stargen
{

    
    public class Generator
    {
        public static StellarGroup GenerateStellarGroup(int seed, int numSystems, SystemGenerationOptions genOptions = null)
        {
            Utilities.InitRandomSeed(seed);
            genOptions = genOptions ?? SystemGenerationOptions.DefaultOptions;
            var group = new StellarGroup() { Seed = seed, GenOptions = genOptions, Systems = new List<StellarSystem>() };
            for (var i = 0; i < numSystems; i++)
            {
                var name = String.Format("System {0}", i);
                group.Systems.Add(GenerateStellarSystem(name, genOptions));
            }
            return group;
        }

        public static StellarSystem GenerateStellarSystem(string systemName, SystemGenerationOptions genOptions = null, Star sun=null, List<PlanetSeed> seedSystem=null)
        {
            genOptions = genOptions ?? SystemGenerationOptions.DefaultOptions;
            sun = sun ?? new Star();
            var useRandomTilt = seedSystem == null;

            var accrete = new Accrete(genOptions.CloudEccentricity, genOptions.GasDensityRatio);
            double outer_planet_limit = GetOuterLimit(sun);
            double outer_dust_limit = GetStellarDustLimit(sun.Mass);
            seedSystem = seedSystem ?? accrete.GetPlanetaryBodies(sun.Mass, 
                sun.Luminosity, 0.0, outer_dust_limit, outer_planet_limit,
                genOptions.DustDensityCoeff, null, true);
            
            var planets = GeneratePlanets(sun, seedSystem, useRandomTilt, genOptions);
            return new StellarSystem()
            {
                Options = genOptions,
                Planets = planets,
                Name = systemName,
                Star = sun
            };
        }

        private static List<Planet> GeneratePlanets(Star sun, List<PlanetSeed> seeds, bool useRandomTilt, SystemGenerationOptions genOptions)
        {
            var planets = new List<Planet>();
            for (var i = 0; i < seeds.Count; i++)
            {
                var planetNo = i + 1; // start counting planets at 1
                var seed = seeds[i];

                string planet_id = planetNo.ToString();

                var planet = new Planet(seed, sun, planetNo, useRandomTilt, planet_id, false, genOptions);
                planets.Add(planet);
            }

            return planets;
        }

        private static void CheckForSpecialRules(out double abund, out double react, double pressure, Planet planet, ChemType gas)
        {
            var sun = planet.Star;
            var pres2 = 1.0;
            abund = gas.Abunds;

            if (gas.Symbol == "Ar")
            {
                react = .15 * sun.AgeYears / 4e9;
            }
            else if (gas.Symbol == "He")
            {
                abund = abund * (0.001 + (planet.GasMassSM / planet.MassSM));
                pres2 = (0.75 + pressure);
                react = Math.Pow(1 / (1 + gas.Reactivity), sun.AgeYears / 2e9 * pres2);
            }
            else if ((gas.Symbol == "O" || gas.Symbol == "O2") && sun.AgeYears > 2e9 && planet.SurfaceTempKelvin > 270 && planet.SurfaceTempKelvin < 400)
            {
                // pres2 = (0.65 + pressure/2); // Breathable - M: .55-1.4
                pres2 = (0.89 + pressure / 4);  // Breathable - M: .6 -1.8
                react = Math.Pow(1 / (1 + gas.Reactivity), Math.Pow(sun.AgeYears / 2e9, 0.25) * pres2);
            }
            else if (gas.Symbol == "CO2" && sun.AgeYears > 2e9 && planet.SurfaceTempKelvin > 270 && planet.SurfaceTempKelvin < 400)
            {
                pres2 = (0.75 + pressure);
                react = Math.Pow(1 / (1 + gas.Reactivity), Math.Pow(sun.AgeYears / 2e9, 0.5) * pres2);
                react *= 1.5;
            }
            else
            {
                pres2 = (0.75 + pressure);
                react = Math.Pow(1 / (1 + gas.Reactivity), sun.AgeYears / 2e9 * pres2);
            }
        }

        private static double GetStellarDustLimit(double stellarMassRatio)
        {
            return (200.0 * Math.Pow(stellarMassRatio, (1.0 / 3.0)));
        }

        private static double GetOuterLimit(Star star)
        {
            if (star.BinaryMass < .001)
            {
                return 0.0;
            }

            // The following is Holman & Wiegert's equation 1 from
            // Long-Term Stability of Planets in Binary Systems
            // The Astronomical Journal, 117:621-628, Jan 1999
            double m1 = star.Mass;
            double m2 = star.BinaryMass;
            double mu = m2 / (m1 + m2);
            double e = star.SemiMajorAxisAU;
            double e2 = Utilities.Pow2(e);
            double a = star.Eccentricity;

            return (0.464 + (-0.380 * mu) + (-0.631 * e) + (0.586 * mu * e) + (0.150 * e2) + (-0.198 * mu * e2)) * a;
        }
    }
}