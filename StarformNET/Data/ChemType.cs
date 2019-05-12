using Newtonsoft.Json;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;


namespace DLS.StarformNET.Data
{

    // TODO abunde isn't used anywhere
    // TODO break out abundance into a separate class for star/planet profiles
    public class ChemType
    {
        public int    num        { get; set; }
        public string symbol     { get; set; }
        public string HTMLSymbol { get; set; }
        public string Name       { get; set; }
        public double weight     { get; set; }
        public double melt       { get; set; }
        public double boil       { get; set; }
        public double density    { get; set; }
        public double abunde     { get; set; }
        public double abunds     { get; set; }  // Solar system abundance
        public double reactivity { get; set; }
        public double max_ipp     { get; set; } // Max inspired partial pressure im millibars

        public ChemType(int an, string sym, string htmlsym, string name, double weight, double m, double b, double dens, double ae, double abs, double rea, double mipp)
        {
            num = an;
            symbol = sym;
            HTMLSymbol = htmlsym;
            Name = name;
            this.weight = weight;
            melt = m;
            boil = b;
            density = dens;
            abunde = ae;
            abunds = abs;
            reactivity = rea;
            max_ipp = mipp;
        }

		public static ChemType[] Load()
		{
			var a = Assembly.GetExecutingAssembly();
			var s = a.GetManifestResourceStream("DLS.StarformNET.elements.dat");

			using (s)
			{
				return Load(s);
			}
		}

		public static ChemType[] Load(Stream s)
		{
			using (var r = new StreamReader(s))
			{
				return Load(r);
			}
		}

		public static ChemType[] Load(TextReader r)
		{
			var chemTable = new List<ChemType>();

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


				chemTable.Add(new ChemType(num, sym, sym, name, weight, melt, boil, dens, abunde, abunds, rea, maxIPP));
			}

			return chemTable.ToArray();
		}

		public static ChemType[] Load(string file)
		{
			using (StreamReader r = new StreamReader(file))
			{
				return Load(r);
			}
		}

    }
}
