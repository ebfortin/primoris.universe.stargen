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

                // Now we're ready to test for habitable planets,
                // so we can count and log them and such
                CheckPlanet(planet, planet_id, false);
                
                for (var m = 0; m < planet.Moons.Count; m++)
                {
                    string moon_id = String.Format("{0}.{1}", planet_id, m);
                    CheckPlanet(planet.Moons[m], moon_id, true);
                }
            }

            return planets;
        }

        // TODO this really should be in a separate class
        public static void CalculateGases(Planet planet, ChemType[] gasTable)
        {
            var sun = planet.Star;
            planet.Atmosphere.Composition = new List<Gas>();

            if (!(planet.Atmosphere.SurfacePressure > 0))
            {
                return;
            }

            double[] amount = new double[gasTable.Length];
            double totamount = 0;
            double pressure = planet.Atmosphere.SurfacePressure / GlobalConstants.MILLIBARS_PER_BAR;
            int n = 0;

            // Determine the relative abundance of each gas in the planet's atmosphere
            for (var i = 0; i < gasTable.Length; i++)
            {
                double yp = gasTable[i].Boil / (373.0 * ((Math.Log((pressure) + 0.001) / -5050.5) + (1.0 / 373.0)));

                // TODO move both of these conditions to separate methods
                if ((yp >= 0 && yp < planet.NighttimeTempKelvin) && (gasTable[i].Weight >= planet.MolecularWeightRetained))
                {
                    double abund, react;
                    CheckForSpecialRules(out abund, out react, pressure, planet, gasTable[i]);

                    double vrms = Environment.RMSVelocity(gasTable[i].Weight, planet.ExosphereTempKelvin);
                    double pvrms = Math.Pow(1 / (1 + vrms / planet.EscapeVelocityCMSec), sun.AgeYears / 1e9);

                    double fract = (1 - (planet.MolecularWeightRetained / gasTable[i].Weight));

                    // Note that the amount calculated here is unitless and doesn't really mean
                    // anything except as a relative value
                    amount[i] = abund * pvrms * react * fract;
                    totamount += amount[i];
                    if (amount[i] > 0.0)
                    {
                        n++;
                    }
                }
                else
                {
                    amount[i] = 0.0;
                }
            }

            // For each gas present, calculate its partial pressure
            if (n > 0)
            {
                planet.Atmosphere.Composition = new List<Gas>();

                n = 0;
                for (var i = 0; i < gasTable.Length; i++)
                {
                    if (amount[i] > 0.0)
                    {
                        planet.Atmosphere.Composition.Add(
                            new Gas(gasTable[i], planet.Atmosphere.SurfacePressure * amount[i] / totamount));
                    }
                }
            }
            
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

        // TODO This should be moved out of this class entirely
        private static void CheckPlanet(Planet planet, string planetID, bool is_moon)
        {
            planet.Illumination = Environment.MinimumIllumination(planet.SemiMajorAxisAU, planet.Star.Luminosity);
            planet.Atmosphere.Breathability = Environment.Breathability(planet);
            planet.IsHabitable = Environment.IsHabitable(planet);
            planet.IsEarthlike = Environment.IsEarthlike(planet);
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