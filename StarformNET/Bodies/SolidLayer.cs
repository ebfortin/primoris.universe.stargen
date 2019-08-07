using System;
using System.Collections.Generic;
using System.Text;
using Primoris.Universe.Stargen.Astrophysics;

namespace Primoris.Universe.Stargen.Bodies
{
	public abstract class SolidLayer : HomogeneousLayer
	{
		public SolidLayer(SatelliteBody parent) : base(parent)
		{
		}

		public SolidLayer(IScienceAstrophysics phy, SatelliteBody parent) : base(phy, parent)
		{
		}

	}
}
