namespace Primoris.Universe.Stargen.Bodies.Burrows;

public class Moon : Planet
{
	public Moon(Seed seed,
				   StellarBody star,
				   SatelliteBody parentBody,
				   string planetID) : base(seed, parentBody)
	{
		var generatedMoon = this;

		Length roche_limit = 2.44 * parentBody.Radius * Math.Pow(parentBody.Density / generatedMoon.Density, 1.0 / 3.0);
		Length hill_sphere = parentBody.SemiMajorAxis * Math.Pow(parentBody.Mass / (3.0 * star.Mass), 1.0 / 3.0);

		if (roche_limit * 3.0 < hill_sphere)
		{
			generatedMoon.SemiMajorAxis = Length.FromKilometers(Extensions.RandomNumber(roche_limit.Kilometers * 1.5, hill_sphere.Kilometers / 2.0));
			generatedMoon.Eccentricity = Ratio.FromDecimalFractions(Extensions.RandomEccentricity());
		}
		else
		{
			generatedMoon.SemiMajorAxis = Length.FromAstronomicalUnits(0.0);
			generatedMoon.Eccentricity = Ratio.FromDecimalFractions(0.0);
		}
	}

	protected override void Generate()
	{
		return;
	}
}
