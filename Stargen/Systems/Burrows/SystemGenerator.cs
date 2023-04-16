using System;
using System.Collections.Generic;
using Primoris.Universe.Stargen.Astrophysics;
using Primoris.Universe.Stargen.Astrophysics.Burrows;
using Primoris.Universe.Stargen.Bodies;
using Primoris.Universe.Stargen.Bodies.Burrows;
using UnitsNet;

namespace Primoris.Universe.Stargen.Systems.Burrows
{

	[Obsolete]
    public class SystemGenerator
	{
		public static StellarGroup GenerateStellarGroup(int seed, int numSystems, SystemGenerationOptions? genOptions = null)
		{
			Extensions.InitRandomSeed(seed);
			genOptions = genOptions ?? SystemGenerationOptions.DefaultOptions;
			var group = new StellarGroup() { Seed = seed, GenOptions = genOptions, Systems = new List<StellarSystem>() };
			for (var i = 0; i < numSystems; i++)
			{
				var name = string.Format("System {0}", i);
				group.Systems.Add(GenerateStellarSystem(name, genOptions));
			}
			return group;
		}

		public static StellarSystem GenerateStellarSystem(string systemName, SystemGenerationOptions? genOptions = null, StellarBody? sun = null, IEnumerable<Seed>? seedSystem = null)
		{
			genOptions ??= new SystemGenerationOptions();

            var phy = new BodyPhysics();
			sun ??= new Star() { Science = phy };
			var useRandomTilt = seedSystem == null;

			var accrete = new Accrete(Ratio.FromDecimalFractions(genOptions.CloudEccentricity), 
									  Ratio.FromDecimalFractions(genOptions.GasDensityRatio), 
									  Ratio.FromDecimalFractions(genOptions.DustDensityCoeff));
			double outer_planet_limit = GetOuterLimit(sun);
			double outer_dust_limit = GetStellarDustLimit(sun.Mass.SolarMasses);
			seedSystem = seedSystem ?? accrete.CreateSeeds(sun.Mass,
				sun.Luminosity, Length.FromAstronomicalUnits(0.0), Length.FromAstronomicalUnits(outer_dust_limit), Length.FromAstronomicalUnits(outer_planet_limit), Length.Zero);
			
			var planets = GeneratePlanets(sun, seedSystem, useRandomTilt, genOptions);
			return new StellarSystem()
			{
				Options = genOptions,
				Planets = planets,
				Name = systemName,
				Star = sun
			};
		}

		private static ICollection<SatelliteBody> GeneratePlanets(StellarBody sun, IEnumerable<Seed> seeds, bool useRandomTilt, SystemGenerationOptions genOptions)
		{
			var planets = new List<SatelliteBody>();
			var i = 0;
			foreach (var seed in seeds)
			{
				var planetNo = i + 1; // start counting planets at 1
				i += 1;

				string planet_id = planetNo.ToString();

				var planet = new Planet(seed, sun);
				planets.Add(planet);
			}

			return planets;
		}

		private static double GetStellarDustLimit(double stellarMassRatio)
		{
			return 200.0 * Math.Pow(stellarMassRatio, 1.0 / 3.0);
		}

		private static double GetOuterLimit(StellarBody star)
		{
			if (star.BinaryMass.SolarMasses < .001)
			{
				return 0.0;
			}

			// The following is Holman & Wiegert's equation 1 from
			// Long-Term Stability of Planets in Binary Systems
			// The Astronomical Journal, 117:621-628, Jan 1999
			double m1 = star.Mass.SolarMasses;
			double m2 = star.BinaryMass.SolarMasses;
			double mu = m2 / (m1 + m2);
			double e = star.BinarySemiMajorAxis.AstronomicalUnits;
			double e2 = Extensions.Pow2(e);
			double a = star.BinaryEccentricity.Value;

			return (0.464 + -0.380 * mu + -0.631 * e + 0.586 * mu * e + 0.150 * e2 + -0.198 * mu * e2) * a;
		}
	}
}