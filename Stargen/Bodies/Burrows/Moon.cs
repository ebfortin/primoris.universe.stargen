﻿using Primoris.Universe.Stargen.Astrophysics;

namespace Primoris.Universe.Stargen.Bodies.Burrows;

public class Moon : Planet
{
	public Moon(IScienceAstrophysics science,
				Seed seed,
				StellarBody star,
				SatelliteBody parentBody,
				string planetID) : base(science, seed, parentBody)
	{
		var generatedMoon = this;

		Length roche_limit = 2.44 * parentBody.Radius * Math.Pow(parentBody.Density / generatedMoon.Density, 1.0 / 3.0);
		Length hill_sphere = parentBody.SemiMajorAxis * Math.Pow(parentBody.Mass / (3.0 * star.Mass), 1.0 / 3.0);

		if (roche_limit * 3.0 < hill_sphere)
		{
			Seed.SemiMajorAxis = Length.FromKilometers(Science.Random.NextFloat(roche_limit.Kilometers * 1.5, hill_sphere.Kilometers / 2.0));
			Seed.Eccentricity = Ratio.FromDecimalFractions(Science.Random.Eccentricity());
		}
		else
		{
			// TODO: Is this ever used? Does it means that the moon does not exist?
			Seed.SemiMajorAxis = Length.FromAstronomicalUnits(0.0);
			Seed.Eccentricity = Ratio.FromDecimalFractions(0.0);
		}
	}

	protected override void Generate()
	{
		return;
	}
}
