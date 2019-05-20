using Main = Primoris.Universe.Stargen;

namespace Primoris.Universe.Stargen.Data
{
    using System;

    // UGLY Not comfortable with binary systems just having a second mass value

    [Serializable]
    public class Star
    {
		public const double MinSunAge = 1.0E9;
		public const double MaxSunAge = 6.0E9;

		public Star()
		{
			var sun = this;

			if (sun.Mass < 0.2 || sun.Mass > 1.5)
			{
				sun.Mass = Utilities.RandomNumber(0.7, 1.4);
			}

			if (sun.Luminosity == 0)
			{
				sun.Luminosity = Main.Environment.MassToLuminosity(sun.Mass);
			}

			sun.EcosphereRadiusAU = Main.Environment.StarEcosphereRadiusAU(sun.Luminosity);
			sun.Life = 1.0E10 * (sun.Mass / sun.Luminosity);

			sun.AgeYears = Utilities.RandomNumber(
				MinSunAge,
				sun.Life < MaxSunAge ? sun.Life : MaxSunAge);
		}

        public string Name { get; set; }

        /// <summary>
        /// Age of the star in years.
        /// </summary>
        public double AgeYears { get; set; }

        /// <summary>
        /// The maximum lifetime of the star in years.
        /// </summary>
        public double Life { get; set; }

        /// <summary>
        /// The distance that the star's "ecosphere" (as far as I can tell,
        /// ye olden science speak for circumstellar habitable zone) is
        /// centered on. Given in AU. 
        /// </summary>
        public double EcosphereRadiusAU { get; set; }

        /// <summary>
        /// Luminosity of the star in solar luminosity units (L<sub>☉</sub>).
        /// The luminosity of the sun is 1.0.
        /// </summary>
        public double Luminosity { get; set; }

        /// <summary>
        /// Mass of the star in solar mass units (M<sub>☉</sub>). The mass of
        /// the sun is 1.0.
        /// </summary>
        public double Mass { get; set; }

        /// <summary>
        /// The mass of this star's companion star (if any) in solar mass
        /// units (M<sub>☉</sub>). 
        /// </summary>
        public double BinaryMass { get; set; }

        /// <summary>
        /// The semi-major axis of the companion star in au.
        /// </summary>
        public double SemiMajorAxisAU { get; set; }

        /// <summary>
        /// The eccentricity of the companion star's orbit.
        /// </summary>
        public double Eccentricity { get; set; }
    }
}
