using System;
using System.Collections.Generic;
using System.Text;
using Primoris.Universe.Stargen.Astrophysics;
using UnitsNet;


namespace Primoris.Universe.Stargen.Bodies
{
	public abstract class SolidLayer : HomogeneousLayer
	{
		public SolidLayer(Length thickness) : base(thickness)
		{
		}

		public SolidLayer(Length thickness, Mass mass, IEnumerable<(Chemical, Ratio)> composition) : base(thickness, composition)
		{
			Mass = mass;
		}

		protected internal override void OnAddedToStack()
		{
			if (Mass == Mass.Zero)
				throw new ArgumentException("A SolidLayer added to a Stack without calling Generate() must have a non zero Mass specified in its constructor.");
		}
	}
}
