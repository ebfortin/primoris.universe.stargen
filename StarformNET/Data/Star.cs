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

		public Star() : this(Utilities.RandomNumber(0.7, 1.4))
		{

		}

		public Star(double mass, double lum = 0.0, double age = double.NaN)
		{
			if (mass < 0.2 || mass > 1.5)
			{
				mass = Utilities.RandomNumber(0.7, 1.4);
			}

			if (lum == 0)
			{
				lum = Main.Environment.MassToLuminosity(mass);
			}

			StellarType = StellarType.FromLuminosityAndRadius(lum, 1.0);

			//EcosphereRadiusAU = Math.Sqrt(lum);
			Life = 1.0E10 * (Mass / Luminosity);

			if (double.IsNaN(age))
				AgeYears = Utilities.RandomNumber(MinSunAge, Life < MaxSunAge ? Life : MaxSunAge);
			else
				AgeYears = age;
		}

		public Star(StellarType st)
		{
			StellarType = st;
			Life = 1.0E10 * (st.Mass / st.Luminosity);
			AgeYears = Utilities.RandomNumber(MinSunAge, Life < MaxSunAge ? Life : MaxSunAge);
		}

		public StellarType StellarType { get; }

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
        public double Luminosity { get => StellarType.Luminosity; }

        /// <summary>
        /// Mass of the star in solar mass units (M<sub>☉</sub>). The mass of
        /// the sun is 1.0.
        /// </summary>
        public double Mass { get => StellarType.Mass; }

		public double Radius { get => StellarType.Radius; }

		public double Temperature { get => StellarType.Temperature; }

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
