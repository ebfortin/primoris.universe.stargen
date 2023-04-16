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

using Data = DLS.StarformNET.Data;
using Env = DLS.StarformNET.Environment;


namespace Primoris.Universe.Stargen.UnitTests
{
    public class StarformValidationTests
    {
        [TestClass]
        public class AstrophysicsTest
        {
            IScienceAstrophysics _phy = new BodyPhysics();
            double LowDelta = 1e-6;
            double HighDelta = 1e-4;
            double VeryHighDelta = 1e-2;
            double UnitDelta = 1.0;

			[TestInitialize]
			public void InitializeTests()
			{
				Provider.Use().WithAstrophysics(new BodyPhysics());
			}

            private StellarBody GetTestStar()
            {
                return new Star(Mass.FromSolarMasses(1.0), Luminosity.FromSolarLuminosities(1.0), Duration.FromYears365(4.6e9)) { Science = new BodyPhysics() };
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

            private Data.Star GetTestStarStarform()
            {
                return new Data.Star()
                {
                    Luminosity = 1.0,
                    Mass = 1.0,
                    AgeYears = 4_600_000_000
                };
            }

            private Data.Planet GetTestPlanetAtmosphereStarform()
            {
                var planet = new Data.Planet();
                planet.Star = GetTestStarStarform();
                planet.Star.EcosphereRadiusAU = System.Math.Sqrt(planet.Star.Luminosity);
                planet.SemiMajorAxisAU = 0.723332;
                planet.Eccentricity = 0.0067;
                planet.AxialTiltDegrees = 2.8;
                planet.OrbitZone = Env.OrbitalZone(planet.Star.Luminosity, planet.SemiMajorAxisAU);
                planet.DayLengthHours = 2802;
                planet.OrbitalPeriodDays = 225;

                planet.MassSM = 0.000002447;
                planet.GasMassSM = 2.41E-10;
                planet.DustMassSM = planet.MassSM - planet.GasMassSM;
                planet.RadiusKM = 6051.8;
                planet.DensityGCC = Env.EmpiricalDensity(planet.MassSM, planet.SemiMajorAxisAU, planet.Star.EcosphereRadiusAU, true);
                planet.ExosphereTempKelvin = GlobalConstants.EARTH_EXOSPHERE_TEMP / Extensions.Pow2(planet.SemiMajorAxisAU / planet.Star.EcosphereRadiusAU);
                planet.SurfaceAccelerationCMSec2 = Env.Acceleration(planet.MassSM, planet.RadiusKM);
                planet.EscapeVelocityCMSec = Env.EscapeVelocity(planet.MassSM, planet.RadiusKM);

                planet.Atmosphere.SurfacePressure = 92000;
                planet.DaytimeTempKelvin = 737;
                planet.NighttimeTempKelvin = 737;
                planet.SurfaceTempKelvin = 737;
                planet.SurfaceGravityG = 0.9;
                planet.MolecularWeightRetained = Env.MinMolecularWeight(planet);

                return planet;
            }

            [TestCategory("Burrows.IScienceAstronomy")]
            [TestMethod]
            public void GetDayLength()
            {
                RotationalSpeed angularVelocityRadSec = RotationalSpeed.FromRadiansPerSecond(10.0);
                Duration orbitalPeriod = Duration.FromDays(600.0);
                Ratio eccentricity = Ratio.FromDecimalFractions(0.5);

                Assert.AreEqual(Env.DayLength(angularVelocityRadSec.RadiansPerSecond, orbitalPeriod.Days, eccentricity.Value), 
                                _phy.Astronomy.GetDayLength(angularVelocityRadSec, orbitalPeriod, eccentricity).Hours,
                                LowDelta);
            }

            [TestCategory("Burrows.IScienceAstronomy")]
            [TestMethod]
            public void GetHillSphere()
            {
                Mass sunMass = Mass.FromSolarMasses(1.0);
                Mass massSM = Mass.FromEarthMasses(1.0);
                Length semiMajorAxisAU = Length.FromAstronomicalUnits(1.0);

                Assert.AreEqual(Env.SimplifiedHillSphereAU(sunMass.SolarMasses, massSM.SolarMasses, semiMajorAxisAU.AstronomicalUnits), 
                                _phy.Astronomy.GetHillSphere(sunMass, massSM, semiMajorAxisAU).AstronomicalUnits,
                                LowDelta);
            }

            [TestCategory("Burrows.IScienceAstronomy")]
            [TestMethod]
            public void GetMinimumIllumination()
            {
                Length a = Length.FromAstronomicalUnits(1.0);
                Luminosity l = Luminosity.FromSolarLuminosities(1.0);

                Assert.AreEqual(Env.MinimumIllumination(a.AstronomicalUnits, l.SolarLuminosities), 
                                _phy.Astronomy.GetMinimumIllumination(a, l).DecimalFractions,
                                LowDelta);
            }

            [TestCategory("Burrows.IScienceAstronomy")]
            [TestMethod]
            public void GetOrbitalZone()
            {
                Luminosity luminosity = Luminosity.FromSolarLuminosities(1.0);
                Length orbitalRadius = Length.FromAstronomicalUnits(1.0);

                Assert.AreEqual(Env.OrbitalZone(luminosity.SolarLuminosities, orbitalRadius.AstronomicalUnits),
                                _phy.Astronomy.GetOrbitalZone(luminosity, orbitalRadius)
                                );
            }

            [TestCategory("Burrows.IScienceAstronomy")]
            [TestMethod]
            public void GetPeriod()
            {
                Length separation = Length.FromAstronomicalUnits(1.0);
                Mass smallMass = Mass.FromEarthMasses(1.0);
                Mass largeMass = Mass.FromSolarMasses(1.0);

                Assert.AreEqual(Env.Period(separation.AstronomicalUnits, smallMass.SolarMasses, largeMass.SolarMasses), 
                                _phy.Astronomy.GetPeriod(separation, smallMass, largeMass).Days,
                                LowDelta);
            }


            [TestCategory("Burrows.IScienceDynamics")]
            [TestMethod]
            public void GetAngularVelocity()
            {
                Mass massSM = Mass.FromEarthMasses(1.0);
                Length radiusKM = Length.FromKilometers(10000.0);
                Density densityGCC = Density.FromGramsPerCubicCentimeter(2.0);
                Length semiMajorAxisAU = Length.FromAstronomicalUnits(1.0);
                bool isGasGiant = false;
                Mass largeMassSM = Mass.FromSolarMasses(1.0);
                Duration largeAgeYears = Duration.FromYears365(1e10);

                Assert.AreEqual(Env.AngularVelocity(massSM.SolarMasses,
                                                         radiusKM.Kilometers,
                                                         densityGCC.GramsPerCubicCentimeter,
                                                         semiMajorAxisAU.AstronomicalUnits,
                                                         isGasGiant,
                                                         largeMassSM.SolarMasses,
                                                         largeAgeYears.Years365), 
                        _phy.Dynamics.GetAngularVelocity(massSM,
                                                         radiusKM,
                                                         densityGCC,
                                                         semiMajorAxisAU,
                                                         isGasGiant,
                                                         largeMassSM,
                                                         largeAgeYears).RadiansPerSecond,
                                LowDelta);
            }

            [TestCategory("Burrows.IScienceDynamics")]
            [TestMethod]
            public void GetBaseAngularVelocity()
            {
                Mass massSM = Mass.FromSolarMasses(1.0);
                Length radiusKM = Length.FromKilometers(5000.0);
                bool isGasGiant = false;

                Assert.AreEqual(Env.BaseAngularVelocity(massSM.SolarMasses,
                                         radiusKM.Kilometers,
                                         isGasGiant), 
                               _phy.Dynamics.GetBaseAngularVelocity(massSM,
                                         radiusKM,
                                         isGasGiant).RadiansPerSecond,
                HighDelta);
            }

            [TestCategory("Burrows.IScienceDynamics")]
            [TestMethod]
            public void GetChangeInAngularVelocity()
            {
                Density densityGCC = Density.FromGramsPerCubicCentimeter(2.5);
                Mass massSM = Mass.FromEarthMasses(0.75);
                Length radiusKM = Length.FromKilometers(2500.0);
                Length semiMajorAxisAU = Length.FromAstronomicalUnits(1.4);
                Mass largeMassSM = Mass.FromSolarMasses(1.5);

                Assert.AreEqual(Env.ChangeInAngularVelocity(densityGCC.GramsPerCubicCentimeter,
                                         massSM.SolarMasses,
                                         radiusKM.Kilometers,
                                         semiMajorAxisAU.AstronomicalUnits,
                                         largeMassSM.SolarMasses), 
                               _phy.Dynamics.GetChangeInAngularVelocity(densityGCC,
                                         massSM,
                                         radiusKM,
                                         semiMajorAxisAU,
                                         largeMassSM).RadiansPerSecond,
                LowDelta);
            }

            [TestCategory("Burrows.IScienceDynamics")]
            [TestMethod]
            public void GetEscapeVelocity()
            {
                Mass massSM = Mass.FromEarthMasses(2.0);
                Length radius = Length.FromKilometers(10000.0);

                Assert.AreEqual(Env.EscapeVelocity(massSM.SolarMasses,
                                                radius.Kilometers) / 1e5, 
                               _phy.Dynamics.GetEscapeVelocity(massSM,
                                                                radius).KilometersPerSecond,
                VeryHighDelta);
            }

            [TestCategory("Burrows.ISciencePhysics")]
            [TestMethod]
            public void GetMolecularWeightRetained()
            {
                SatelliteBody p1 = GetTestPlanetAtmosphere();
                Data.Planet p2 = GetTestPlanetAtmosphereStarform();

                Assert.AreEqual(Env.MinMolecularWeight(p2),
                                _phy.Physics.GetMolecularWeightRetained(p1.SurfaceAcceleration,
                                                        p1.Mass,
                                                        p1.Radius,
                                                        p1.ExosphereTemperature,
                                                        p1.Parent.Age).Grams,
                UnitDelta);
            }

            [TestCategory("Burrows.ISciencePhysics")]
            [TestMethod]
            public void GetGasLife()
            {
                //SatelliteBody p1 = GetTestPlanetAtmosphere();
                //Data.Planet p2 = GetTestPlanetAtmosphereStarform();

                Mass mwr = Mass.FromGrams(14.5);
                Temperature t = Temperature.FromKelvins(6000.0);
                Acceleration a = Acceleration.FromStandardGravity(0.9);
                Length r = Length.FromKilometers(6500.0);

                Assert.AreEqual(Env.GasLife(mwr.Grams,
                                            t.Kelvins,
                                            a.StandardGravity,
                                            r.Kilometers), 
                                _phy.Physics.GetGasLife(mwr,
                                                        t,
                                                        a,
                                                        r).Years365,
                LowDelta);
            }

            [TestCategory("Burrows.ISciencePhysics")]
            [TestMethod]
            public void GetInitialMolecularWeightRetained()
            {
                SatelliteBody p1 = GetTestPlanetAtmosphere();
                Data.Planet p2 = GetTestPlanetAtmosphereStarform();

                Assert.AreEqual(Env.MoleculeLimit(p2.MassSM, p2.RadiusKM, p2.ExosphereTempKelvin), 
                               _phy.Physics.GetInitialMolecularWeightRetained(p1.Mass,
                                                                        p1.Radius,
                                                                        p1.ExosphereTemperature).Grams,
                VeryHighDelta);
            }

            [TestCategory("Burrows.ISciencePhysics")]
            [TestMethod]
            public void GetRMSVelocity()
            {
                Mass molecularWeight = Mass.FromGrams(14.0);
                Temperature exoTemp = Temperature.FromKelvins(5000.0);

                Assert.AreEqual(Env.RMSVelocity(molecularWeight.Grams,
                                                exoTemp.Kelvins), 
                               _phy.Physics.GetRMSVelocity(molecularWeight,
                                                                exoTemp).CentimetersPerSecond,
                LowDelta);
            }
        }
    }
}
