using System;
using System.Collections.Generic;
using System.Text;
using Primoris.Universe.Stargen.Astrophysics;
using Primoris.Universe.Stargen.Systems;
using Environment = Primoris.Universe.Stargen.Astrophysics.Environment;


namespace Primoris.Universe.Stargen.Bodies.Burrows
{
	public class Moon : Planet
	{
		public Moon(Seed seed,
					   Star star,
					   SatelliteBody parentBody,
					   int num,
					   bool useRandomTilt,
					   string planetID,
					   SystemGenerationOptions genOptions) : base(seed,
												   star,
												   num,
												   useRandomTilt,
												   planetID,
												   genOptions)
		{
			var generatedMoon = this;

			double roche_limit = 2.44 * parentBody.Radius * Math.Pow(parentBody.DensityGCC / generatedMoon.DensityGCC, 1.0 / 3.0);
			double hill_sphere = parentBody.SemiMajorAxisAU * GlobalConstants.KM_PER_AU * Math.Pow(parentBody.MassSM / (3.0 * star.MassSM), 1.0 / 3.0);

			if (roche_limit * 3.0 < hill_sphere)
			{
				generatedMoon.MoonSemiMajorAxisAU = Utilities.RandomNumber(roche_limit * 1.5, hill_sphere / 2.0) / GlobalConstants.KM_PER_AU;
				generatedMoon.MoonEccentricity = Utilities.RandomEccentricity();
			}
			else
			{
				generatedMoon.MoonSemiMajorAxisAU = 0;
				generatedMoon.MoonEccentricity = 0;
			}
		}

	}
}
