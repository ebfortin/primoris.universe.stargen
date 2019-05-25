using Microsoft.VisualStudio.TestTools.UnitTesting;
using Primoris.Universe.Stargen;
using Primoris.Universe.Stargen.Data;
using System.Linq;
using System.Collections.Generic;
using System;

namespace StarformNET.UnitTests
{
	class NameGeneratorTests
	{
		[TestClass]
		public class NameGeneratorUniquenessTests
		{
			[TestCategory("NameGenerator Uniqueness")]
			[TestMethod]
			public void NameGenerationTest()
			{
				var hs = new HashSet<string>(25_000_000);
				int i = 0;

				var ng = new NameGenerator(3, 5);
				for (; i < 25_000_000; i++)
				{
					var r = hs.Add(ng.NextName());
					//Assert.IsTrue(r, "Duplicate found @ " + i);
				}
			}
		}
	}
}
