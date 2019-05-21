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

		public Star() : this(Utilities.RandomNumber(0.7, 1.4)) { }

		public Star(double mass, double lum = 0.0, double age = 4600000000.0)
		{
			if (mass < 0.2 || mass > 1.5)
			{
				Mass = Utilities.RandomNumber(0.7, 1.4);
			}

			if (lum == 0)
			{
				Luminosity = Main.Environment.MassToLuminosity(Mass);
			}

			SpectralType = SpectralType.FromLuminosity(Luminosity);

			//EcosphereRadiusAU = Math.Sqrt(lum);
			Life = 1.0E10 * (Mass / Luminosity);

			AgeYears = Utilities.RandomNumber(
				MinSunAge,
				Life < MaxSunAge ? Life : MaxSunAge);
		}

		public Star(SpectralType st)
		{
			SpectralType = st;
			// TODO: Complete constructor.
		}

		public SpectralType SpectralType { get; }

        public string Name { get; set; }

        /// <summary>
        /// Age of the star in years.
        /// </summary>
        public double AgeYears { get; }

        /// <summary>
        /// The maximum lifetime of the star in years.
        /// </summary>
        public double Life { get; }

        /// <summary>
        /// The distance that the star's "ecosphere" (as far as I can tell,
        /// ye olden science speak for circumstellar habitable zone) is
        /// centered on. Given in AU. 
        /// </summary>
        public double EcosphereRadiusAU { get => Math.Sqrt(Luminosity); }

        /// <summary>
        /// Luminosity of the star in solar luminosity units (L<sub>☉</sub>).
        /// The luminosity of the sun is 1.0.
        /// </summary>
        public double Luminosity { get; }

        /// <summary>
        /// Mass of the star in solar mass units (M<sub>☉</sub>). The mass of
        /// the sun is 1.0.
        /// </summary>
        public double Mass { get; }

        /// <summary>
        /// The mass of this star's companion star (if any) in solar mass
        /// units (M<sub>☉</sub>). 
        /// </summary>
        public double BinaryMass { get; }

        /// <summary>
        /// The semi-major axis of the companion star in au.
        /// </summary>
        public double SemiMajorAxisAU { get; }

        /// <summary>
        /// The eccentricity of the companion star's orbit.
        /// </summary>
        public double Eccentricity { get; }
    }
}
