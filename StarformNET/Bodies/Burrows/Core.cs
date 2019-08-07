using System;
using System.Collections.Generic;
using System.Text;
using Primoris.Universe.Stargen.Astrophysics;

namespace Primoris.Universe.Stargen.Bodies.Burrows
{
	public class Core : SolidLayer
	{
		public Core(SatelliteBody parent) : base(parent)
		{
		}

		public Core(IScienceAstrophysics phy, SatelliteBody parent) : base(phy, parent)
		{
		}

		public override void CalculateComposition()
		{
			Thickness = Parent.CoreRadius;
			MeanDensity = Parent.Density;
			MeanTemperature = Parent.Temperature;
		}

		public override void CalculateComposition(IEnumerable<Chemical> availableChems)
		{
			CalculateComposition();
		}
	}
}
