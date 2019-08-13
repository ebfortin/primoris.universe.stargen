using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using UnitsNet;
using Primoris.Universe.Stargen.Astrophysics;


namespace Primoris.Universe.Stargen.Bodies
{
	public abstract class GaseousLayer : HomogeneousLayer
	{
		public GaseousLayer(Pressure surfPres) : base()
		{
			LowerBoundaryPressure = surfPres;
		}

		public GaseousLayer(IEnumerable<(Chemical, Ratio)> composition, Pressure surfPres) : base(composition)
		{
			LowerBoundaryPressure = surfPres;
		}

		public Pressure LowerBoundaryPressure { get; protected set; }
		public Breathability Breathability { get; protected set; } = Breathability.None;

		// TODO: Have poisonous gas being put in the list. Right now doesn't work.
		protected IList<ValueTuple<Chemical, Ratio>> PoisonousCompositionInternal { get; } = new List<ValueTuple<Chemical, Ratio>>();
		public IEnumerable<ValueTuple<Chemical, Ratio>> PoisonousComposition { get; protected set; } = new ValueTuple<Chemical, Ratio>[0];

	}
}
