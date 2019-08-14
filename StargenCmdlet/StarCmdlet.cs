using System;
using System.IO;
using System.Linq.Expressions;
using System.Management.Automation;
using Primoris.Universe.Stargen;

using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Primoris.Universe.Stargen.Astrophysics;
using Primoris.Universe.Stargen.Astrophysics.Burrows;
using Primoris.Universe.Stargen.Services;

using Units = UnitsNet;
using Primoris.Universe.Stargen.Bodies;

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

	internal class StarMap : ClassMap<StellarBody>
	{
		public StarMap()
		{
			Map(m => m.StellarType).TypeConverter<StellarTypeConverter>();
			Map(m => m.Color).Ignore();
			Map(m => m.Name);
			Map(m => m.Age);
			Map(m => m.Life);
			Map(m => m.EcosphereRadius);
			Map(m => m.Luminosity);
			Map(m => m.Mass);
			Map(m => m.Radius);
			Map(m => m.Temperature);
			Map(m => m.BinarySemiMajorAxis);
			Map(m => m.BinaryEccentricity);
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
		public double Luminosity { get; set; } = 0.0;

		[Parameter]
		public double Mass { get; set; } = 0.0;

		[Parameter]
		public double Temperature { get; set; } = 0.0;

		[Parameter]
		public double Radius { get; set; } = 0.0;

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

				cw.WriteHeader<StellarBody>();
				cw.NextRecord();
				cw.WriteRecord<StellarBody>(sun);
				cw.Flush();

				w.Close();
            }
		}

        protected StellarBody GenerateStar()
        {
            StellarType st = StellarType.FromString(StarStellarType);
            if (!(Mass == 0.0) || !(Luminosity == 0.0)  || !(Temperature == 0.0) || !(Radius == 0.0))
                st.Change(Units.Mass.FromSolarMasses(Mass),
						  Units.Luminosity.FromSolarLuminosities(Luminosity),
						  Units.Temperature.FromKelvins(Temperature),
						  Units.Length.FromSolarRadiuses(Radius));

            if (Name == String.Empty)
            {
                var ng = new NameGenerator();
                Name = ng.NextName();
            }

			Provider.Use().WithAstrophysics(new BodyPhysics());
            return new Star(st, Name);
        }
	}
}
