using System;
using System.Collections.Generic;
using System.Text;

namespace Primoris.Universe.Stargen.Bodies
{
	public abstract class Body
	{
		public string Name { get; protected set; }

		public abstract double MassSM { get; protected set; }

		// TODO: Add Satellites here instead of in Star. Will also be used for moons of a planet.
	}
}
