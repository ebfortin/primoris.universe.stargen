﻿using System;
using System.Collections.Generic;
using System.Text;
using Primoris.Universe.Stargen.Astrophysics;
using UnitsNet;


namespace Primoris.Universe.Stargen.Bodies
{
	public abstract class Body
	{
        public virtual IScienceAstrophysics Astro { get; protected set; }

        public virtual int Position { get; protected set; }

        public string Name { get; protected set; }

        public Body Parent { get; protected set; }

        public StellarBody StellarBody { get; protected set; }

        public Duration Age { get; protected set; }

		public Mass Mass { get; protected set; }

        public Length Radius { get; protected set; }

        public Speed EscapeVelocity { get; protected set; }

        public Temperature Temperature { get; protected set; }



        public IEnumerable<SatelliteBody> Satellites { get; protected set; }
	}
}
