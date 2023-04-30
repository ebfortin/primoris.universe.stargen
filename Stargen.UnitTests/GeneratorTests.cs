using Microsoft.VisualStudio.TestTools.UnitTesting;

using Primoris.Universe.Stargen.Astrophysics;
using Primoris.Universe.Stargen.Astrophysics.Burrows;
using Primoris.Universe.Stargen.Bodies;
using Primoris.Universe.Stargen.Bodies.Burrows;
using Primoris.Universe.Stargen.Services;

using System;
using System.Collections.Generic;
using System.Linq;

using UnitsNet;

namespace Primoris.Universe.Stargen.UnitTests;


public class GeneratorTests
{
	[TestClass]
	public class GenerateStellarSystemTests
	{
		private string TEST_FILE = "testsystem.bin";
		private string TEST_FILE_PATH = "Testdata";

		[TestInitialize]
		public void InitializeTests()
		{
			Provider.Use().WithAstrophysics(new BodyPhysics());
		}
	}

	[TestClass]
	public class CalculateGasesTest
	{
		IBodyFormationAlgorithm _algo = null;

		[TestInitialize]
		public void InitializeTests()
		{
			//Provider.Use().WithAstrophysics(new BodyPhysics());

			_algo = new Accrete(Ratio.FromDecimalFractions(GlobalConstants.CLOUD_ECCENTRICITY),
								Ratio.FromDecimalFractions(GlobalConstants.K),
								Ratio.FromDecimalFractions(GlobalConstants.DUST_DENSITY_COEFF));
		}

		private double DELTA = 0.0001;

		private StellarBody GetTestStar()
		{
			return new Star(new BodyPhysics(), Mass.FromSolarMasses(1.0), Luminosity.FromSolarLuminosities(1.0), Duration.FromYears365(1e10))
			{
				BodyFormationScience = _algo
			};
		}

		private SatelliteBody GetTestPlanetAtmosphere()
		{
			var star = GetTestStar();

			var planet = new Planet(star,
						Length.FromAstronomicalUnits(0.723332),
						Ratio.FromDecimalFractions(0.0067),
						Angle.FromDegrees(2.8),
						Duration.FromHours(2802.0),
						Duration.FromDays(225.0),
						Mass.FromSolarMasses(0.000002447),
						Mass.FromSolarMasses(2.41E-10),
						Length.FromKilometers(6051.8),
						Pressure.FromMillibars(92000.0),
						Temperature.FromKelvins(737.0),
						Temperature.FromKelvins(737.0),
						Temperature.FromKelvins(737.0),
						Acceleration.FromStandardGravity(0.9));

			return planet;
		}

		private SatelliteBody GetTestPlanetNoAtmosphere()
		{
			var seed = new Seed(Length.FromAstronomicalUnits(1.0), Ratio.FromDecimalFractions(1.0), Mass.FromEarthMasses(1.0), Mass.FromEarthMasses(1.0), Mass.Zero);
			var star = GetTestStar();
			var planet = new Planet(seed, star, new List<Layer>() { new BasicSolidLayer(Length.FromKilometers(10000.0), Mass.FromEarthMasses(1.0), Array.Empty<(Chemical, Ratio)>()) });
			return planet;
		}

		[TestCategory("Atmosphere")]
		[TestMethod]
		public void TestEmptyPlanet()
		{
			var star = GetTestStar();
			var planet = GetTestPlanetNoAtmosphere();

			Assert.AreEqual(0, planet.AtmosphereComposition.Count());
		}

		[TestCategory("Atmosphere")]
		[TestMethod]
		public void TestAtmosphereDefaultChemTable()
		{
			var expected = new Dictionary<string, double>()
			{
				{ "CH4", 0.00000 },
				{ "NH3", 0.00000 },
				{ "H2O", 0.004784178 },
				{ "Ne", 0.000258104 },
				{ "Ar", 0.978221709 },
				{ "CO2", 0.014300338 },
				{ "O3", 0.00000 },
				{ "Br", 0.00000 },
				{ "Kr", 0.002107423 },
				{ "I", 5.20849E-05 },
				{ "Xe", 0.000276165 }
			};

			var planet = GetTestPlanetAtmosphere();

			Assert.AreEqual(expected.Count, planet.AtmosphereComposition.Count());

			double surfPres = 0.0;
			foreach (var gas in planet.AtmosphereComposition)
			{
				Assert.AreEqual(expected[gas.Item1.Symbol], gas.Item2.DecimalFractions, DELTA);
				surfPres += gas.Item2.DecimalFractions;
			}

			Assert.AreEqual(1.0, surfPres, DELTA);
		}

		[TestCategory("Atmosphere")]
		[TestMethod]
		public void TestNoAtmosphereDefaultChemTable()
		{
			var planet = GetTestPlanetNoAtmosphere();


			Assert.AreEqual(0, planet.AtmosphereComposition.Count());
		}
	}
}
