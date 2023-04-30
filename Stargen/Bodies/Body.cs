using System;
using System.Collections.Generic;
using System.Text;
using Primoris.Universe.Stargen.Astrophysics;
using Primoris.Universe.Stargen.Astrophysics.Singularity;
using Primoris.Universe.Stargen.Services;
using UnitsNet;


namespace Primoris.Universe.Stargen.Bodies;

/// <summary>
/// Abstract base class for a Body.
/// </summary>
public abstract class Body
{
	public static readonly Body Null = new NullBody();


    public virtual IScienceAstrophysics Science { get; }

	/// <summary>
	/// Gets or sets the parent.
	/// </summary>
	/// <remarks>
	/// For a planet this would be a StellarBody. For a Satellite this would be a planet.
	/// </remarks>
	/// <value>
	/// The parent.
	/// </value>
	public Body Parent { get; protected set; } = Null;

	/// <summary>
	/// Gets or sets the position.
	/// </summary>
	/// <value>
	/// The position.
	/// </value>
	public virtual int Position { get; set; }

	/// <summary>
	/// Gets or sets the name.
	/// </summary>
	/// <value>
	/// The name.
	/// </value>
	public virtual string Name { get; set; } = String.Empty;

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

	/// <summary>
	/// Velocity at which an object can escape the Body gravitational pull.
	/// </summary>
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


	public Body(IScienceAstrophysics science) 
	{
		Science = science;
	}


	class NullBody : Body
	{
		public NullBody() : base(new SingularityPhysics()) { }

		public override IScienceAstrophysics Science => base.Science;

		public override int Position
		{
			get => 0;
			set { }
		}

		public override string Name
		{
			get => String.Empty;
			set { }
		}
	}
}
