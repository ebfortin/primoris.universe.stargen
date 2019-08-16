using System;
using System.Collections.Generic;
using System.Text;
using Primoris.Universe.Stargen.Astrophysics;
using UnitsNet;


namespace Primoris.Universe.Stargen.Bodies.Burrows
{
	public class BasicSolidLayer : SolidLayer
	{
		public BasicSolidLayer() : base()
		{
		}

		public override Layer Generate(SatelliteBody parentBody, Mass availableMass, IEnumerable<Chemical> availableChems, IEnumerable<Layer> curLayers)
		{
			Thickness = parentBody.CoreRadius;
			//MeanDensity = parentBody.Density;
			MeanTemperature = parentBody.Temperature;
			Mass = availableMass;

			return this;
		}
	}
}
