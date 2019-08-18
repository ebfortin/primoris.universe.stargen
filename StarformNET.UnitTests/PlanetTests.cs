using Microsoft.VisualStudio.TestTools.UnitTesting;
using Primoris.Universe.Stargen.Bodies;
using Primoris.Universe.Stargen.Bodies.Burrows;
using System.Linq;
using Primoris.Universe.Stargen.Systems.Burrows;
using Primoris.Universe.Stargen.Services;
using Primoris.Universe.Stargen.Astrophysics;
using Primoris.Universe.Stargen.Astrophysics.Burrows;


namespace Primoris.Universe.Stargen.UnitTests
{

	public class PlanetTests
    {
        [TestClass]
        public class EqualTests
        {
			[TestInitialize]
			public void InitializeTests()
			{
				Provider.Use().WithAstrophysics(new BodyPhysics());
			}

			private SatelliteBody CreatePlanet(Seed seed, StellarBody star, int pos, string planetID)
			{
				return new Planet(seed, star, star) { Position = pos, Name = planetID };
			}

			[TestCategory("Planet.Equals")]
            [TestMethod]
            public void TestGeneratedEquality()
            {
                Extensions.InitRandomSeed(0);
				var star = new Star();
				star.GenerateSystem(CreatePlanet);
				var system1 = star.Satellites;

				Extensions.InitRandomSeed(0);
				var star2 = new Star();
				star.GenerateSystem(CreatePlanet);
				var system2 = star.Satellites;

				Assert.IsTrue(system1.SequenceEqual(system2));
            }

            [TestCategory("Planet.Equals")]
            [TestMethod]
            public void TestGeneratedInequality()
            {
				Extensions.InitRandomSeed(0);
				var star = new Star();
				star.GenerateSystem(CreatePlanet);
				var system1 = star.Satellites;

				Extensions.InitRandomSeed(1);
				var star2 = new Star();
				star.GenerateSystem(CreatePlanet);
				var system2 = star.Satellites;

				Assert.IsFalse(system1.SequenceEqual(system2));
            }
        }
    }
}
