using System;
using System.Collections.Generic;
using System.Text;
using Primoris.Universe.Stargen.Astrophysics;
using UnitsNet;


namespace Primoris.Universe.Stargen.Bodies.Burrows;

public class BasicSolidLayer : SolidLayer
{
	public BasicSolidLayer(SatelliteBody parent, Length thickness) 
		: base(parent, thickness)
	{
	}

	public BasicSolidLayer(SatelliteBody parent, Length thickness, Mass mass, IEnumerable<(Chemical, Ratio)> composition) 
		: base(parent, thickness, mass, composition)
	{
	}

	protected override void OnGenerate(Mass availableMass, IEnumerable<Chemical> availableChems, IEnumerable<Layer> curLayers)
	{
		//Thickness = parentBody.CoreRadius;
		//MeanDensity = parentBody.Density;
		MeanTemperature = Parent.Temperature;
		if (Mass > Mass.Zero && !Mass.Equals(availableMass, Extensions.Epsilon, ComparisonType.Relative))
			throw new ArgumentException("Available Mass must be equal to specified mass.");
	
		Mass = availableMass;
	}
}
