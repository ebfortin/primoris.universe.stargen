using System;

namespace Primoris.Universe.Stargen.Systems
{
	using System.Collections.Generic;

	[Obsolete]
	public class StellarGroup
	{
		public int Seed;
		public SystemGenerationOptions? GenOptions;
		public List<StellarSystem>? Systems;
	}
}
