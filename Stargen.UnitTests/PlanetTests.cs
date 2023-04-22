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
			return new Planet(seed, star) { Position = pos, Name = planetID };
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

		[TestCategory("Planet.Atmosphere")]
		[TestMethod]
		public void TestAtmosphereComposition()
		{
			var star = new Star();
			var seed = new Seed(Length.FromAstronomicalUnits(1.0), Ratio.FromDecimalFractions(1.0), Mass.FromEarthMasses(1.0), Mass.FromEarthMasses(1.0), Mass.Zero);

			var layers = new List<Layer>()
			{
				new BasicSolidLayer(Length.FromKilometers(10000.0), Mass.FromEarthMasses(1.0), new (Chemical, Ratio)[0]),
				new BasicGaseousLayer(Length.FromKilometers(100.0), new List<(Chemical, Ratio)>() { (Chemical.All["N"], Ratio.FromDecimalFractions(0.50)) }, Pressure.FromBars(0.5)),
				new BasicGaseousLayer(Length.FromKilometers(100.0), new List<(Chemical, Ratio)>() { (Chemical.All["N"], Ratio.FromDecimalFractions(0.25)) }, Pressure.FromBars(0.25))
			};
			var planet = new Planet(seed, star, layers);

			var ele = planet.AtmosphereComposition.ElementAt(0);
			Assert.AreEqual(Math.Round((0.50 * 0.5 + 0.25 * 0.25) / 0.75, 2, MidpointRounding.ToNegativeInfinity), Math.Round(ele.Item2.DecimalFractions, 2, MidpointRounding.ToNegativeInfinity));
		}
	}
}
