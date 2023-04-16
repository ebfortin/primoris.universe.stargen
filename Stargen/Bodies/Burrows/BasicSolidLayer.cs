using System;
using System.Collections.Generic;
using System.Text;
using Primoris.Universe.Stargen.Astrophysics;
using UnitsNet;


namespace Primoris.Universe.Stargen.Bodies.Burrows
{
	public class BasicSolidLayer : SolidLayer
	{
		public BasicSolidLayer(Length thickness) : base(thickness)
		{
		}

		public BasicSolidLayer(Length thickness, Mass mass, IEnumerable<(Chemical, Ratio)> composition) : base(thickness, mass, composition)
		{
		}

		public override Layer Generate(SatelliteBody parentBody, Mass availableMass, IEnumerable<Chemical> availableChems, IEnumerable<Layer> curLayers)
		{
			//Thickness = parentBody.CoreRadius;
			//MeanDensity = parentBody.Density;
			MeanTemperature = parentBody.Temperature;
			if (Mass > Mass.Zero && !Mass.Equals(availableMass, Extensions.Epsilon, ComparisonType.Relative))
				throw new ArgumentException("Available Mass must be equal to specified mass.");
		
			Mass = availableMass;

			return this;
		}
	}
}
