using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CsvHelper;


namespace Primoris.Universe.Stargen.Data
{
	public class StellarType
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

		public SpectralClass SpectralClass { get; private set; }
		public int SubType { get; private set; }
		public LuminosityClass LuminosityClass { get; private set; }
		public double Temperature { get; private set; }
		public double Mass { get; private set; }
		public double Luminosity { get; private set; }
		public double Radius { get; private set; }

		private static List<StellarTypeRow> _types;

		public StellarType(SpectralClass sc, LuminosityClass lc, int subType = 0)
		{
			SpectralClass = sc;
			LuminosityClass = lc;
			SubType = subType;

			var str = (Enum.GetName(typeof(SpectralClass), sc) + SubType.ToString() + (lc != LuminosityClass.O ? Enum.GetName(typeof(LuminosityClass), lc) : ""));
			var data = (from row in _types
						where row.Type == str
						select new { Temperature = row.Temperature, Mass = row.Mass, Radius = row.Radius, Luminosity = row.Luminosity }).FirstOrDefault();
			Temperature = data.Temperature;
			Mass = data.Mass;
			Luminosity = data.Luminosity;
			Radius = data.Radius;
		}

		private StellarType() { }

		public void ChangeLuminosity(double lum)
		{
			Change(Mass, lum, Temperature, Radius);
		}

		public void ChangeMass(double mass)
		{
			Change(mass, Luminosity, Temperature, Radius);
		}

		public void ChangeTemperature(double temp)
		{
			Change(Mass, Luminosity, temp, Radius);
		}

		public void ChangeRadius(double radius)
		{
			Change(Mass, Luminosity, Temperature, radius);
		}

		public void Change(double mass, double lum, double temp, double radius)
		{
			var data = (from row in _types
						orderby Math.Sqrt((!double.IsNaN(lum) ? Math.Pow(lum - row.Luminosity, 2.0) : 0.0) + 
										  (!double.IsNaN(radius) ? Math.Pow(radius - row.Radius, 2.0) : 0.0) +
									      (!double.IsNaN(mass) ? Math.Pow(mass - row.Mass, 2.0) : 0.0) +
										  (!double.IsNaN(temp) ? Math.Pow(temp / GlobalConstants.EARTH_SUN_TEMPERATURE - row.Temperature / GlobalConstants.EARTH_SUN_TEMPERATURE, 2.0) : 0.0))
						ascending
						select row).FirstOrDefault();

			StellarType st = StellarType.FromString(data.Type);
			SpectralClass = st.SpectralClass;
			LuminosityClass = st.LuminosityClass;
			SubType = st.SubType;

			double m = st.Mass;
			double l = st.Luminosity;
			double t = st.Temperature;
			double r = st.Radius;

			Mass = !double.IsNaN(mass) ? mass : st.Mass;
			Luminosity = !double.IsNaN(lum) ? lum : st.Luminosity;
			Temperature = !double.IsNaN(temp) ? temp : st.Temperature;
			Radius = !double.IsNaN(radius) ? radius : st.Radius;

			/*if(Mass != m && Luminosity == l)
			{
				Mass = Environment.LuminosityToMass(l);
			}
			else if (Mass == m && Luminosity != l)
			{
				Luminosity = Environment.MassToLuminosity(m);
			}

			// (R/Rs/((L/Ls)1/2))1/2 = (Ts/T)
			if (Temperature != t && Radius == r)
			{
				Temperature = (1.0/Math.Sqrt(Radius / Math.Sqrt(Luminosity))) * GlobalConstants.EARTH_SUN_TEMPERATURE;
			}
			// R/Rs = (Ts/T)2(L/Ls)1/2
			else if (Temperature == t && Radius != r)
			{
				Radius = Math.Pow((GlobalConstants.EARTH_SUN_TEMPERATURE / Temperature), 2.0) * Math.Sqrt(Luminosity);
			}*/


		}
		
		/// <summary>
		/// Give a SpectralType given a star luminosity ratio to Earth's sun.
		/// </summary>
		/// <remarks>
		/// Mostly valid for main sequence stars.
		/// </remarks>
		/// <param name="lum">Luminosity ratio to Earth's sun.</param>
		/// <param name="radius">Radius ratio to Earth's sun.</param>
		/// <returns></returns>
		public static StellarType FromLuminosityAndRadius(double lum, double radius = 1.0)
		{
			var data =	(from row in _types
						orderby Math.Sqrt(Math.Pow(lum - row.Luminosity, 2.0) + 
										  Math.Pow(radius - row.Radius, 2.0)) 
						ascending
						select row).FirstOrDefault();

			StellarType st = StellarType.FromString(data.Type);
			st.Luminosity = lum;
			st.Radius = radius;

			return st;
		}

		public static StellarType FromMassAndTemperature(double mass, double temp)
		{
			var data = (from row in _types
						orderby Math.Sqrt(Math.Pow(mass - row.Mass, 2.0) + 
						                  Math.Pow(temp / GlobalConstants.EARTH_SUN_TEMPERATURE - row.Temperature / GlobalConstants.EARTH_SUN_TEMPERATURE, 2.0)) 
						ascending
						select row).FirstOrDefault();

			StellarType st = StellarType.FromString(data.Type);
			st.Mass = mass;
			st.Temperature = temp;

			return st;
		}

		/// <summary>
		/// Give the SpectralType given a star mass ratio to Earth's sun.
		/// </summary>
		/// <remarks>
		/// Reference: http://homepage.physics.uiowa.edu/~pkaaret/s09/L12_starsmainseq.pdf
		/// La/Lb = (Ra^2*Ta^4)/(Rb^2*Tb^4)
		/// </remarks>
		/// <param name="mass">Mass ratio to earth's sun.</param>
		/// <param name="radius">Radius ratio to earth's sun.</param>
		/// <returns></returns>
		public static StellarType FromMassAndRadius(double mass, double radius = 1.0)
		{
			var data = (from row in _types
						orderby Math.Sqrt(Math.Pow(mass - row.Mass, 2.0) + 
										  Math.Pow(radius - row.Radius, 2.0)) 
						ascending
						select row).FirstOrDefault();

			StellarType st = StellarType.FromString(data.Type);
			st.Mass = mass;
			st.Radius = radius;

			return st;
		}

		public static StellarType FromTemperatureAndLuminosity(double eff_temp, double luminosity = 0.0)
		{
			var data = (from row in _types
						orderby Math.Sqrt(Math.Pow(eff_temp / GlobalConstants.EARTH_SUN_TEMPERATURE - row.Temperature / GlobalConstants.EARTH_SUN_TEMPERATURE, 2.0) + 
									      Math.Pow(luminosity - row.Luminosity, 2.0))
						ascending
						select row).FirstOrDefault();

			StellarType st = StellarType.FromString(data.Type);
			st.Temperature = eff_temp;
			st.Luminosity = luminosity;

			return st;
		}

		public static StellarType FromString(string st)
		{
			if (String.IsNullOrEmpty(st))
				return new StellarType(SpectralClass.Undefined, LuminosityClass.Undefined);

			try
			{
				var mt = Regex.Match(st, @"(\D*)(\d*)(\D*)?");
				SpectralClass sc = (SpectralClass)Enum.Parse(typeof(SpectralClass), mt.Groups[1].Value);
				LuminosityClass lc;
				if (!String.IsNullOrEmpty(mt.Groups[3].Value))
					lc = (LuminosityClass)Enum.Parse(typeof(LuminosityClass), mt.Groups[3].Value);
				else
					lc = LuminosityClass.O;
				int subType = Int32.Parse(mt.Groups[2].Value);

				return new StellarType(sc, lc, subType);
			}
			catch (Exception)
			{
				throw new ArgumentException();
			}
		}

		public override string ToString()
		{
			return (Enum.GetName(typeof(SpectralClass), SpectralClass) + 
					SubType.ToString() + 
					(LuminosityClass != LuminosityClass.O ? Enum.GetName(typeof(LuminosityClass), LuminosityClass) : ""));
		}
	}
}
