using Microsoft.VisualStudio.TestTools.UnitTesting;
using Primoris.Universe.Stargen;
using Primoris.Universe.Stargen.Bodies;
using System.Collections.Generic;
using System;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Environment = Primoris.Universe.Stargen.Astrophysics;
using Primoris.Universe.Stargen.Systems;
using Primoris.Universe.Stargen.Bodies.Burrows;
using Primoris.Universe.Stargen.Systems.Burrows;
using Primoris.Universe.Stargen.Astrophysics;
using Primoris.Universe.Stargen.Astrophysics.Burrows;
using UnitsNet;

namespace Primoris.Universe.Stargen.UnitTests
{

    class GeneratorTests
	{
		[TestClass]
		public class GenerateStellarSystemTests
		{
			private string TEST_FILE = "testsystem.bin";
			private string TEST_FILE_PATH = "Testdata";

			/*[TestCategory("Generator Regression")]
			[TestMethod]*/
			public void TestSameSeedAgainstSavedOutput()
			{
				var baseDir = AppDomain.CurrentDomain.BaseDirectory;
				var testFileDir = Path.Combine(baseDir, TEST_FILE_PATH, TEST_FILE);

				IFormatter formatter = new BinaryFormatter();
				Stream stream = new FileStream(testFileDir, FileMode.Open, FileAccess.Read, FileShare.Read);
				var savedSystem = ((StellarSystem)formatter.Deserialize(stream)).Planets;
				stream.Close();

				Utilities.InitRandomSeed(0);
				var newSystem = SystemGenerator.GenerateStellarSystem("test").Planets;
				Assert.AreEqual(savedSystem.Count(), newSystem.Count(), "Incorrect number of planets");
				for (var i = 0; i < savedSystem.Count(); i++)
				{
					Assert.IsTrue(savedSystem.SequenceEqual<SatelliteBody>(newSystem), String.Format("Planet {0} not equal", i));
				}
			}

			/*[TestCategory("Generator Regression")]
			[TestMethod]*/
			public void TestDifferentSeedAgainstSavedOutput()
			{
				var baseDir = AppDomain.CurrentDomain.BaseDirectory;
				var testFileDir = Path.Combine(baseDir, TEST_FILE_PATH, TEST_FILE);

				IFormatter formatter = new BinaryFormatter();
				Stream stream = new FileStream(testFileDir, FileMode.Open, FileAccess.Read, FileShare.Read);
				var savedSystem = ((StellarSystem)formatter.Deserialize(stream)).Planets;
				stream.Close();

				Utilities.InitRandomSeed(1);
				var newSystem = SystemGenerator.GenerateStellarSystem("test").Planets;
				var atleastOneDifferent = false;
				if(savedSystem.Count() != newSystem.Count())
				{
					atleastOneDifferent = true;
				}
				else
				{
					atleastOneDifferent = savedSystem.SequenceEqual<SatelliteBody>(newSystem);
				}
				/*for (var i = 0; i < savedSystem.Count; i++)
				{
					if (!savedSystem[i].Equals(newSystem[i]))
					{
						atleastOneDifferent = true;
						break;
					}
				}*/
				Assert.IsTrue(atleastOneDifferent);
			}
		}

		[TestClass]
		public class CalculateGasesTest
		{
			private double DELTA = 0.0001;

			private StellarBody GetTestStar()
			{
                var phy = new BodyPhysics();
				return new Star(Mass.FromSolarMasses(1.0), Luminosity.FromSolarLuminosities(1.0), Duration.FromYears365(1e10));
			}

			private SatelliteBody GetTestPlanetAtmosphere()
			{
                var star = GetTestStar();

				var planet = new Planet(star,
                            star,
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
                var star = GetTestStar();
				var planet = new Planet(star, star);
				return planet;
			}

			[TestCategory("Atmosphere")]
			[TestMethod]
			public void TestEmptyPlanet()
			{
                var star = GetTestStar();
				var planet = new Planet(star, star);

				Assert.AreEqual(0, planet.Atmosphere.Composition.Count);
			}

			[TestCategory("Atmosphere")]
			[TestMethod]
			public void TestEmptyChemTable()
			{
				var planet = GetTestPlanetAtmosphere();
				planet.RecalculateGases(new Chemical[0]);

				Assert.AreEqual(0, planet.Atmosphere.Composition.Count);
			}

			[TestCategory("Atmosphere")]
			[TestMethod]
			public void TestAtmosphereDefaultChemTable()
			{
				var expected = new Dictionary<string, double>()
				{
					{ "CH4", 0.00000 },
					{ "NH3", 0.00000 },
					{ "H2O", 440.14434 },
					{ "Ne", 23.74552607000449 },
					{ "Ar", 89996.39719877411 },
					{ "CO2", 1315.6310584147352 },
					{ "O3", 0.00000 },
					{ "Br", 0.00000 },
					{ "Kr", 193.8829087894019 },
					{ "I", 4.791810606490484 },
					{ "Xe", 25.407153913445107 }
				};

                var planet = GetTestPlanetAtmosphere();

                Assert.AreEqual(expected.Count, planet.Atmosphere.Composition.Count);

				double surfPres = 0.0;
                foreach (var gas in planet.Atmosphere.Composition)
                {
                    Assert.AreEqual(expected[gas.Chemical.Symbol], gas.SurfacePressure.Millibars, DELTA);
					surfPres += gas.SurfacePressure.Millibars;
				}

				Assert.AreEqual(planet.Atmosphere.SurfacePressure.Millibars, surfPres, DELTA);
            }

            [TestCategory("Atmosphere")]
            [TestMethod]
            public void TestNoAtmosphereDefaultChemTable()
            {
                var planet = GetTestPlanetNoAtmosphere();


                Assert.AreEqual(0, planet.Atmosphere.Composition.Count);
            }
        }
    }
}
