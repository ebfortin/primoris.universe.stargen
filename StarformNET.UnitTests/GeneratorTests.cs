namespace Primoris.Universe.Stargen.UnitTests
{
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using Primoris.Universe.Stargen;
	using Data;
	using System.Collections.Generic;
	using System;
	using System.IO;
	using System.Runtime.Serialization;
	using System.Runtime.Serialization.Formatters.Binary;
	using Environment = Primoris.Universe.Stargen.Environment;

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
				var savedSystem = (List<Planet>)formatter.Deserialize(stream);
				stream.Close();

				Utilities.InitRandomSeed(0);
				var newSystem = Generator.GenerateStellarSystem("test").Planets;
				Assert.AreEqual(savedSystem.Count, newSystem.Count, "Incorrect number of planets");
				for (var i = 0; i < savedSystem.Count; i++)
				{
					Assert.IsTrue(savedSystem[i].Equals(newSystem[i]), String.Format("Planet {0} not equal", i));
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
				var savedSystem = (List<Planet>)formatter.Deserialize(stream);
				stream.Close();

				Utilities.InitRandomSeed(1);
				var newSystem = Generator.GenerateStellarSystem("test").Planets;
				var atleastOneDifferent = false;
				for (var i = 0; i < savedSystem.Count; i++)
				{
					if (!savedSystem[i].Equals(newSystem[i]))
					{
						atleastOneDifferent = true;
						break;
					}
				}
				Assert.IsTrue(atleastOneDifferent);
			}
		}

		[TestClass]
		public class CalculateGasesTest
		{
			private double DELTA = 0.0001;

			private Star GetTestStar()
			{
				return new Star(1.0, 1.0, 1e10);
			}

			private Planet GetTestPlanetAtmosphere()
			{
				var planet = new Planet(GetTestStar(), 0.723332, 0.0067, 2.8, 2802.0, 225.0, 0.000002447, 2.41E-10, 6051.8, 92000.0, 737.0, 737.0, 737.0, 0.9);

				return planet;
			}

			private Planet GetTestPlanetNoAtmosphere()
			{
				var planet = new Planet(GetTestStar());
				return planet;
			}

			[TestCategory("Atmosphere")]
			[TestMethod]
			public void TestEmptyPlanet()
			{
				var planet = new Planet(GetTestStar());
				//Generator.CalculateGases(planet, new ChemType[0]);

				Assert.AreEqual(0, planet.Atmosphere.Composition.Count);
			}

			[TestCategory("Atmosphere")]
			[TestMethod]
			public void TestEmptyChemTable()
			{
				var planet = GetTestPlanetAtmosphere();
				planet.RecalculateGases(new ChemType[0]);

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
					{ "H2O", 439.75570 },
					{ "Ne", 23.73352 },
					{ "Ar", 89996.73270 },
					{ "CO2", 1315.66719 },
					{ "O3", 0.00000 },
					{ "Br", 0.00000 },
					{ "Kr", 193.90706 },
					{ "I", 4.79257 },
					{ "Xe", 25.41126 }
				};

                var planet = GetTestPlanetAtmosphere();
                var star = planet.Star;
                //Generator.CalculateGases(planet, ChemType.Load());

                Assert.AreEqual(expected.Count, planet.Atmosphere.Composition.Count);

                foreach (var gas in planet.Atmosphere.Composition)
                {
                    Assert.AreEqual(expected[gas.GasType.Symbol], gas.SurfacePressure, DELTA);
                }
            }

            [TestCategory("Atmosphere")]
            [TestMethod]
            public void TestNoAtmosphereDefaultChemTable()
            {
                var planet = GetTestPlanetNoAtmosphere();
                var star = planet.Star;
                //Generator.CalculateGases(planet, ChemType.Load());

                Assert.AreEqual(0, planet.Atmosphere.Composition.Count);
            }
        }
    }
}
