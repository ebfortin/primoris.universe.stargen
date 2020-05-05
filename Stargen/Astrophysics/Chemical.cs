using Newtonsoft.Json;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnitsNet;


namespace Primoris.Universe.Stargen.Astrophysics
{

	// TODO abunde isn't used anywhere
	// TODO break out abundance into a separate class for star/planet profiles
	public class Chemical
	{
		private static IReadOnlyDictionary<string, Chemical> _all = null;

		/// <summary>
		/// Gets or sets all chemical available for the current simulation.
		/// </summary>
		/// <value>
		/// All.
		/// </value>
		public static IReadOnlyDictionary<string, Chemical> All 
		{ 
			get
			{
				if (_all == null)
					_all = Load();
				
				return _all;
			} 

			protected set
			{
				_all = value;
			} 
		}

		/// <summary>
		/// Gets or sets the atomic number.
		/// </summary>
		/// <value>
		/// The atomic number.
		/// </value>
		public int Num { get; set; }

		/// <summary>
		/// Gets or sets the chemical symbol.
		/// </summary>
		/// <value>
		/// The chemical symbol.
		/// </value>
		public string Symbol { get; set; }

		/// <summary>
		/// Gets or sets the display symbol.
		/// </summary>
		/// <value>
		/// The display symbol.
		/// </value>
		public string DisplaySymbol { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the weight.
		/// </summary>
		/// <value>
		/// The weight.
		/// </value>
		public Mass Weight { get; set; }

		/// <summary>
		/// Gets or sets the melting Temperature.
		/// </summary>
		/// <value>
		/// The melting Temperature.
		/// </value>
		public Temperature Melt { get; set; }

		/// <summary>
		/// Gets or sets the boiling Temperature.
		/// </summary>
		/// <remarks>
		/// This value is valid at Earth's atmospheric pressure.
		/// </remarks>
		/// <value>
		/// The boiling Temperature.
		/// </value>
		public Temperature Boil { get; set; }

		/// <summary>
		/// Gets or sets the density.
		/// </summary>
		/// <remarks>
		/// This value is valid at Earth's atmospheric pressure. 
		/// </remarks>
		/// <value>
		/// The density.
		/// </value>
		public Density Density { get; set; }
		public Ratio Abunde { get; set; }
		public Ratio Abunds { get; set; }  // Solar system abundance
		public Ratio Reactivity { get; set; }

		/// <summary>
		/// Gets or sets the maximum ipp (inspired partial pressure).
		/// </summary>
		/// <value>
		/// The maximum ipp.
		/// </value>
		public Pressure MaxIpp { get; set; } // Max inspired partial pressure im millibars

		/// <summary>
		/// Initializes a new instance of the <see cref="Chemical"/> class.
		/// </summary>
		/// <param name="an">An.</param>
		/// <param name="sym">The sym.</param>
		/// <param name="htmlsym">The htmlsym.</param>
		/// <param name="name">The name.</param>
		/// <param name="w">The w.</param>
		/// <param name="m">The m.</param>
		/// <param name="b">The b.</param>
		/// <param name="dens">The dens.</param>
		/// <param name="ae">The ae.</param>
		/// <param name="abs">The abs.</param>
		/// <param name="rea">The rea.</param>
		/// <param name="mipp">The mipp.</param>
		public Chemical(int an, string sym, string htmlsym, string name, double w, double m, double b, double dens, double ae, double abs, double rea, double mipp)
		{
			Num = an;
			Symbol = sym;
			DisplaySymbol = htmlsym;
			Name = name;
			Weight = Mass.FromGrams(w);
			Melt = Temperature.FromKelvins(m);
			Boil = Temperature.FromKelvins(b);
			Density = Density.FromGramsPerCubicCentimeter(dens);
			Abunde = Ratio.FromDecimalFractions(ae);
			Abunds = Ratio.FromDecimalFractions(abs);
			Reactivity = Ratio.FromDecimalFractions(rea);
			MaxIpp = Pressure.FromMillibars(mipp);
		}

		/// <summary>
		/// Reloads this instance.
		/// </summary>
		/// <returns></returns>
		public static IReadOnlyDictionary<string, Chemical> Reload()
		{
			All = null;
			return Load();
		}

		/// <summary>
		/// Loads this instance.
		/// </summary>
		/// <returns>Dictionary of Chemicals.</returns>
		public static IReadOnlyDictionary<string, Chemical> Load()
		{
			if (_all != null)
				return _all;

			var a = Assembly.GetExecutingAssembly();
			var s = a.GetManifestResourceStream("Primoris.Universe.Stargen.Resources.elements.dat");

			using (s)
			{
				return Load(s);
			}
		}

		/// <summary>
		/// Loads chemicals from the specified stream.
		/// </summary>
		/// <param name="s">The source stream.</param>
		/// <returns>Dictionary of Chemicals</returns>
		public static IReadOnlyDictionary<string, Chemical> Load(Stream s)
		{
			using (var r = new StreamReader(s))
			{
				return Load(r);
			}
		}

		/// <summary>
		/// Loads the chemicals from the specified TextReader.
		/// </summary>
		/// <param name="r">The source TextReader.</param>
		/// <returns>Dictionary of Chemicals.</returns>
		public static IReadOnlyDictionary<string, Chemical> Load(TextReader r)
		{
			var chemTable = new Dictionary<string, Chemical>();

			var json = r.ReadToEnd();
			var items = JsonConvert.DeserializeObject<List<List<object>>>(json);


			foreach (var item in items)
			{
				var num = Convert.ToInt32(item[0]);
				var sym = (string)item[1];
				var name = (string)item[2];
				var weight = Convert.ToDouble(item[3]);
				var melt = Convert.ToDouble(item[4]);
				var boil = Convert.ToDouble(item[5]);
				var dens = Convert.ToDouble(item[6]);
				var abunde = Convert.ToDouble(item[7]);
				var abunds = Convert.ToDouble(item[8]);
				var rea = Convert.ToDouble(item[9]);
				var maxIPP = (item.Count == 11 ? Convert.ToDouble(item[10]) : 0) * GlobalConstants.MMHG_TO_MILLIBARS;


				chemTable.Add(sym, new Chemical(num, sym, sym, name, weight, melt, boil, dens, abunde, abunds, rea, maxIPP));
			}

			All = chemTable;
			return All;
		}

		/// <summary>
		/// Loads chemicals from the specified file.
		/// </summary>
		/// <param name="file">The file path.</param>
		/// <returns>Dictionary of chemicals.</returns>
		public static IReadOnlyDictionary<string, Chemical> Load(string file)
		{
			using (StreamReader r = new StreamReader(file))
			{
				return Load(r);
			}
		}

		/// <summary>
		/// Converts to string.
		/// </summary>
		/// <returns>
		/// A string that represents the current object.
		/// </returns>
		public override string? ToString()
		{
			return Symbol;
		}
	}
}
