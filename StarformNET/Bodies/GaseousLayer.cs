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

		/// <summary>
		/// Gets or sets the lower boundary pressure.
		/// </summary>
		/// <remarks>
		/// The pressure at the inner surface of the layer. 
		/// </remarks>
		/// <value>
		/// The lower boundary pressure.
		/// </value>
		public Pressure LowerBoundaryPressure { get; protected set; }

		/// <summary>
		/// Gets or sets the breathability.
		/// </summary>
		/// <value>
		/// The breathability.
		/// </value>
		public Breathability Breathability { get; protected set; } = Breathability.None;

		// TODO: Have poisonous gas being put in the list. Right now doesn't work.
		protected IList<(Chemical, Ratio)> PoisonousCompositionInternal { get; } = new List<(Chemical, Ratio)>();

		/// <summary>
		/// Gets or sets the poisonous composition.
		/// </summary>
		/// <value>
		/// The poisonous composition.
		/// </value>
		public IEnumerable<(Chemical, Ratio)> PoisonousComposition { get; protected set; } = new (Chemical, Ratio)[0];

		/// <summary>
		/// Called when [added to stack].
		/// </summary>
		protected internal override void OnAddedToStack()
		{
			Mass = GetMassFromPressure(LowerBoundaryPressure);
		}

		/// <summary>
		/// Gets the mass from pressure.
		/// </summary>
		/// <param name="pres">The pressure.</param>
		/// <returns>Mass</returns>
		protected virtual Mass GetMassFromPressure(Pressure pres)
		{
			if (Parent is null)
				return Mass.Zero;

			return pres * LowerBoundaryArea / Parent.Layers.ComputeAccelerationAt(this);
		}
	}
}
