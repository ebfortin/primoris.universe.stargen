using System;
using System.Collections.Generic;
using System.Text;
using UnitsNet;
using Primoris.Universe.Stargen.Astrophysics;


namespace Primoris.Universe.Stargen.Bodies
{
	public class GaseousLayer : HomogeneousLayer
	{
		public GaseousLayer(SatelliteBody parent) : base(parent)
		{
		}

		public GaseousLayer(IScienceAstrophysics phy, SatelliteBody parent) : base(phy, parent)
		{
		}

		public Pressure LowerBoundaryPressure { get; protected set; }
		public Breathability Breathability { get; protected set; }
		public IEnumerable<Chemical> PoisonousGases { get; protected set; }

		public override void CalculateComposition()
		{
			throw new NotImplementedException();
		}

		public override void CalculateComposition(IEnumerable<Chemical> availableChems)
		{
			throw new NotImplementedException();
		}
	}
}
