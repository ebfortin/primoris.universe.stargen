using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using UnitsNet;
using Primoris.Universe.Stargen.Astrophysics;
using Primoris.Universe.Stargen.Services;
using System.Diagnostics.CodeAnalysis;

namespace Primoris.Universe.Stargen.Bodies
{
	public abstract class Layer : IEquatable<Layer>
	{
		private IScienceAstrophysics _phy = null;
		public IScienceAstrophysics Science { get => _phy is null ? Provider.Use().GetService<IScienceAstrophysics>() : _phy; set => _phy = value; }

		public StellarBody StellarBody { get => Parent.StellarBody; }
		public SatelliteBody Parent { get; internal set; }
		public Length Thickness { get; protected set; }

		/// <summary>
		/// Total mass of the Layer. 
		/// </summary>
		/// <remarks>
		/// Combined mass of all layers should equal SatelliteBody mass. There is currently no automated way to do this.
		/// </remarks>
		public Mass Mass { get; protected set; }
		public virtual Density MeanDensity { get; protected set; }
		public virtual Temperature MeanTemperature { get; protected set; }

		protected IList<ValueTuple<Chemical, Ratio>> CompositionInternal { get; } = new List<ValueTuple<Chemical, Ratio>>();
		public IEnumerable<ValueTuple<Chemical, Ratio>> Composition { get => CompositionInternal; }

		public Layer()
		{
		}

		public Layer(IEnumerable<ValueTuple<Chemical, Ratio>> composition)
		{
			CompositionInternal = new List<ValueTuple<Chemical, Ratio>>(composition);
		}

		public abstract Layer Generate(SatelliteBody parentBody, Mass availableMass, IEnumerable<Chemical> availableChems, IEnumerable<Layer> curLayers);

		public override bool Equals(object obj)
		{
			return Equals(obj as Layer);
		}

		public bool Equals([AllowNull] Layer other)
		{
			return other != null &&
				   EqualityComparer<StellarBody>.Default.Equals(StellarBody, other.StellarBody) &&
				   EqualityComparer<SatelliteBody>.Default.Equals(Parent, other.Parent) &&
				   Thickness.Equals(other.Thickness) &&
				   Mass.Equals(other.Mass) &&
				   MeanDensity.Equals(other.MeanDensity) &&
				   MeanTemperature.Equals(other.MeanTemperature) &&
				   EqualityComparer<IList<(Chemical, Ratio)>>.Default.Equals(CompositionInternal, other.CompositionInternal) &&
				   EqualityComparer<IEnumerable<(Chemical, Ratio)>>.Default.Equals(Composition, other.Composition);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(StellarBody, Parent, Thickness, Mass, MeanDensity, MeanTemperature, CompositionInternal, Composition);
		}

		public static bool operator ==(Layer left, Layer right)
		{
			return EqualityComparer<Layer>.Default.Equals(left, right);
		}

		public static bool operator !=(Layer left, Layer right)
		{
			return !(left == right);
		}
	}
}
