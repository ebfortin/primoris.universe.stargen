using Microsoft.VisualStudio.TestTools.UnitTesting;
using Primoris.Universe.Stargen.Bodies;
using System.Linq;
using Primoris.Universe.Stargen.Systems.Burrows;
using Primoris.Universe.Stargen.Services;
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

			[TestCategory("Planet.Equals")]
            [TestMethod]
            public void TestGeneratedEquality()
            {
                Utilities.InitRandomSeed(0);
                var system1 = SystemGenerator.GenerateStellarSystem("system1").Planets;

                Utilities.InitRandomSeed(0);
                var system2 = SystemGenerator.GenerateStellarSystem("system2").Planets;

                Assert.IsTrue(system1.SequenceEqual(system2));
            }

            [TestCategory("Planet.Equals")]
            [TestMethod]
            public void TestGeneratedInequality()
            {
                Utilities.InitRandomSeed(0);
                var system1 = SystemGenerator.GenerateStellarSystem("system1").Planets;

                Utilities.InitRandomSeed(1);
                var system2 = SystemGenerator.GenerateStellarSystem("system2").Planets;

				Assert.IsFalse(system1.SequenceEqual(system2));
            }
        }
    }
}
