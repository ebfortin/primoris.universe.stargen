using System;
using System.Collections.Generic;
using System.Text;
using Primoris.Universe.Stargen.Astrophysics;
using UnitsNet;


namespace Primoris.Universe.Stargen.Bodies;

/// <summary>
/// Represents a Solid layer of matter in a Body.
/// </summary>
/// <seealso cref="Primoris.Universe.Stargen.Bodies.HomogeneousLayer" />
public abstract class SolidLayer : HomogeneousLayer
{
	/// <summary>
	/// Initializes a new instance of the <see cref="SolidLayer"/> class.
	/// </summary>
	/// <param name="thickness">The thickness.</param>
	/// <param name="mass">The mass.</param>
	/// <param name="composition">The composition.</param>
	public SolidLayer(LayerStack stack, Mass mass, Length thickness, IEnumerable<(Chemical, Ratio)> composition) 
		: base(stack, mass, thickness, Temperature.Zero, composition)
	{
		Mass = mass;
	}

}
