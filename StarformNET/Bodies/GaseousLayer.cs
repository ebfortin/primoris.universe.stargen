using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using UnitsNet;
using Primoris.Universe.Stargen.Astrophysics;


namespace Primoris.Universe.Stargen.Bodies
{
	public abstract class GaseousLayer : HomogeneousLayer
	{
		public GaseousLayer(SatelliteBody parent, Pressure surfPres) : base(parent)
		{
			LowerBoundaryPressure = surfPres;
		}

		public GaseousLayer(IScienceAstrophysics phy, SatelliteBody parent, Pressure surfPres) : base(phy, parent)
		{
			LowerBoundaryPressure = surfPres;
		}

		public Pressure LowerBoundaryPressure { get; protected set; }
		public Breathability Breathability { get; protected set; }

		protected IList<Chemical> PoisonousChemicalInternal { get; } = new List<Chemical>();
		public IEnumerable<Chemical> PoisonousChemicals { get; protected set; }

	}
}
