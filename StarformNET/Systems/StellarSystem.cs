
using System;
using System.Collections.Generic;
using Primoris.Universe.Stargen.Bodies;
using Primoris.Universe.Stargen.Data;

namespace Primoris.Universe.Stargen.Systems
{


	[Serializable]
	public class StellarSystem
	{
		public string Name { get; set; }
		public Star Star { get; set; }
		public IEnumerable<Body> Planets { get; set; }
		public SystemGenerationOptions Options { get; set; }

		public override string ToString()
		{
			return Name;
		}
	}
}
