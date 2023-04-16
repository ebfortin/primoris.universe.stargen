
using System;
using System.Collections.Generic;
using Primoris.Universe.Stargen.Bodies;

namespace Primoris.Universe.Stargen.Systems
{

	[Obsolete]
    [Serializable]
	public class StellarSystem
	{
		public string Name { get; set; } = String.Empty;
		public StellarBody? Star { get; set; }
		public IEnumerable<SatelliteBody>? Planets { get; set; }
		public SystemGenerationOptions? Options { get; set; }

		public override string ToString()
		{
			return Name;
		}
	}
}
