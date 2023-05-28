using System;
using System.Collections.Generic;
using UnitsNet;



namespace Primoris.Universe.Stargen.Bodies;

public record Seed
{
	/// <summary>
	/// Gets or sets the satellites Seeds.
	/// </summary>
	/// <value>
	/// The satellites Seeds.
	/// </value>
	public IEnumerable<Seed> Satellites { get; set; } = Array.Empty<Seed>();

	/// <summary>
	/// Gets or sets the semi major axis.
	/// </summary>
	/// <value>
	/// The semi major axis. Default to Zero.
	/// </value>
	public Length SemiMajorAxis { get; set; } = Length.Zero;

	/// <summary>
	/// Gets or sets the eccentricity.
	/// </summary>
	/// <value>
	/// The eccentricity. Default to Zero (perfectly circular orbit).
	/// </value>
	public Ratio Eccentricity { get; set; } = Ratio.Zero;

	/// <summary>
	/// Gets or sets the total mass of the seed.
	/// </summary>
	/// <remarks>
	/// This in most circumstances is equal to DustMass + GasMass. However a derived class could have this value higher to account for different
	/// state of matter present, like plasma.
	/// </remarks>
	/// <value>
	/// The total mass of the Seed.
	/// </value>
	public Mass Mass => DustMass + GasMass;

	/// <summary>
	/// Gets or sets the dust mass.
	/// </summary>
	/// <value>
	/// The dust mass.
	/// </value>
	public Mass DustMass { get; set; } = Mass.Zero;

	/// <summary>
	/// Gets or sets the gas mass.
	/// </summary>
	/// <value>
	/// The gas mass.
	/// </value>
	public Mass GasMass { get; set; } = Mass.Zero;

	/// <summary>
	/// Gets or sets a value indicating whether this instance of a Seed leads to the formation of a gas giant.
	/// </summary>
	/// <value>
	///   <c>true</c> if this instance is gas giant; otherwise, <c>false</c>.
	/// </value>
	public bool IsGasGiant { get; set; } = false;

	internal Seed() : this(default, default, default, default)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Seed"/> class.
	/// </summary>
	/// <param name="a">Semi major axis.</param>
	/// <param name="e">Orbital eccentricity..</param>
	/// <param name="mass">The total mass of the Seed.</param>
	/// <param name="dMass">The dust mass present in the Seed.</param>
	/// <param name="gMass">The gas mass present in the Seed.</param>
	public Seed(Length a, Ratio e, Mass dMass, Mass gMass)
	{
		SemiMajorAxis = a;
		Eccentricity = e;
		DustMass = dMass;
		GasMass = gMass;
	}

}

