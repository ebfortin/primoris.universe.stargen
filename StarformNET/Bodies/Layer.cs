using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using UnitsNet;
using Primoris.Universe.Stargen.Astrophysics;
using Primoris.Universe.Stargen.Services;


namespace Primoris.Universe.Stargen.Bodies
{
	public abstract class Layer
	{
		public IScienceAstrophysics Science { get; set; }

		public StellarBody StellarBody { get => Parent.StellarBody; }
		public SatelliteBody Parent { get; }
		public Length Thickness { get; protected set; }
		public virtual Density MeanDensity { get; protected set; }
		public virtual Temperature MeanTemperature { get; protected set; }

		protected IList<ValueTuple<Chemical, Ratio>> CompositionInternal { get; } = new List<ValueTuple<Chemical, Ratio>>();
		public IEnumerable<ValueTuple<Chemical, Ratio>> Composition { get => CompositionInternal; }

		public Layer(SatelliteBody parent) : this(parent.Science, parent) { }
		public Layer(IScienceAstrophysics phy, SatelliteBody parent)
		{
			Science = phy;
			Parent = parent;
		}

		public abstract void CalculateComposition();

		public abstract void CalculateComposition(IEnumerable<Chemical> availableChems);

		public override bool Equals(object? obj)
		{
			if (!(obj is Layer))
				return false;

			var p = (Layer)obj;

			var c1 = new List<Chemical>(from c in Composition select c.Item1);
			var c2 = new List<Chemical>(from c in p.Composition select c.Item1);

			bool eq = true;
			foreach(var c in c1)
			{
				if (c2.Contains(c))
					continue;
				else
				{
					eq = false;
					break;
				}
			}

			if (!eq)
				return false;

			return Parent == p.Parent && 
				   Utilities.AlmostEqual(Thickness.Kilometers, p.Thickness.Kilometers) && 
				   Utilities.AlmostEqual(MeanDensity.GramsPerCubicCentimeter, p.MeanDensity.GramsPerCubicCentimeter) && 
				   Utilities.AlmostEqual(MeanTemperature.Kelvins, p.MeanTemperature.Kelvins);
		}

	}
}
