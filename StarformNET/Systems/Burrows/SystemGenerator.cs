using System;
using System.Collections.Generic;
using Primoris.Universe.Stargen.Astrophysics;
using Primoris.Universe.Stargen.Bodies;
using Primoris.Universe.Stargen.Bodies.Burrows;
using UnitsNet;

namespace Primoris.Universe.Stargen.Systems.Burrows
{


	public class SystemGenerator
	{
		public static StellarGroup GenerateStellarGroup(int seed, int numSystems, SystemGenerationOptions genOptions = null)
		{
			Utilities.InitRandomSeed(seed);
			genOptions = genOptions ?? SystemGenerationOptions.DefaultOptions;
			var group = new StellarGroup() { Seed = seed, GenOptions = genOptions, Systems = new List<StellarSystem>() };
			for (var i = 0; i < numSystems; i++)
			{
				var name = string.Format("System {0}", i);
				group.Systems.Add(GenerateStellarSystem(name, genOptions));
			}
			return group;
		}

		public static StellarSystem GenerateStellarSystem(string systemName, SystemGenerationOptions genOptions = null, Star sun = null, IEnumerable<Seed> seedSystem = null)
		{
			genOptions ??= new SystemGenerationOptions();
			sun ??= new Star();
			var useRandomTilt = seedSystem == null;

			var accrete = new Accrete(genOptions.CloudEccentricity, genOptions.GasDensityRatio);
			double outer_planet_limit = GetOuterLimit(sun);
			double outer_dust_limit = GetStellarDustLimit(sun.Mass.SolarMasses);
			seedSystem = seedSystem ?? accrete.CreateSeeds(sun.Mass,
				sun.Luminosity, Length.FromAstronomicalUnits(0.0), Length.FromAstronomicalUnits(outer_dust_limit), Length.FromAstronomicalUnits(outer_planet_limit),
				Ratio.FromDecimalFractions(genOptions.DustDensityCoeff), Length.FromAstronomicalUnits(double.NaN), Ratio.FromDecimalFractions(double.NaN));

			// Todo: swing that to Star.
			var planets = GeneratePlanets(sun, seedSystem, useRandomTilt, genOptions);
			return new StellarSystem()
			{
				Options = genOptions,
				Planets = planets,
				Name = systemName,
				Star = sun
			};
		}

		private static ICollection<SatelliteBody> GeneratePlanets(Star sun, IEnumerable<Seed> seeds, bool useRandomTilt, SystemGenerationOptions genOptions)
		{
			var planets = new List<SatelliteBody>();
			var i = 0;
			foreach (var seed in seeds)
			{
				var planetNo = i + 1; // start counting planets at 1
				i += 1;

				string planet_id = planetNo.ToString();

				var planet = new Planet(seed, sun, planetNo, useRandomTilt, planet_id, genOptions);
				planets.Add(planet);
			}

			return planets;
		}

		private static double GetStellarDustLimit(double stellarMassRatio)
		{
			return 200.0 * Math.Pow(stellarMassRatio, 1.0 / 3.0);
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
			double m1 = star.Mass.SolarMasses;
			double m2 = star.BinaryMass;
			double mu = m2 / (m1 + m2);
			double e = star.SemiMajorAxisAU;
			double e2 = Utilities.Pow2(e);
			double a = star.Eccentricity;

			return (0.464 + -0.380 * mu + -0.631 * e + 0.586 * mu * e + 0.150 * e2 + -0.198 * mu * e2) * a;
		}
	}
}