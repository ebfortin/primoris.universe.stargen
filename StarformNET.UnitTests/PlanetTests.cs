using Microsoft.VisualStudio.TestTools.UnitTesting;
using Primoris.Universe.Stargen.Systems;

namespace Primoris.Universe.Stargen.UnitTests
{

	public class PlanetTests
    {
        [TestClass]
        public class EqualTests
        {
            [TestCategory("Planet.Equals")]
            [TestMethod]
            public void TestGeneratedEquality()
            {
                Utilities.InitRandomSeed(0);
                var system1 = OriginalGenerator.GenerateStellarSystem("system1").Planets;

                Utilities.InitRandomSeed(0);
                var system2 = OriginalGenerator.GenerateStellarSystem("system2").Planets;

                Assert.IsTrue(system1[0].Equals(system2[0]));
            }

            [TestCategory("Planet.Equals")]
            [TestMethod]
            public void TestGeneratedInequality()
            {
                Utilities.InitRandomSeed(0);
                var system1 = OriginalGenerator.GenerateStellarSystem("system1").Planets;

                Utilities.InitRandomSeed(1);
                var system2 = OriginalGenerator.GenerateStellarSystem("system2").Planets;

                Assert.IsFalse(system1[0].Equals(system2[0]));
            }
        }
    }
}
