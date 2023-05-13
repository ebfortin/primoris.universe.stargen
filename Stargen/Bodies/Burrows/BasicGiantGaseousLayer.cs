using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Primoris.Universe.Stargen.Astrophysics;
using UnitsNet;
using System.Reflection.Metadata.Ecma335;

namespace Primoris.Universe.Stargen.Bodies.Burrows;

public class BasicGiantGaseousLayer : BasicGaseousLayer
{
	public BasicGiantGaseousLayer(SatelliteBody parent, Length thickness) 
		: base(parent, thickness, Pressure.Zero) 
	{ 
	}

	public BasicGiantGaseousLayer(SatelliteBody parent, Length thickness, Pressure surfPres) 
		: base(parent, thickness, surfPres)
	{
	}

	protected override void OnGenerate(Mass availableMass, IEnumerable<Chemical> availableChems, IEnumerable<Layer> curLayers)
	{
		return;
	}

	protected internal override void OnAddedToStack()
	{
	}
}
