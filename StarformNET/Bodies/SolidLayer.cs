using System;
using System.Collections.Generic;
using System.Text;
using Primoris.Universe.Stargen.Astrophysics;
using UnitsNet;


namespace Primoris.Universe.Stargen.Bodies
{
	public abstract class SolidLayer : HomogeneousLayer
	{
		public SolidLayer() : base()
		{
		}

		public SolidLayer(IEnumerable<(Chemical, Ratio)> composition) : base(composition)
		{
		}
	}
}
