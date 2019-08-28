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

		public int Num { get; set; }
		public string Symbol { get; set; }
		public string DisplaySymbol { get; set; }
		public string Name { get; set; }
		public Mass Weight { get; set; }
		public Temperature Melt { get; set; }
		public Temperature Boil { get; set; }
		public Density Density { get; set; }
		public Ratio Abunde { get; set; }
		public Ratio Abunds { get; set; }  // Solar system abundance
		public Ratio Reactivity { get; set; }
		public Pressure MaxIpp { get; set; } // Max inspired partial pressure im millibars

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

		public static IReadOnlyDictionary<string, Chemical> Reload()
		{
			All = null;
			return Load();
		}

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

		public static IReadOnlyDictionary<string, Chemical> Load(Stream s)
		{
			using (var r = new StreamReader(s))
			{
				return Load(r);
			}
		}

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

		public static IReadOnlyDictionary<string, Chemical> Load(string file)
		{
			using (StreamReader r = new StreamReader(file))
			{
				return Load(r);
			}
		}

		public override string? ToString()
		{
			return Symbol;
		}
	}
}
