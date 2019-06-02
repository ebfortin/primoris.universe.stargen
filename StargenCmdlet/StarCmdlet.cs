using System;
using System.Management.Automation;
using Primoris.Universe.Stargen;
using Primoris.Universe.Stargen.Data;


namespace Primoris.Universe.Stargen.Cmdlets
{
	[Cmdlet(VerbsCommon.Get, "Star")]
	public class StarCmdlet : PSCmdlet
	{
		[Parameter]
		public bool CsvOutput { get; set; } = false;

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

			StellarType st = StellarType.FromString(StarStellarType);
			st.Change(Mass, Luminosity, Temperature, Radius);

			if (Name == String.Empty)
			{
				var ng = new NameGenerator();
				Name = ng.NextName();
			}

			var sun = new Star(st);
			sun.Name = Name;
			WriteObject(sun);
		}
	}
}
