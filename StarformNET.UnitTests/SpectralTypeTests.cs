using Microsoft.VisualStudio.TestTools.UnitTesting;
using Primoris.Universe.Stargen;
using Primoris.Universe.Stargen.Data;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Primoris.Universe.Stargen.UnitTests
{
	class SpectralTypeTests
	{
		[TestClass]
		public class SpectralConversionTests
		{
			/// <summary>
			/// Check with a known star (GAIA DR2 1872046574983507456).
			/// </summary>
			[TestCategory("SpectralType Conversions")]
			[TestMethod]
			public void TestLuminosityToSpectral()
			{
				var st1 = SpectralType.FromLuminosity(0.16378798, 0.72014487);
				Console.WriteLine(st1.ToString());
				var st2 = SpectralType.FromString(st1.ToString());
				Console.WriteLine(st2.Temperature);
				Console.WriteLine(st2.LuminosityClass);
				Console.WriteLine(st2.SpectralClass);
				Assert.IsTrue(Math.Abs(st2.Temperature - 4327.0) <= 300.0);

				var st3 = SpectralType.FromLuminosity(78.0, 8.6);
				Console.WriteLine(st3.Temperature);
			}

			[TestCategory("SpectralType Conversions")]
			[TestMethod]
			public void TestTemperatureToSpectral()
			{
				var st1 = SpectralType.FromTemperature(5000.0);
				Console.WriteLine(st1.ToString());
				var st2 = SpectralType.FromString(st1.ToString());
				Console.WriteLine(st2.Temperature);
				Assert.AreEqual(5250.0, st2.Temperature);

				st1 = SpectralType.FromTemperature(55000.0);
				Console.WriteLine(st1.ToString());
				st2 = SpectralType.FromString(st1.ToString());
				Console.WriteLine(st2.Temperature);
				Assert.AreEqual(50000.0, st2.Temperature);

				st1 = SpectralType.FromTemperature(5900.0);
				Console.WriteLine(st1.ToString());
				st2 = SpectralType.FromString(st1.ToString());
				Console.WriteLine(st2.Temperature);
				Assert.AreEqual(5940.0, st2.Temperature);

				st1 = SpectralType.FromTemperature(3340.0);
				Console.WriteLine(st1.ToString());
				st2 = SpectralType.FromString(st1.ToString());
				Console.WriteLine(st2.Temperature);
				Assert.AreEqual(3720.0, st2.Temperature);
			}
		}
	}
}
