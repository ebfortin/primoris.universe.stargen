using System;
using System.Collections.Generic;
using System.Text;

namespace Primoris.Universe.Stargen.Bodies
{
	public abstract class Body
	{
		public string Name { get; protected set; }

		public abstract double MassSM { get; protected set; }

		// TODO: Make it used by Star. Right now Planets reside in StellarSystem. 
		public IEnumerable<SatelliteBody> Satellites { get; protected set; }
	}
}
