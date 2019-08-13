using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Primoris.Universe.Stargen.Astrophysics;
using UnitsNet;

namespace Primoris.Universe.Stargen.Bodies.Burrows
{
	public class BasicGiantGaseousLayer : GaseousLayer
	{
		public BasicGiantGaseousLayer() : base(Pressure.Zero) { }

		public BasicGiantGaseousLayer(Pressure surfPres) : base(surfPres)
		{
		}

		public override Layer Generate(SatelliteBody parentBody, Mass availableMass, IEnumerable<Chemical> availableChems, IEnumerable<Layer> curLayers)
		{
			if (curLayers.Count() != 0)
				throw new InvalidBodyLayerSequenceException();

			Mass = availableMass;

			return this;
		}
	}
}
