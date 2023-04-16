using System;
using System.Collections.Generic;
using System.Text;
using Primoris.Universe.Stargen.Astrophysics;
using Primoris.Universe.Stargen.Services;
using UnitsNet;


namespace Primoris.Universe.Stargen.Bodies;

/// <summary>
/// Abstract base class for a Body.
/// </summary>
public abstract class Body
{
    private IScienceAstrophysics? _phy = null;
    public IScienceAstrophysics? Science { get => _phy is null ? Provider.Use().GetService<IScienceAstrophysics>() : _phy; set => _phy = value; }

	/// <summary>
	/// Gets or sets the position.
	/// </summary>
	/// <value>
	/// The position.
	/// </value>
	public int Position { get; set; }

	/// <summary>
	/// Gets or sets the name.
	/// </summary>
	/// <value>
	/// The name.
	/// </value>
	public string Name { get; set; } = String.Empty;

	/// <summary>
	/// Gets or sets the age.
	/// </summary>
	/// <value>
	/// The age.
	/// </value>
	public Duration Age { get; protected set; }

	/// <summary>
	/// Gets or sets the mass.
	/// </summary>
	/// <value>
	/// The mass.
	/// </value>
	public virtual Mass Mass { get; protected set; }

	/// <summary>
	/// Radius of the Body, including all Layers. 
	/// </summary>
        public virtual Length Radius { get; protected set; }

        public Speed EscapeVelocity { get; protected set; }

	/// <summary>
	/// Gets or sets the temperature.
	/// </summary>
	/// <value>
	/// The temperature.
	/// </value>
	public Temperature Temperature { get; protected set; }

	/// <summary>
	/// Gets or sets the satellites.
	/// </summary>
	/// <value>
	/// The satellites.
	/// </value>
	public IEnumerable<SatelliteBody> Satellites { get; protected set; } = Array.Empty<SatelliteBody>();
}
