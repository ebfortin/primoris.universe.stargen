using System;
using System.IO;
using System.Linq.Expressions;
using System.Management.Automation;
using Primoris.Universe.Stargen;
using Primoris.Universe.Stargen.Data;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;


namespace Primoris.Universe.Stargen.Cmdlets
{
	public class StellarTypeConverter : DefaultTypeConverter
	{
		public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
		{
			return StellarType.FromString(text);
		}

		public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
		{
			return ((StellarType)value).ToString();
		}
	}

	internal class StarMap : ClassMap<Star>
	{
		public StarMap()
		{
			Map(m => m.StellarType).TypeConverter<StellarTypeConverter>();
			Map(m => m.Color).Ignore();
			Map(m => m.Name);
			Map(m => m.AgeYears);
			Map(m => m.Life);
			Map(m => m.EcosphereRadiusAU);
			Map(m => m.Luminosity);
			Map(m => m.Mass);
			Map(m => m.Radius);
			Map(m => m.Temperature);
			Map(m => m.SemiMajorAxisAU);
			Map(m => m.Eccentricity);
		}
	}

	[Cmdlet(VerbsCommon.Get, "Star")]
	public class StarCmdlet : PSCmdlet
	{
		[Parameter]
		public string CsvOutputPath { get; set; } = String.Empty;

		[Parameter]
		public string Name { get; set; } = String.Empty;

		[Parameter]
		public double Luminosity { get; set; } = double.NaN;

		[Parameter]
		public double Mass { get; set; } = double.NaN;

		[Parameter]
		public double Temperature { get; set; } = double.NaN;

		[Parameter]
		public double Radius { get; set; } = double.NaN;

		[Parameter]
		public string StarStellarType { get; set; } = "G2V";

		protected override void ProcessRecord()
		{
			base.ProcessRecord();

            var sun = GenerateStar();
            if (String.IsNullOrEmpty(CsvOutputPath))
            {
                WriteObject(sun);
            }
            else
            {
				var conf = new Configuration();
				conf.RegisterClassMap<StarMap>();

				var f = new FileStream(CsvOutputPath, FileMode.Create);
				var w = new StreamWriter(f);
				var cw = new CsvWriter(w, conf);

				cw.WriteHeader<Star>();
				cw.NextRecord();
				cw.WriteRecord<Star>(sun);
				cw.Flush();

				w.Close();
            }
		}

        protected Star GenerateStar()
        {
            StellarType st = StellarType.FromString(StarStellarType);
            if (!double.IsNaN(Mass) || !double.IsNaN(Luminosity) || !double.IsNaN(Temperature) || !double.IsNaN(Radius))
                st.Change(Mass, Luminosity, Temperature, Radius);

            if (Name == String.Empty)
            {
                var ng = new NameGenerator();
                Name = ng.NextName();
            }

            return new Star(st, Name);
        }
	}
}
