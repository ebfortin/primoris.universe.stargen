using System;
using System.Collections.Generic;
using Main = Primoris.Universe.Stargen;
using Primoris.Universe.Stargen.Bodies;
using System.Drawing;
using UnitsNet;

namespace Primoris.Universe.Stargen.Astrophysics
{

	// UGLY Not comfortable with binary systems just having a second mass value

	[Serializable]
	public class Star : Body
	{
		public static readonly Duration MinSunAge = Duration.FromYears365(1.0E9);
		public static readonly Duration MaxSunAge = Duration.FromYears365(6.0E9);

		public Star() : this(Mass.FromSolarMasses(Utilities.RandomNumber(0.7, 1.4))) { }

		public Star(Mass mass) : this(mass, Luminosity.FromSolarLuminosities(0.0), Duration.FromYears365(double.NaN)) { }

		public Star(Mass mass, Luminosity lum, Duration age)
		{
			if (mass.SolarMasses < 0.2 || mass.SolarMasses > 1.5)
			{
				mass = Mass.FromSolarMasses(Utilities.RandomNumber(0.7, 1.4));
			}

			if (lum.SolarLuminosities == 0.0)
			{
				lum = Luminosity.FromSolarLuminosities(Environment.MassToLuminosity(mass));
			}

			StellarType = StellarType.FromLuminosityAndRadius(lum, Length.FromSolarRadiuses(1.0));

			//EcosphereRadiusAU = Math.Sqrt(lum);
			Life = Duration.FromYears365(1.0E10 * (Mass.SolarMasses / Luminosity.SolarLuminosities));

			if (double.IsNaN(age.Years365))
				Age = Duration.FromYears365(Utilities.RandomNumber(MinSunAge.Years365, Life < MaxSunAge ? Life.Years365 : MaxSunAge.Years365));
			else
				Age = age;
		}

		public Star(StellarType st)
		{
			StellarType = st;
			Life = Duration.FromYears365(1.0E10 * (st.Mass.SolarMasses / st.Luminosity.SolarLuminosities));
			Age = Duration.FromYears365(Utilities.RandomNumber(MinSunAge.Years365, Life < MaxSunAge ? Life.Years365 : MaxSunAge.Years365));
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
				return Math.Sqrt(Math.Pow(Luminosity.SolarLuminosities - st.Luminosity.SolarLuminosities, 2.0) +
								 Math.Pow(Radius.SolarRadiuses - st.Radius.SolarRadiuses, 2.0) +
								 Math.Pow(Mass.SolarMasses - st.Mass.SolarMasses, 2.0) +
								 Math.Pow(Temperature.Kelvins / GlobalConstants.EARTH_SUN_TEMPERATURE - st.Temperature.Kelvins / GlobalConstants.EARTH_SUN_TEMPERATURE, 2.0));

			}
		}

		/// <summary>
		/// Age of the star in years.
		/// </summary>
		public Duration Age { get; }

		/// <summary>
		/// The maximum lifetime of the star in years.
		/// </summary>
		public Duration Life { get; }

		public Length EcosphereRadius { get => Length.FromKilometers(Math.Sqrt(Luminosity.SolarLuminosities) * GlobalConstants.ASTRONOMICAL_UNIT_KM); }

		/// <summary>
		/// Luminosity of the star in solar luminosity units (L<sub>☉</sub>).
		/// The luminosity of the sun is 1.0.
		/// </summary>
		public Luminosity Luminosity { get => StellarType.Luminosity; }

		/// <summary>
		/// Mass of the star in solar mass units (M<sub>☉</sub>). The mass of
		/// the sun is 1.0.
		/// </summary>
		public override Mass Mass { get => StellarType.Mass; protected set => throw new NotImplementedException(); }

		public Length Radius { get => StellarType.Radius; }

		public Temperature Temperature { get => StellarType.Temperature; }

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
