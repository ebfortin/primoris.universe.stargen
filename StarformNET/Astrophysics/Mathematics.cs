using System;
using System.Collections.Generic;
using System.Text;
using UnitsNet;


namespace Primoris.Universe.Stargen.Astrophysics
{
	/// <summary>
	/// Common maths functions.
	/// </summary>
	public static class Mathematics
	{
		/// <summary>
		/// Calculates the radius of a planet from a Mass and a Density.
		/// </summary>
		/// <param name="mass">Mass in units of solar masses</param>
		/// <param name="density">Density in units of grams/cc</param>
		/// <returns>Radius in units of km</returns>
		public static Length GetRadiusFromVolume(Mass m, Density density)
		{
			double volume;

			double mass = m.Grams;
			volume = mass / density.GramsPerCubicCentimeter;
			return Length.FromKilometers(Math.Pow((3.0 * volume) / (4.0 * Math.PI), (1.0 / 3.0)) / GlobalConstants.CM_PER_KM);
		}

		/// <summary>
		/// Density given a Volume and a Mass.
		/// </summary>
		/// <param name="mass">Mass in units of solar masses</param>
		/// <param name="equatRadius">Equatorial radius in km</param>
		/// <returns>Units of grams/cc</returns>
		public static Density GetDensityFromVolume(Mass m, Length r)
		{
			double mass = m.Grams;
			double equatRadius = r.Centimeters;
			double volume = (4.0 * Math.PI * Extensions.Pow3(equatRadius)) / 3.0;
			return Density.FromGramsPerCubicCentimeter(mass / volume);
		}


	}
}
