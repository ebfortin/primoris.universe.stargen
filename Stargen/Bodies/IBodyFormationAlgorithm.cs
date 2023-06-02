using Primoris.Numerics;
using Primoris.Universe.Stargen.Astrophysics.Burrows;
using Primoris.Universe.Stargen.Bodies.Burrows;

using System.Collections.Generic;
using UnitsNet;

namespace Primoris.Universe.Stargen.Bodies;

/// <summary>
/// Implemented by a class that can create Seeds for Body creation.
/// </summary>
public interface IBodyFormationAlgorithm
{
	/// <summary>
	/// Gets the dust density coefficient.
	/// </summary>
	/// <value>
	/// The dust density coefficient.
	/// </value>
	Ratio DustDensityCoefficient { get; }

	/// <summary>
	/// Gets the cloud eccentricity.
	/// </summary>
	/// <value>
	/// The cloud eccentricity.
	/// </value>
	Ratio CloudEccentricity { get; }

	/// <summary>
	/// Gets the gas dust ratio.
	/// </summary>
	/// <value>
	/// The gas dust ratio.
	/// </value>
	Ratio GasDustRatio { get; }

	/// <summary>
	/// Creates the seeds.
	/// </summary>
	/// <param name="stellarMassRatio">The stellar mass ratio.</param>
	/// <param name="stellarLumRatio">The stellar lum ratio.</param>
	/// <param name="innerDust">The inner dust.</param>
	/// <param name="outerDust">The outer dust.</param>
	/// <param name="outerPlanetLimit">The outer planet limit.</param>
	/// <param name="semiMajorAxisAU">The semi major axis au.</param>
	/// <returns>IEnumerable of created Seeds.</returns>
	IEnumerable<Seed> CreateSeeds(Mass stellarMassRatio,
							Luminosity stellarLumRatio,
							Length innerDust,
							Length outerDust,
							Length outerPlanetLimit,
							Length semiMajorAxisAU);
}