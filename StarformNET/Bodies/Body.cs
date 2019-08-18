using System;
using System.Collections.Generic;
using System.Text;
using Primoris.Universe.Stargen.Astrophysics;
using Primoris.Universe.Stargen.Services;
using UnitsNet;


namespace Primoris.Universe.Stargen.Bodies
{
	public abstract class Body
	{
        private IScienceAstrophysics _phy = null;
        public IScienceAstrophysics Science { get => _phy is null ? Provider.Use().GetService<IScienceAstrophysics>() : _phy; set => _phy = value; }

        public int Position { get; set; }

        public string Name { get; set; }

        public Body Parent { get; protected set; }

        public StellarBody StellarBody { get; protected set; }

        public Duration Age { get; protected set; }

		public virtual Mass Mass { get; protected set; }

		/// <summary>
		/// Radius of the Body, including all Layers. 
		/// </summary>
        public virtual Length Radius { get; protected set; }

        public Speed EscapeVelocity { get; protected set; }

        public Temperature Temperature { get; protected set; }



		public IEnumerable<SatelliteBody> Satellites { get; protected set; } = new SatelliteBody[0];
	}
}
