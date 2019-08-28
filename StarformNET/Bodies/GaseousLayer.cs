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
		public GaseousLayer(Length thickness, Pressure surfPres) : base(thickness)
		{
			LowerBoundaryPressure = surfPres;
			//Mass = GetMassFromPressure(surfPres);
		}

		public GaseousLayer(Length thickness, IEnumerable<(Chemical, Ratio)> composition, Pressure surfPres) : base(thickness, composition)
		{
			LowerBoundaryPressure = surfPres;
			//Mass = GetMassFromPressure(surfPres);
		}

		public Pressure LowerBoundaryPressure { get; protected set; }

		public Breathability Breathability { get; protected set; } = Breathability.None;

		// TODO: Have poisonous gas being put in the list. Right now doesn't work.
		protected IList<(Chemical, Ratio)> PoisonousCompositionInternal { get; } = new List<ValueTuple<Chemical, Ratio>>();
		public IEnumerable<(Chemical, Ratio)> PoisonousComposition { get; protected set; } = new ValueTuple<Chemical, Ratio>[0];

		protected internal override void OnAddedToStack()
		{
			Mass = GetMassFromPressure(LowerBoundaryPressure);
		}

		protected virtual Mass GetMassFromPressure(Pressure pres)
		{
			if (Parent is null)
				return Mass.Zero;

			return pres * LowerBoundaryArea / Parent.Layers.ComputeAccelerationAt(this);
		}
	}
}
