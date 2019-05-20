using Microsoft.VisualStudio.TestTools.UnitTesting;
using DLS.StarformNET;
using DLS.StarformNET.Data;
using System.Linq;
using System.Collections.Generic;
using System;

namespace DLS.StarformNET.UnitTests
{
	class SpectralTypeTests
	{
		[TestClass]
		public class SpectralConversionTests
		{

			[TestCategory("Spectral Type Conversions")]
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
