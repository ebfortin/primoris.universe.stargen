using Microsoft.VisualStudio.TestTools.UnitTesting;
using Primoris.Universe.Stargen;
using System.Linq;
using System.Collections.Generic;
using System;
using Primoris.Universe.Stargen.Astrophysics;

namespace Primoris.Universe.Stargen.UnitTests
{
	class SpectralTypeTests
	{
		[TestClass]
		public class StellarConversionTests
		{
			/// <summary>
			/// Check with a known star (GAIA DR2 1872046574983507456).
			/// </summary>
			[TestCategory("StellarType Conversions")]
			[TestMethod]
			public void TestLuminosityToSpectral()
			{
				var st1 = StellarType.FromLuminosityAndRadius(0.16378798, 0.72014487);
				Console.WriteLine(st1.ToString());
				Console.WriteLine("Mass = " + st1.Mass);
				Console.WriteLine("Temperature = " + st1.Temperature);
				Console.WriteLine("Luminosity = " + st1.Luminosity);
				Console.WriteLine("Radius = " + st1.Radius);

				var st2 = StellarType.FromString(st1.ToString());
				Console.WriteLine(st2.SpectralClass);
				Console.WriteLine(st2.LuminosityClass);
				Console.WriteLine(st2.SubType);
				Console.WriteLine("Mass = " + st2.Mass);
				Console.WriteLine("Temperature = " + st2.Temperature);
				Console.WriteLine("Luminosity = " + st2.Luminosity);
				Console.WriteLine("Radius = " + st2.Radius);

				Assert.IsTrue(Math.Abs(st2.Temperature - 4327.0) <= 150.0);
			}

			[TestCategory("StellarType Creation")]
			[TestMethod]
			public void TestTemperatureAndLuminosity()
			{
				StellarType st = StellarType.FromTemperatureAndLuminosity(20000.0, 2.0);
				Console.WriteLine(st.ToString());
				Console.WriteLine(st.Luminosity);
				Console.WriteLine(st.Mass);
				Console.WriteLine(st.Temperature);
				Console.WriteLine(st.Radius);
			}

			/// <summary>
			/// Check with a known star (GAIA DR2 828929527040).
			/// </summary>
			[TestCategory("StellarType Conversions")]
			[TestMethod]
			public void TestTemperatureToSpectral()
			{
				var st1 = StellarType.FromTemperatureAndLuminosity(4417.3335, 0.17183004);
				Console.WriteLine(st1.ToString());
				Console.WriteLine(st1.Temperature);
				Console.WriteLine(st1.Luminosity);

				Assert.AreEqual(4417.3335, st1.Temperature);
				Assert.AreEqual(0.17183004, st1.Luminosity);

				var st2 = StellarType.FromString(st1.ToString());
				Console.WriteLine(st2.Temperature);
				Console.WriteLine(st2.Luminosity);

				Assert.IsTrue(Math.Abs(st2.Temperature - 4417.3335) <= 150.0);
			}
		}
	}
}
