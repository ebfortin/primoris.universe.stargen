using System;
using System.Management.Automation;
using Primoris.Universe.Stargen;
using Primoris.Universe.Stargen.Data;


namespace Primoris.Universe.Stargen.Cmdlets
{
	[Cmdlet(VerbsCommon.Get, "Planets")]
	public class PlanetsCmdlet : StarCmdlet
	{
		[Parameter]
		public double DustRatio { get; set; } = 0.0;

		[Parameter]
		public double DustBorder { get; set; } = 0.3;

		[Parameter]
		public double EccentricityCoefficient { get; set; } = 0.077;

		protected override void ProcessRecord()
		{
			StellarType st = StellarType.FromString(StarStellarType);
			st.ChangeAll(Mass, Luminosity, Temperature, Radius);

			var sun = new Star(st);

			if (Name == String.Empty)
			{
				var ng = new NameGenerator();
				Name = ng.NextName();
			}

			var sys = Generator.GenerateStellarSystem(Name, new SystemGenerationOptions(), sun : sun);
			WriteObject(sys.Planets);
		}
	}
}
