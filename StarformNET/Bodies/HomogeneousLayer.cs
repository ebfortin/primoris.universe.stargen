using System;
using System.Collections.Generic;
using System.Text;
using Primoris.Universe.Stargen.Astrophysics;

namespace Primoris.Universe.Stargen.Bodies
{
	public abstract class HomogeneousLayer : Layer
	{
		public HomogeneousLayer(SatelliteBody parent) : base(parent)
		{
		}

		public HomogeneousLayer(IScienceAstrophysics phy, SatelliteBody parent) : base(phy, parent)
		{
		}
	}
}
