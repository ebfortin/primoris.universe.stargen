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
	public BasicGiantGaseousLayer(LayerStack stack, Mass mass, Length thickness) 
		: base(stack, mass, thickness) 
	{ 
	}

    public BasicGiantGaseousLayer(LayerStack stack, Mass mass, Length thickness, IEnumerable<Chemical> availableChems)
    : base(stack, mass, thickness, availableChems)
    {
    }

}
