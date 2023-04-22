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
	/// <summary>
	/// Body layer.
	/// </summary>
	/// <remarks>
	/// Bodies are formed from different layers one on top of the other.
	/// </remarks>
	/// <seealso cref="System.IEquatable{Primoris.Universe.Stargen.Bodies.Layer}" />
	public abstract class Layer : IEquatable<Layer>
	{
		/// <summary>
		/// Gets or sets the science.
		/// </summary>
		/// <value>
		/// The science.
		/// </value>
		public IScienceAstrophysics Science { get; set; } = IScienceAstrophysics.Default;

		/// <summary>
		/// Gets the stellar body.
		/// </summary>
		/// <remarks>
		/// Get the StellarBody of the system. 
		/// </remarks>
		/// <value>
		/// The stellar body, ie Parent.StellarBody.
		/// </value>
		public StellarBody StellarBody { get => Parent!.StellarBody; }

		/// <summary>
		/// Gets the parent this layer belongs to. 
		/// </summary>
		/// <value>
		/// The parent.
		/// </value>
		public SatelliteBody? Parent { get; internal set; }

		/// <summary>
		/// Gets or sets the thickness of the layer. 
		/// </summary>
		/// <value>
		/// The thickness.
		/// </value>
		public Length Thickness { get; protected set; }

		/// <summary>
		/// Total mass of the Layer. 
		/// </summary>
		/// <remarks>
		/// Combined mass of all layers should equal SatelliteBody mass. There is currently no automated way to do this.
		/// </remarks>
		/// <returns>
		/// Mass of the Layer.
		/// </returns>
		public virtual Mass Mass { get; protected set; }

		/// <summary>
		/// Gets the mean density.
		/// </summary>
		/// <remarks>
		/// MeanDensity equals to Mass / Volume. This property is virtual so derived classes can customize the default value.
		/// </remarks>
		/// <value>
		/// The mean density.
		/// </value>
		public virtual Density MeanDensity => Mass / Volume;

		/// <summary>
		/// Gets or sets the mean temperature.
		/// </summary>
		/// <value>
		/// The mean temperature.
		/// </value>
		public virtual Temperature MeanTemperature { get; set; } = Temperature.Zero;

		/// <summary>
		/// TODO: Add Unit Test.
		/// </summary>
		public virtual Volume Volume
		{
			get
			{
				if (Parent is null)
					return Volume.Zero;

				var belowrad = Parent.ComputeThicknessBelow(this).Kilometers;
				var aboverad = belowrad + Thickness.Kilometers;
				var outer = 4.0 / 3.0 * Math.PI * Math.Pow(aboverad, 3.0);
				var inner = 4.0 / 3.0 * Math.PI * Math.Pow(belowrad, 3.0);

				return Volume.FromCubicKilometers(outer - inner);
			}
		}

		/// <summary>
		/// Gets the upper boundary area.
		/// </summary>
		/// <remarks>
		/// Area encompassing the outer surface of the Layer. Equals the LowerBoundaryArea of the next layer in the stack.
		/// </remarks>
		/// <value>
		/// The upper boundary area.
		/// </value>
		public virtual Area UpperBoundaryArea
		{
			get
			{
				if (Parent is null)
					return Area.Zero;

				var aboverad = Parent.ComputeThicknessBelow(this).Kilometers + Thickness.Kilometers;
				var outer = 4.0 * Math.PI * Math.Pow(aboverad, 2.0);

				return Area.FromSquareKilometers(outer);
			}
		}

		/// <summary>
		/// Gets the lower boundary area.
		/// </summary>
		/// <remarks>
		/// Area encompassing the inner surface of the Layer. Equals the UpperBoundaryArea of the previous layer in the stack.
		/// </remarks>
		/// <value>
		/// The lower boundary area.
		/// </value>
		public virtual Area LowerBoundaryArea
		{
			get
			{
				if (Parent is null)
					return Area.Zero;

				var belowrad = Parent.ComputeThicknessBelow(this).Kilometers;
				var inner = 4.0 * Math.PI * Math.Pow(belowrad, 2.0);

				return Area.FromSquareKilometers(inner);
			}
		}

		/// <summary>
		/// Gets or sets the chemical composition collection of the Layer.
		/// </summary>
		/// <value>
		/// The composition internal.
		/// </value>
		protected IList<(Chemical, Ratio)> CompositionInternal { get; set; } = new List<ValueTuple<Chemical, Ratio>>();

		/// <summary>
		/// Gets the composition.
		/// </summary>
		/// <value>
		/// The composition.
		/// </value>
		public IEnumerable<(Chemical, Ratio)> Composition { get => CompositionInternal; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Layer"/> class.
		/// </summary>
		/// <param name="thickness">The thickness.</param>
		public Layer(Length thickness)
		{
			Thickness = thickness;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Layer"/> class.
		/// </summary>
		/// <param name="thickness">The thickness.</param>
		/// <param name="composition">The composition.</param>
		public Layer(Length thickness, IEnumerable<(Chemical, Ratio)> composition) : this(thickness)
		{
			CompositionInternal = new List<(Chemical, Ratio)>(composition);
		}

		/// <summary>
		/// Generates the Layer characteristics.
		/// </summary>
		/// <param name="parentBody">The parent body.</param>
		/// <param name="availableMass">The available mass.</param>
		/// <param name="availableChems">The available chems.</param>
		/// <param name="curLayers">The current layers.</param>
		/// <returns></returns>
		public abstract Layer Generate(SatelliteBody parentBody, Mass availableMass, IEnumerable<Chemical> availableChems, IEnumerable<Layer> curLayers);

		/// <summary>
		/// Called when [added to stack].
		/// </summary>
		protected internal abstract void OnAddedToStack();

		/// <summary>
		/// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
		/// <returns>
		///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object? obj)
		{
			return Equals(obj as Layer);
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>
		///   <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.
		/// </returns>
		public bool Equals(Layer? other)
		{
			if (other is null)
				return false;

			return EqualityComparer<StellarBody>.Default.Equals(StellarBody, other.StellarBody) &&
				   EqualityComparer<SatelliteBody>.Default.Equals(Parent, other.Parent) &&
				   Thickness.Equals(other.Thickness, Extensions.Epsilon, ComparisonType.Relative) &&
				   Mass.Equals(other.Mass, Extensions.Epsilon, ComparisonType.Relative) &&
				   MeanTemperature.Equals(other.MeanTemperature, Extensions.Epsilon, ComparisonType.Relative) &&
				   EqualityComparer<IList<(Chemical, Ratio)>>.Default.Equals(CompositionInternal, other.CompositionInternal) &&
				   EqualityComparer<IEnumerable<(Chemical, Ratio)>>.Default.Equals(Composition, other.Composition);
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode()
		{
			return HashCode.Combine(StellarBody, Parent, Thickness, Mass, MeanTemperature, CompositionInternal, Composition);
		}

		/// <summary>
		/// Implements the operator ==.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		public static bool operator ==(Layer left, Layer right)
		{
			return EqualityComparer<Layer>.Default.Equals(left, right);
		}

		/// <summary>
		/// Implements the operator !=.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		public static bool operator !=(Layer left, Layer right)
		{
			return !(left == right);
		}
	}
}
