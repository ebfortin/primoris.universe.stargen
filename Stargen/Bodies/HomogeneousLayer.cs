using System;
using System.Collections.Generic;
using System.Text;
using Primoris.Universe.Stargen.Astrophysics;
using UnitsNet;

namespace Primoris.Universe.Stargen.Bodies
{
	/// <summary>
	/// Layer that is Homogeneous.
	/// </summary>
	/// <remarks>
	/// In this version of Stargen, all layers are Homogeneous.
	/// </remarks>
	/// <seealso cref="Primoris.Universe.Stargen.Bodies.Layer" />
	public abstract class HomogeneousLayer : Layer
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="HomogeneousLayer"/> class.
		/// </summary>
		/// <param name="thickness">The thickness.</param>
		public HomogeneousLayer(SatelliteBody parent, Length thickness) 
			: base(parent, thickness)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HomogeneousLayer"/> class.
		/// </summary>
		/// <param name="thickness">The thickness.</param>
		/// <param name="composition">The composition.</param>
		public HomogeneousLayer(SatelliteBody parent, Length thickness, IEnumerable<(Chemical, Ratio)> composition) 
			: base(parent, thickness, composition)
		{
		}
	}
}
