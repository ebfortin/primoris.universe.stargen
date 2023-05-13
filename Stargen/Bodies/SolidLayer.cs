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
	public SolidLayer(SatelliteBody parent, Length thickness) 
		: base(parent, thickness)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="SolidLayer"/> class.
	/// </summary>
	/// <param name="thickness">The thickness.</param>
	/// <param name="mass">The mass.</param>
	/// <param name="composition">The composition.</param>
	public SolidLayer(SatelliteBody parent, Length thickness, Mass mass, IEnumerable<(Chemical, Ratio)> composition) 
		: base(parent, thickness, composition)
	{
		Mass = mass;
	}

	/// <summary>
	/// Called when [added to stack].
	/// </summary>
	/// <exception cref="ArgumentException">A SolidLayer added to a Stack without calling Generate() must have a non zero Mass specified in its constructor.</exception>
	protected internal override void OnAddedToStack()
	{
		if (Mass.Equals(Mass.Zero, 0.0, ComparisonType.Absolute))
			throw new ArgumentException("A SolidLayer added to a Stack without calling Generate() must have a non zero Mass specified in its constructor.");
	}
}
