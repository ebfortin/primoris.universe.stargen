using System;
using System.IO;
using System.Drawing;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CsvHelper;
using UnitsNet;

namespace Primoris.Universe.Stargen.Astrophysics
{
	public sealed class StellarType
	{

		private class StellarTypeRow
		{
			public string Type { get; set; }
			public double Mass { get; set; }
			public double Luminosity { get; set; }
			public double Radius { get; set; }
			public double Temperature { get; set; }
			public double ColorIndex { get; set; }
			public double AbsMag { get; set; }
			public double BoloCorr { get; set; }
			public double BoloMag { get; set; }
			public string ColorRGB { get; set; }
		}

		#region Static Constructor
		static StellarType()
		{
			// Full StellarType table from http://www.isthe.com/chongo/tech/astro/HR-temp-mass-table-byhrclass.html

			var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Primoris.Universe.Stargen.Resources.stellartypes.csv");
			var reader = new StreamReader(stream);
			var csv = new CsvReader(reader);
			_types = csv.GetRecords<StellarTypeRow>().ToList();
		}
		#endregion

		/// <summary>
		/// Gets the spectral class.
		/// </summary>
		/// <value>
		/// The spectral class.
		/// </value>
		public SpectralClass SpectralClass { get; private set; }

		/// <summary>
		/// Gets the subtype of the StellarBody.
		/// </summary>
		/// <value>
		/// The subtype.
		/// </value>
		public int SubType { get; private set; }

		/// <summary>
		/// Gets the luminosity class.
		/// </summary>
		/// <value>
		/// The luminosity class.
		/// </value>
		public LuminosityClass LuminosityClass { get; private set; }

		/// <summary>
		/// Gets the temperature.
		/// </summary>
		/// <value>
		/// The temperature.
		/// </value>
		public Temperature Temperature { get; private set; }

		/// <summary>
		/// Gets the mass.
		/// </summary>
		/// <value>
		/// The mass.
		/// </value>
		public Mass Mass { get; private set; }

		/// <summary>
		/// Gets the luminosity.
		/// </summary>
		/// <value>
		/// The luminosity.
		/// </value>
		public Luminosity Luminosity { get; private set; }

		/// <summary>
		/// Gets the radius.
		/// </summary>
		/// <value>
		/// The radius.
		/// </value>
		public Length Radius { get; private set; }

		/// <summary>
		/// Gets the color.
		/// </summary>
		/// <value>
		/// The color.
		/// </value>
		public Color Color { get; private set; }


		private static List<StellarTypeRow> _types;

		/// <summary>
		/// Initializes a new instance of the <see cref="StellarType"/> class.
		/// </summary>
		/// <param name="sc">The SpectralClass of the StellarBody.</param>
		/// <param name="lc">The LuminosityClass of the StellarBody.</param>
		/// <param name="subType">Type subtype of the StellarBody.</param>
		public StellarType(SpectralClass sc, LuminosityClass lc, int subType = 0)
		{
			SpectralClass = sc;
			LuminosityClass = lc;
			SubType = subType;

			if (sc == SpectralClass.Undefined || lc == LuminosityClass.Undefined)
				return;

			var str = Enum.GetName(typeof(SpectralClass), sc) + SubType.ToString() + (lc != LuminosityClass.O ? Enum.GetName(typeof(LuminosityClass), lc) : "");
			var data = (from row in _types
						where row.Type == str
						select new { row.Temperature, row.Mass, row.Radius, row.Luminosity, row.ColorRGB }).FirstOrDefault();
			Temperature = Temperature.FromKelvins(data.Temperature);
			Mass = Mass.FromSolarMasses(data.Mass);
			Luminosity = Luminosity.FromSolarLuminosities(data.Luminosity);
			Radius = Length.FromKilometers(data.Radius * GlobalConstants.KM_SUN_RADIUS);
			Color = ConvertColor(data.ColorRGB);
		}

		private StellarType() { }

		/// <summary>
		/// Changes the luminosity.
		/// </summary>
		/// <param name="lum">The new luminosity.</param>
		public void ChangeLuminosity(Luminosity lum)
		{
			Change(Mass, lum, Temperature, Radius);
		}


		/// <summary>
		/// Changes the mass.
		/// </summary>
		/// <param name="mass">The new mass.</param>
		public void ChangeMass(Mass mass)
		{
			Change(mass, Luminosity, Temperature, Radius);
		}


		/// <summary>
		/// Changes the temperature.
		/// </summary>
		/// <param name="temp">The temporary.</param>
		public void ChangeTemperature(Temperature temp)
		{
			Change(Mass, Luminosity, temp, Radius);
		}

		public void ChangeRadius(Length radius)
		{
			Change(Mass, Luminosity, Temperature, radius);
		}

		/// <summary>
		/// Change all the parameters of the StellarClass.
		/// TODO: Add more check to prevent impossible values like Temperature = 1K. Could be by calculating typical distance per components instead of global.
		/// </summary>
		/// <remarks>
		/// This method search for the nearest existing StellarType and then adjust the parameters with the values specified. Right now impossible values are
		/// allowed, like a star with a 1K Temperature.
		/// 
		/// TODO: Add more check to prevent impossible values like Temperature = 1K. Could be by calculating typical distance per components instead of global.
		/// </remarks>
		/// <param name="mass">The new Mass.</param>
		/// <param name="lum">The new luminosity.</param>
		/// <param name="temp">The new Temperature.</param>
		/// <param name="radius">The new Radius.</param>
		public void Change(Mass mass, Luminosity lum, Temperature temp, Length radius)
		{
			var data = (from row in _types
						orderby Math.Sqrt((!(lum.Value == 0.0) ? Math.Pow(lum.SolarLuminosities - row.Luminosity, 2.0) : 0.0) +
										  (!(radius.Value == 0.0) ? Math.Pow(radius.SolarRadiuses - row.Radius, 2.0) : 0.0) +
										  (!(mass.Value == 0.0) ? Math.Pow(mass.SolarMasses - row.Mass, 2.0) : 0.0) +
										  (!(temp.Value == 0.0) ? Math.Pow(temp.Kelvins / GlobalConstants.EARTH_SUN_TEMPERATURE - row.Temperature / GlobalConstants.EARTH_SUN_TEMPERATURE, 2.0) : 0.0))
						ascending
						select row).FirstOrDefault();

			StellarType st = FromString(data.Type);
			SpectralClass = st.SpectralClass;
			LuminosityClass = st.LuminosityClass;
			SubType = st.SubType;

			double m = st.Mass.SolarMasses;
			double l = st.Luminosity.SolarLuminosities;
			double t = st.Temperature.Kelvins;
			double r = st.Radius.SolarRadiuses;

			Mass = !(mass.Value == 0.0) ? mass : Mass.FromSolarMasses(m);
			Luminosity = !(lum.Value == 0.0) ? lum : Luminosity.FromSolarLuminosities(l);
			Temperature = !(temp.Value == 0.0) ? temp : Temperature.FromKelvins(t);
			Radius = !(radius.Value == 0.0) ? radius : Length.FromSolarRadiuses(r);

			Color = ConvertColor(data.ColorRGB);
		}

		/// <summary>
		/// Give a SpectralType given a star luminosity ratio to Earth's sun.
		/// </summary>
		/// <remarks>
		/// Mostly valid for main sequence stars.
		/// </remarks>
		/// <param name="lum">Luminosity ratio to Earth's sun.</param>
		/// <param name="radius">Radius ratio to Earth's sun.</param>
		/// <returns>Nearest StellarType with modified parameters.</returns>
		public static StellarType FromLuminosityAndRadius(Luminosity lum, Length radius)
		{
			var data = (from row in _types
						orderby Math.Sqrt(Math.Pow(lum.SolarLuminosities - row.Luminosity, 2.0) +
										  Math.Pow(radius.SolarRadiuses - row.Radius, 2.0))
						ascending
						select row).FirstOrDefault();

			StellarType st = FromString(data.Type);
			st.Luminosity = lum;
			st.Radius = radius;
			st.Color = ConvertColor(data.ColorRGB);

			return st;
		}

		/// <summary>
		/// Returns a StellarType given a mass and temperature.
		/// </summary>
		/// <param name="mass">The mass.</param>
		/// <param name="temp">The temperature.</param>
		/// <returns>Nearest StellarType with modified parameters.</returns>
		public static StellarType FromMassAndTemperature(Mass mass, Temperature temp)
		{
			var data = (from row in _types
						orderby Math.Sqrt(Math.Pow(mass.SolarMasses - row.Mass, 2.0) +
										  Math.Pow(temp.Kelvins / GlobalConstants.EARTH_SUN_TEMPERATURE - row.Temperature / GlobalConstants.EARTH_SUN_TEMPERATURE, 2.0))
						ascending
						select row).FirstOrDefault();

			StellarType st = FromString(data.Type);
			st.Mass = mass;
			st.Temperature = temp;
			st.Color = ConvertColor(data.ColorRGB);

			return st;
		}

		/// <summary>
		/// Returns a StellarType given a mass.
		/// </summary>
		/// <remarks>
		/// Assume a Mass of one solar mass.
		/// </remarks>
		/// <param name="mass">The mass.</param>
		/// <returns>Nearest StellarType with modified parameters.</returns>
		public static StellarType FromMass(Mass mass)
		{
			return FromMassAndRadius(mass, Length.FromSolarRadiuses(1.0));
		}

		/// <summary>
		/// Give the StellarType given a star mass ratio to Earth's sun.
		/// </summary>
		/// <remarks>
		/// Reference: http://homepage.physics.uiowa.edu/~pkaaret/s09/L12_starsmainseq.pdf
		/// La/Lb = (Ra^2*Ta^4)/(Rb^2*Tb^4)
		/// </remarks>
		/// <param name="mass">Mass ratio to earth's sun.</param>
		/// <param name="radius">Radius ratio to earth's sun.</param>
		/// <returns>Nearest StellarType with modified parameters.</returns>
		public static StellarType FromMassAndRadius(Mass mass, Length radius)
		{
			var data = (from row in _types
						orderby Math.Sqrt(Math.Pow(mass.SolarMasses - row.Mass, 2.0) +
										  Math.Pow(radius.SolarRadiuses - row.Radius, 2.0))
						ascending
						select row).FirstOrDefault();

			StellarType st = FromString(data.Type);
			st.Mass = mass;
			st.Radius = radius;
			st.Color = ConvertColor(data.ColorRGB);

			return st;
		}

		/// <summary>
		/// Give the StellarType given a Temperature.
		/// </summary>
		/// <remarks>
		/// Assume a Luminosity of 0.0 Solar luminosity.
		/// </remarks>
		/// <param name="eff_temp">The eff temperature.</param>
		/// <returns>Nearest StellarType with modified parameters.</returns>
		public static StellarType FromTemperature(Temperature eff_temp)
		{
			return FromTemperatureAndLuminosity(eff_temp, Luminosity.FromSolarLuminosities(0.0));
		}


		/// <summary>
		/// Give the StellarType given the temperature and luminosity.
		/// </summary>
		/// <param name="eff_temp">The eff temperature.</param>
		/// <param name="luminosity">The luminosity.</param>
		/// <returns>Nearest StellarType with modified parameters.</returns>
		public static StellarType FromTemperatureAndLuminosity(Temperature eff_temp, Luminosity luminosity)
		{
			var data = (from row in _types
						orderby Math.Sqrt(Math.Pow(eff_temp.Kelvins / GlobalConstants.EARTH_SUN_TEMPERATURE - row.Temperature / GlobalConstants.EARTH_SUN_TEMPERATURE, 2.0) +
										  Math.Pow(luminosity.SolarLuminosities - row.Luminosity, 2.0))
						ascending
						select row).FirstOrDefault();

			StellarType st = FromString(data.Type);
			st.Temperature = eff_temp;
			st.Luminosity = luminosity;
			st.Color = ConvertColor(data.ColorRGB);

			return st;
		}

		private static Color ConvertColor(string comps)
		{
			var gs = Regex.Match(comps, @"(\d{3})(\d{3})(\d{3})");

			return Color.FromArgb(int.Parse(gs.Groups[1].Value), int.Parse(gs.Groups[2].Value), int.Parse(gs.Groups[3].Value));
		}

		/// <summary>
		/// Give a StellarType given a standardized StellarType string.
		/// </summary>
		/// <param name="st">The StellarType string.</param>
		/// <returns>Nearest StellarType with modified parameters.</returns>
		/// <exception cref="ArgumentException">String given is of the wrong format.</exception>
		public static StellarType FromString(string st)
		{
			if (string.IsNullOrEmpty(st) || st == "Unk")
				return new StellarType(SpectralClass.Undefined, LuminosityClass.Undefined);

			try
			{
				var mt = Regex.Match(st, @"(\D*)(\d*)(\D*)?");
				SpectralClass sc = (SpectralClass)Enum.Parse(typeof(SpectralClass), mt.Groups[1].Value);
				LuminosityClass lc;
				if (!string.IsNullOrEmpty(mt.Groups[3].Value))
					lc = (LuminosityClass)Enum.Parse(typeof(LuminosityClass), mt.Groups[3].Value);
				else
					lc = LuminosityClass.O;
				int subType = int.Parse(mt.Groups[2].Value);

				return new StellarType(sc, lc, subType);
			}
			catch (Exception)
			{
				throw new ArgumentException();
			}
		}

		/// <summary>
		/// Converts to string.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String" /> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			if (SpectralClass == SpectralClass.Undefined || LuminosityClass == LuminosityClass.Undefined)
				return "Unk";

			return Enum.GetName(typeof(SpectralClass), SpectralClass) +
					SubType.ToString() +
					(LuminosityClass != LuminosityClass.O ? Enum.GetName(typeof(LuminosityClass), LuminosityClass) : "");
		}
	}
}
