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

		public SatelliteBody Parent { get; }
		public Length Thickness { get; protected set; }
		public virtual Density MeanDensity { get; protected set; }
		public virtual Temperature MeanTemperature { get; protected set; }

		protected List<Chemical> CompositionInternal { get; } = new List<Chemical>();
		public IEnumerable<Chemical> Composition { get => CompositionInternal; }

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

			var c1 = new List<Chemical>(Composition);
			var c2 = new List<Chemical>(p.Composition);

			bool eq = true;
			foreach(var c in c1)
			{
				if (c1.Contains(c))
					continue;
				else
					break;
			}

			return Parent == p.Parent && Thickness == p.Thickness && MeanDensity == p.MeanDensity && MeanTemperature == p.MeanTemperature;
		}

	}
}
