using System;
using System.Management.Automation;
using Primoris.Universe.Stargen;
using Primoris.Universe.Stargen.Data;


namespace Primoris.Universe.Stargen.Cmdlets
{
	[Cmdlet(VerbsCommon.Get, "Planets")]
	public class StargenCmdlet : PSCmdlet
	{
		[Parameter(ParameterSetName = "LuminosityTemperature", Mandatory = true)]
		[Parameter(ParameterSetName = "LuminosityStellarType", Mandatory = true)]
		[Parameter(ParameterSetName = "MassTemperature", Mandatory = false)]
		[Parameter(ParameterSetName = "MassStellarType", Mandatory = false)]
		public double Luminosity { get; set; } = 1.0;

		[Parameter]
		[Parameter(ParameterSetName = "LuminosityTemperature", Mandatory = false)]
		[Parameter(ParameterSetName = "LuminosityStellarType", Mandatory = false)]
		[Parameter(ParameterSetName = "MassTemperature", Mandatory = true)]
		[Parameter(ParameterSetName = "MassStellarType", Mandatory = true)]
		public double Mass { get; set; } = 1.0;

		[Parameter(ParameterSetName = "LuminosityTemperature", Mandatory = true)]
		[Parameter(ParameterSetName = "LuminosityStellarType", Mandatory = false)]
		[Parameter(ParameterSetName = "MassTemperature", Mandatory = true)]
		[Parameter(ParameterSetName = "MassStellarType", Mandatory = false)]
		public double Temperature { get; set; } = 5780.0;

		[Parameter]
		[Parameter(ParameterSetName = "LuminosityTemperature", Mandatory = false)]
		[Parameter(ParameterSetName = "LuminosityStellarType", Mandatory = true)]
		[Parameter(ParameterSetName = "MassTemperature", Mandatory = false)]
		[Parameter(ParameterSetName = "MassStellarType", Mandatory = true)]
		public string StarStellarType { get; set; } = "G2V";

		[Parameter]
		public double DustRatio { get; set; } = 0.0;

		[Parameter]
		public double DustBorder { get; set; } = 0.3;

		[Parameter]
		public double EccentricityCoefficient { get; set; } = 0.077;

		protected override void ProcessRecord()
		{
			base.ProcessRecord();

			StellarType st = null;
			switch(ParameterSetName)
			{
				case "LuminosityTemperature":
					st = StellarType.FromTemperatureAndLuminosity(Luminosity, Temperature);
					break;
				case "LuminosityStellarType":
					st = StellarType.FromString(StarStellarType);
					st.ChangeLuminosity(Luminosity);
					break;
				case "MassTemperature":
					st = StellarType.FromMassAndTemperature(Mass, Temperature);
					break;
				case "MassStellarType":
					st = StellarType.FromString(StarStellarType);
					st.ChangeMass(Mass);
					break;
				default:
					ThrowTerminatingError(new ErrorRecord(new ArgumentException(), "STARGEN0001", ErrorCategory.InvalidArgument, st));
					break;
			}

			WriteObject(st);
		}
	}
}
