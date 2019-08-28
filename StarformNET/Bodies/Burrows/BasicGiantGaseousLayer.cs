using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Primoris.Universe.Stargen.Astrophysics;
using UnitsNet;

namespace Primoris.Universe.Stargen.Bodies.Burrows
{
	public class BasicGiantGaseousLayer : BasicGaseousLayer
	{
		public BasicGiantGaseousLayer(Length thickness) : base(thickness, Pressure.Zero) 
		{ 
		}

		public BasicGiantGaseousLayer(Length thickness, Pressure surfPres) : base(thickness, surfPres)
		{
		}

		public override Layer Generate(SatelliteBody parentBody, Mass availableMass, IEnumerable<Chemical> availableChems, IEnumerable<Layer> curLayers)
		{
			return base.Generate(parentBody, availableMass, availableChems, curLayers);
		}
	}
}
