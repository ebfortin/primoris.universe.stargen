using System;
using System.Collections.Generic;
using System.Text;
using Primoris.Universe.Stargen.Astrophysics;
using UnitsNet;


namespace Primoris.Universe.Stargen.Bodies.Burrows;

public class BasicSolidLayer : SolidLayer
{


	public BasicSolidLayer(LayerStack stack, Mass mass, Length thickness, IEnumerable<(Chemical, Ratio)> composition) 
		: base(stack, mass, thickness, composition)
	{
	}

}
