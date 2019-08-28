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

		public bool IsDefined { get; protected set; } = false;
		public StellarBody StellarBody { get => Parent.StellarBody; }
		public SatelliteBody Parent { get; internal set; }
		public Length Thickness { get; protected set; }

		/// <summary>
		/// Total mass of the Layer. 
		/// </summary>
		/// <remarks>
		/// Combined mass of all layers should equal SatelliteBody mass. There is currently no automated way to do this.
		/// </remarks>
		public virtual Mass Mass { get; protected set; }

		public virtual Density MeanDensity
		{
			get
			{
				return Mass / Volume;
			}
		}

		public virtual Temperature MeanTemperature { get; set; } = Temperature.Zero;

		/// <summary>
		/// TODO: Add Unit Test.
		/// </summary>
		public virtual Volume Volume
		{
			get
			{
				var belowrad = Parent.Layers.ComputeThicknessBelow(this).Kilometers;
				var aboverad = belowrad + Thickness.Kilometers;
				var outer = 4.0 / 3.0 * Math.PI * Math.Pow(aboverad, 3.0);
				var inner = 4.0 / 3.0 * Math.PI * Math.Pow(belowrad, 3.0);

				return Volume.FromCubicKilometers(outer - inner);
			}
		}

		public virtual Area UpperBoundaryArea
		{
			get
			{
				if (Parent is null)
					return Area.Zero;

				var aboverad = Parent.Layers.ComputeThicknessBelow(this).Kilometers + Thickness.Kilometers;
				var outer = 4.0 * Math.PI * Math.Pow(aboverad, 2.0);

				return Area.FromSquareKilometers(outer);
			}
		}

		public virtual Area LowerBoundaryArea
		{
			get
			{
				if (Parent is null)
					return Area.Zero;

				var belowrad = Parent.Layers.ComputeThicknessBelow(this).Kilometers;
				var inner = 4.0 * Math.PI * Math.Pow(belowrad, 2.0);

				return Area.FromSquareKilometers(inner);
			}
		}

		protected IList<(Chemical, Ratio)> CompositionInternal { get; set; } = new List<ValueTuple<Chemical, Ratio>>();
		public IEnumerable<ValueTuple<Chemical, Ratio>> Composition { get => CompositionInternal; }

		public Layer(Length thickness)
		{
			Thickness = thickness;
		}

		public Layer(Length thickness, IEnumerable<(Chemical, Ratio)> composition) : this(thickness)
		{
			CompositionInternal = new List<(Chemical, Ratio)>(composition);
		}

		public abstract Layer Generate(SatelliteBody parentBody, Mass availableMass, IEnumerable<Chemical> availableChems, IEnumerable<Layer> curLayers);

		protected internal abstract void OnAddedToStack();

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
				   MeanTemperature.Equals(other.MeanTemperature) &&
				   EqualityComparer<IList<(Chemical, Ratio)>>.Default.Equals(CompositionInternal, other.CompositionInternal) &&
				   EqualityComparer<IEnumerable<(Chemical, Ratio)>>.Default.Equals(Composition, other.Composition);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(StellarBody, Parent, Thickness, Mass, MeanTemperature, CompositionInternal, Composition);
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
