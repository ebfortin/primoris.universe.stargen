using System;
using Main = Primoris.Universe.Stargen;
using System.Drawing;
using Primoris.Universe.Stargen.Physics;

namespace Primoris.Universe.Stargen.Data
{

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
				Age = Utilities.RandomNumber(MinSunAge, Life < MaxSunAge ? Life : MaxSunAge);
			else
				Age = age;
		}

		public Star(StellarType st)
		{
			StellarType = st;
			Life = 1.0E10 * (st.Mass / st.Luminosity);
			Age = Utilities.RandomNumber(MinSunAge, Life < MaxSunAge ? Life : MaxSunAge);
		}

        public Star(StellarType st, string name) : this(st)
        {
            Name = name;
        }

        public StellarType StellarType { get; }
		public Color Color { get => StellarType.Color; }
		public double DistanceFromTypical
		{
			get
			{
				var st = StellarType.FromString(StellarType.ToString());
				return Math.Sqrt(Math.Pow(Luminosity - st.Luminosity, 2.0) +
							     Math.Pow(Radius - st.Radius, 2.0) +
								 Math.Pow(Mass - st.Mass, 2.0) +
								 Math.Pow(Temperature / GlobalConstants.EARTH_SUN_TEMPERATURE - st.Temperature / GlobalConstants.EARTH_SUN_TEMPERATURE, 2.0));
				
			}
		}

        public string Name { get; set; }

        /// <summary>
        /// Age of the star in years.
        /// </summary>
        public double Age { get; }

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

		public double EcosphereRadius { get => EcosphereRadiusAU * GlobalConstants.ASTRONOMICAL_UNIT_KM; }

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

        public override string ToString()
        {
            return Name + " (" + StellarType + ")";
        }
    }
}
