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
		/*
		[Parameter]
		public double DustRatio { get; set; } = 0.0;

		[Parameter]
		public double DustBorder { get; set; } = 0.3;

		[Parameter]
		public double EccentricityCoefficient { get; set; } = 0.077;
		*/
		protected override void ProcessRecord()
		{
			base.ProcessRecord();

			StellarType st = StellarType.FromString(StarStellarType);
			st.ChangeAll(Mass, Luminosity, Temperature, Radius);

			var sun = new Star(st);
			WriteObject(sun);
		}
	}
}
