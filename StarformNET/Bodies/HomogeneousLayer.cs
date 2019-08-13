using System;
using System.Collections.Generic;
using System.Text;
using Primoris.Universe.Stargen.Astrophysics;
using UnitsNet;

namespace Primoris.Universe.Stargen.Bodies
{
	public abstract class HomogeneousLayer : Layer
	{
		public HomogeneousLayer() : base()
		{
		}

		public HomogeneousLayer(IEnumerable<(Chemical, Ratio)> composition) : base(composition)
		{
		}
	}
}
