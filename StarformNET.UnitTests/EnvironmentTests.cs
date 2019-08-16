using Microsoft.VisualStudio.TestTools.UnitTesting;
using Primoris.Universe.Stargen.Bodies;
using System.Linq;
using System.Collections.Generic;
using System;
using Primoris.Universe.Stargen.Astrophysics;
using Primoris.Universe.Stargen.Bodies.Burrows;
using Primoris.Universe.Stargen.Astrophysics.Burrows;
using Primoris.Universe.Stargen.Services;
using UnitsNet;


namespace Primoris.Universe.Stargen.UnitTests
{

    class BurrowsPhysicsTests
    {
        [TestClass]
        public class GasLifeTest
        {
            // Note these expected values aren't based on anything. They are
            // just the output from the function expected given certain input
            // values. They are intended for regression testing, not to prove
            // 'correctness' of the function.

            private static readonly double[] Weights =
            {
                GlobalConstants.ATOMIC_HYDROGEN,
                GlobalConstants.MOL_HYDROGEN,
                GlobalConstants.ATOMIC_NITROGEN,
                GlobalConstants.MOL_NITROGEN,
                GlobalConstants.MOL_OXYGEN,
                GlobalConstants.WATER_VAPOR
            };

			[TestInitialize]
			public void InitializeTests()
			{
				Provider.Use().WithAstrophysics(new BodyPhysics());
			}

            [TestCategory("GasLife")]
            [TestMethod]
            public void TestGasesEarth()
            {
                double[] expected =
                {
                    0.000886904496537799,
                    0.0469963194268096,
                    double.MaxValue,
                    double.MaxValue,
                    double.MaxValue,
                    double.MaxValue
                };
                CheckGasValues(1500, 1.0, 6371, expected, Weights);
            }

            [TestCategory("GasLife")]
            [TestMethod]
            public void TestGasesHighExoTemp()
            {
                double[] expected =
                {
                    0.000442164077775748,
                    0.000215237230333243,
                    0.000539283973908679,
                    0.0167717184670422,
                    0.0493293473613224,
                    0.00132928020274972
                };
                CheckGasValues(4500, 0.5, 2440, expected, Weights);
            }

			private void ActionReturnNanZeroTemp()
			{
				Provider.Use().GetService<IScienceAstrophysics>().Physics.GetGasLife(Mass.FromGrams(GlobalConstants.ATOMIC_NITROGEN),
																														 Temperature.FromKelvins(0.0),
																														 Acceleration.FromStandardGravity(0.5),
																														 Length.FromKilometers(2440));
			}

			private void ActionReturnNanNegativeTemp()
			{
				Provider.Use().GetService<IScienceAstrophysics>().Physics.GetGasLife(
					Mass.FromGrams(GlobalConstants.ATOMIC_NITROGEN), Temperature.FromKelvins(-100), Acceleration.FromStandardGravity(0.5), Length.FromKilometers(2440));
			}

            [TestCategory("GasLife")]
            [TestMethod]
            public void TestReturnNaNZeroTemp()
            {
                Assert.ThrowsException<ArgumentException>(ActionReturnNanZeroTemp);
				//(Mass molecularWeight, Temperature exoTempKelvin, Acceleration surfGravG, Length radiusKM);
			}

            [TestCategory("GasLife")]
            [TestMethod]
            public void TestReturnNaNNegativeTemp()
            {
                Assert.ThrowsException<ArgumentException>(ActionReturnNanNegativeTemp);
            }

            private static void CheckGasValues(double exo, double surfG, double radius, 
                IReadOnlyList<double> expected, IReadOnlyList<double> weights)
            {
                for (var i = 0; i < expected.Count; i++)
                {
                    var e = expected[i];
                    var w = weights[i];
                    Assert.AreEqual(e, Provider.Use().GetService<IScienceAstrophysics>().Physics.GetGasLife(
                        Mass.FromGrams(w), Temperature.FromKelvins(exo), Acceleration.FromStandardGravity(surfG), Length.FromKilometers(radius)).Years365, 0.000001);
                }
            }
        }

        [TestClass]
        public class EcosphereTests
        {
            [TestCategory("Ecosphere")]
            [TestMethod]
            public void TestSunEcosphere()
            {
                const double expectedValue = 1.0;
                const double sunLuminosity = 1.0;

                var phy = new BodyPhysics();

                Assert.AreEqual(expectedValue, phy.Astronomy.GetEcosphereRadius(Mass.FromSolarMasses(1.0), 
                                                                               Luminosity.FromSolarLuminosities(sunLuminosity)).AstronomicalUnits, 0.0001);
            }
        }

        [TestClass]
        public class IlluminationTests
        {
            [TestCategory("Illumination")]
            [TestMethod]
            public void TestSunIllumination()
            {
				var phy = new BodyPhysics();

                var expectedValue = 1.0;
                var sunLuminosity = Luminosity.FromSolarLuminosities(1.0);
                var earthSemiMajorAxis =  Length.FromAstronomicalUnits(1.0);

                Assert.AreEqual(expectedValue, phy.GetMinimumIllumination(earthSemiMajorAxis, sunLuminosity).DecimalFractions);
            }
        }

        [TestClass]
        public class HillSphereTests
        {
            // Expected hill sphere value from:
            // http://orbitsimulator.com/formulas/hillsphere.html

            public static Mass SunMass = Mass.FromSolarMasses(1);
            public static Mass EarthMass = Mass.FromSolarMasses(0.000003003);
            public static Mass MercuryMass = Mass.FromSolarMasses(0.0000001652);
            public static Mass VenusMass = Mass.FromSolarMasses(0.000002447);
            public static Mass JupiterMass = Mass.FromSolarMasses(0.0009543);

            public static Length EarthSemiMajorAxis = Length.FromKilometers(149600000);
            //public static double EarthSemiMajorAxisAU = EarthSemiMajorAxisKM / GlobalConstants.KM_PER_AU;
            public static Length MercurySemiMajorAxis = Length.FromKilometers(57909050);
            //public static double MercurySemiMajorAxisAU = MercurySemiMajorAxisKM / GlobalConstants.KM_PER_AU;
            public static Length VenusSemiMajorAxis = Length.FromKilometers(108208000);
            //public static double VenusSemiMajorAxisAU = VenusSemiMajorAxisKM / GlobalConstants.KM_PER_AU;
            public static Length JupiterSemiMajorAxis = Length.FromKilometers(778297882);
            //public static double JupiterSemiMajorAxisAU = JupiterSemiMajorAxisKM / GlobalConstants.KM_PER_AU;

            [TestCategory("Hill Sphere")]
            [TestMethod]
            public void TestSunEarthHillSphere()
            {
				var phy = new BodyPhysics();

                var earthSphereKM = 1496498;
                var earthSphereAU = earthSphereKM / GlobalConstants.KM_PER_AU;

                var hAU = phy.GetHillSphere(SunMass, EarthMass, EarthSemiMajorAxis).AstronomicalUnits;
                Assert.AreEqual(earthSphereAU, hAU, 0.001);

                var hKM = phy.GetHillSphere(SunMass, EarthMass, EarthSemiMajorAxis).Kilometers;
                Assert.AreEqual(earthSphereKM, hKM, 0.99);
            }

            [TestCategory("Hill Sphere")]
            [TestMethod]
            public void TestSunMercuryHillSphere()
            {
				var phy = new BodyPhysics();

                var mercurySphereKM = 220314;
                var mercurySphereAU = mercurySphereKM / GlobalConstants.KM_PER_AU;

                var hAU = phy.GetHillSphere(SunMass, MercuryMass, MercurySemiMajorAxis).AstronomicalUnits;
                Assert.AreEqual(mercurySphereAU, hAU, 0.001);

                var hKM = phy.GetHillSphere(SunMass, MercuryMass, MercurySemiMajorAxis).Kilometers;
                Assert.AreEqual(mercurySphereKM, hKM, 0.99);
            }

            [TestCategory("Hill Sphere")]
            [TestMethod]
            public void TestSunVenusHillSphere()
            {
				var phy = new BodyPhysics();

                var venusSphereKM = 1011028;
                var venusSphereAU = venusSphereKM / GlobalConstants.KM_PER_AU;

                var hAU = phy.GetHillSphere(SunMass, VenusMass, VenusSemiMajorAxis).AstronomicalUnits;
                Assert.AreEqual(venusSphereAU, hAU, 0.001);

                var hKM = phy.GetHillSphere(SunMass, VenusMass, VenusSemiMajorAxis).Kilometers;
                Assert.AreEqual(venusSphereKM, hKM, 0.99);
            }

            [TestCategory("Hill Sphere")]
            [TestMethod]
            public void TestSunJupiterHillSphere()
            {
				var phy = new BodyPhysics();

                var jupiterSphereKM = 53129256;
                var jupiterSphereAU = jupiterSphereKM / GlobalConstants.KM_PER_AU;

                var hAU = phy.GetHillSphere(SunMass, JupiterMass, JupiterSemiMajorAxis).AstronomicalUnits;
                Assert.AreEqual(jupiterSphereAU, hAU, 0.001);

                var hKM = phy.GetHillSphere(SunMass, JupiterMass, JupiterSemiMajorAxis).Kilometers;
                Assert.AreEqual(jupiterSphereKM, hKM, 0.99);
            }
        }

        [TestClass]
        public class RocheLimitTests
        {
            public static Density SunDensity = Density.FromGramsPerCubicCentimeter(1.408);
            public static Length SunRadius = Length.FromMeters(696000000);
            public static Density EarthDensity = Density.FromGramsPerCubicCentimeter(5.513);
            public static Length EarthRadius = Length.FromMeters(6378137);
            public static Density MoonDensity = Density.FromGramsPerCubicCentimeter(3.346);
            public static Length MoonRadius = Length.FromMeters(1737100);
            public static Density JupiterDensity = Density.FromGramsPerCubicCentimeter(1.326);
            public static Length JupiterRadius = Length.FromMeters(71493000);
            public static Density SaturnDensity = Density.FromGramsPerCubicCentimeter(0.687);
            public static Length SaturnRadius = Length.FromMeters(60267000);
            public static Density AvgCometDensity = Density.FromGramsPerCubicCentimeter(.5);

            [TestCategory("Roche Limit")]
            [TestMethod]
            public void TestEarthMoonRocheLimit()
            {
				var phy = new BodyPhysics();

                var earthMoonKM = 9492;
                var earthMoonAU = earthMoonKM / GlobalConstants.KM_PER_AU;

                var dAU = phy.GetRocheLimit(EarthRadius, EarthDensity, MoonDensity).AstronomicalUnits;
                Assert.AreEqual(earthMoonAU, dAU, 0.99);

                var dKM = phy.GetRocheLimit(EarthRadius, EarthDensity, MoonDensity).Kilometers;
                Assert.AreEqual(earthMoonKM, dKM, 0.99);
            }

            [TestCategory("Roche Limit")]
            [TestMethod]
            public void TestEarthAverageCometRocheLimit()
            {
				var phy = new BodyPhysics();

                var earthAvgCometKM = 17887;
                var earthAvgCometAU = earthAvgCometKM / GlobalConstants.KM_PER_AU;

                var dAU = phy.GetRocheLimit(EarthRadius, EarthDensity, AvgCometDensity).AstronomicalUnits;
                Assert.AreEqual(earthAvgCometAU, dAU, 0.99);

                var dKM = phy.GetRocheLimit(EarthRadius, EarthDensity, AvgCometDensity).Kilometers;
                Assert.AreEqual(earthAvgCometKM, dKM, 0.99);
            }

            [TestCategory("Roche Limit")]
            [TestMethod]
            public void TestSunEarthRocheLimit()
            {
				var phy = new BodyPhysics();

                var sunEarthKM = 556397;
                var sunEarthAU = sunEarthKM / GlobalConstants.KM_PER_AU;

				var dAU = phy.GetRocheLimit(SunRadius, SunDensity, EarthDensity).AstronomicalUnits;
                Assert.AreEqual(sunEarthAU, dAU, 0.99);

                var dKM = phy.GetRocheLimit(SunRadius, SunDensity, EarthDensity).Kilometers;
                Assert.AreEqual(sunEarthKM, dKM, 0.99);
            }

            [TestCategory("Roche Limit")]
            [TestMethod]
            public void TestSunMoonRocheLimit()
            {
				var phy = new BodyPhysics();

                var sunMoonKM = 657161;
                var sunMoonAU = sunMoonKM / GlobalConstants.KM_PER_AU;

                var dAU = phy.GetRocheLimit(SunRadius, SunDensity, MoonDensity).AstronomicalUnits;
                Assert.AreEqual(sunMoonAU, dAU, 0.99);

                var dKM = phy.GetRocheLimit(SunRadius, SunDensity, MoonDensity).Kilometers;
                Assert.AreEqual(sunMoonKM, dKM, 0.99);
            }

            [TestCategory("Roche Limit")]
            [TestMethod]
            public void TestSunJupiterRocheLimit()
            {
				var phy = new BodyPhysics();

                var sunJupiterKM = 894677;
                var sunJupiterAU = sunJupiterKM / GlobalConstants.KM_PER_AU;

                var dAU = phy.GetRocheLimit(SunRadius, SunDensity, JupiterDensity).AstronomicalUnits;
                Assert.AreEqual(sunJupiterAU, dAU, 0.99);

                var dKM = phy.GetRocheLimit(SunRadius, SunDensity, JupiterDensity).Kilometers;
                Assert.AreEqual(sunJupiterKM, dKM, 0.99);
            }
        }

        [TestClass]
        public class BreathabilityTests
        {
            private static Dictionary<string, Chemical> TestGases = new Dictionary<string, Chemical>()
            {
                {"N", new Chemical(GlobalConstants.AN_N,  "N",    "N<SUB><SMALL>2</SMALL></SUB>",  "Nitrogen",        14.0067,  63.34,  77.40,  0.0012506, 1.99526e-05, 3.13329,       0,     GlobalConstants.MAX_N2_IPP ) },
                {"O", new Chemical(GlobalConstants.AN_O,  "O",    "O<SUB><SMALL>2</SMALL></SUB>",  "Oxygen",          15.9994,  54.80,  90.20,  0.001429,  0.501187,    23.8232,       10,    GlobalConstants.MAX_O2_IPP ) },
                {"CO2", new Chemical(GlobalConstants.AN_CO2, "CO2", "CO<SUB><SMALL>2</SMALL></SUB>", "CarbonDioxide",   44.0000, 194.66, 194.66,  0.001,     0.01,        0.0005,        0,     GlobalConstants.MAX_CO2_IPP) },
            };

			//private static Provider _dummy = Provider.Use().WithAstrophysics(new BodyPhysics());

			[TestInitialize]
			public void InitializeTests()
			{
				Provider.Use().WithAstrophysics(new BodyPhysics());
			}

            private IEnumerable<Layer> GetMockBreathableAtmo()
            {
				var layers = new List<Layer>()
				{
					new BasicSolidLayer(),
					new BasicGaseousLayer(new List<ValueTuple<Chemical, Ratio>>()
					{
						new ValueTuple<Chemical, Ratio>(TestGases["O"], Ratio.FromDecimalFractions(0.21)),
						new ValueTuple<Chemical, Ratio>(TestGases["N"], Ratio.FromDecimalFractions(0.78))
					}, Pressure.FromBars(1.0))
				};

				return layers;
                /*return new Gas[]
                {
                    new Gas(TestGases["O"], Pressure.FromMillibars(GlobalConstants.EARTH_SURF_PRES_IN_MILLIBARS * 0.21) ),
                    new Gas(TestGases["N"], Pressure.FromMillibars(GlobalConstants.EARTH_SURF_PRES_IN_MILLIBARS * 0.78) )
                };*/
            }

            private IEnumerable<Layer> GetMockPoisonousAtmo()
            {
				var layers = new List<Layer>()
				{
					new BasicSolidLayer(),
					new BasicGaseousLayer(new List<ValueTuple<Chemical, Ratio>>()
					{
						new ValueTuple<Chemical, Ratio>(TestGases["CO2"], Ratio.FromDecimalFractions(1.0))
					}, Pressure.FromBars(1.0))
				};

				return layers;

				/*return new Gas[]
                {
                    new Gas(TestGases["CO2"], Pressure.FromMillibars(GlobalConstants.EARTH_SURF_PRES_IN_MILLIBARS) )
                };*/
            }

            private IEnumerable<Layer> GetMockUnbreathableAtmo()
            {
				var layers = new List<Layer>()
				{
					new BasicSolidLayer(),
					new BasicGaseousLayer(new List<ValueTuple<Chemical, Ratio>>()
					{
						new ValueTuple<Chemical, Ratio>(TestGases["N"], Ratio.FromDecimalFractions(1.0))
					}, Pressure.FromBars(1.0))
				};

				return layers;

				/*return new Gas[]
                {
                    new Gas(TestGases["N"], Pressure.FromMillibars(GlobalConstants.EARTH_SURF_PRES_IN_MILLIBARS) )
                };*/
            }

            private IEnumerable<Layer> GetMockNoAtmo()
            {
				var layers = new List<Layer>()
				{
					new BasicSolidLayer()
				};

				return layers;

				//return new Gas[0];
            }

            private SatelliteBody GetMockPlanet(Func<IEnumerable<Layer>> mockAtmoGen)
            {
				var star = new Star();

				var seed = new Seed(Length.FromAstronomicalUnits(1.0), Ratio.FromDecimalFractions(1.0), Mass.FromEarthMasses(1.0), Mass.FromEarthMasses(1.0), Mass.FromEarthMasses(0.000001));

                var planet = new Planet(seed, star, star, mockAtmoGen());

				//planet.RecalculateGases(mockAtmoGen());
				return planet;
            }

            /*[TestCategory("Breathability")]
            [ExpectedException(typeof(ArgumentNullException))]
            [TestMethod]
            public void TestNullPlanet()
            {
                var breathe = Primoris.Universe.Stargen.Environment.Breathability(null);
            }*/

            [TestCategory("Breathability")]
            [TestMethod]
            public void TestNoAtmoPlanet()
            {
                var planet = GetMockPlanet(GetMockNoAtmo);
				var breathe = planet.Breathability;
                Assert.AreEqual(Breathability.None, breathe);
            }

            [TestCategory("Breathability")]
            [TestMethod]
            public void TestBreathablePlanet()
            {
                var planet = GetMockPlanet(GetMockBreathableAtmo);
				var breathe = planet.Breathability;
				Assert.AreEqual(Breathability.Breathable, breathe);
            }

            [TestCategory("Breathability")]
            [TestMethod]
            public void TestUnbreathablePlanet()
            {
                var planet = GetMockPlanet(GetMockUnbreathableAtmo);
				var breathe = planet.Breathability;
				Assert.AreEqual(Breathability.Unbreathable, breathe);
            }

            [TestCategory("Breathability")]
            [TestMethod]
            public void TestPoisonousPlanet()
            {
                var planet = GetMockPlanet(GetMockPoisonousAtmo);
				var breathe = planet.Breathability;
				Assert.AreEqual(Breathability.Poisonous, breathe);
            }
        }
    }
}
