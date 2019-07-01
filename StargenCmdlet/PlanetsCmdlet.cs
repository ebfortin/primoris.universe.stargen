using System;
using System.Management.Automation;
using Primoris.Universe.Stargen;
using Primoris.Universe.Stargen.Data;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;


namespace Primoris.Universe.Stargen.Cmdlets
{
	internal class PlanetMap : ClassMap<Planet>
	{
		public PlanetMap()
		{
			Map(m => m.Atmosphere).Ignore();
			Map(m => m.SemiMajorAxisAU);
			Map(m => m.Eccentricity);
			Map(m => m.AxialTilt);
			Map(m => m.OrbitZone);
			Map(m => m.OrbitalPeriod);
			Map(m => m.AngularVelocityRadSec);
			Map(m => m.DayLength);
			Map(m => m.HillSphere);
			Map(m => m.MassSM);
			Map(m => m.DustMassSM);
			Map(m => m.GasMassSM);
			Map(m => m.EscapeVelocityCMSec);
			Map(m => m.SurfaceAccelerationCMSec2);
			Map(m => m.SurfaceGravityG);
			Map(m => m.CoreRadius);
			Map(m => m.Radius);
			Map(m => m.DensityGCC);
			Map(m => m.Type);
			Map(m => m.IsTidallyLocked);
			Map(m => m.IsEarthlike);
			Map(m => m.IsHabitable);
			Map(m => m.HasResonantPeriod);
			Map(m => m.HasGreenhouseEffect);
			Map(m => m.RMSVelocityCMSec);
			Map(m => m.MolecularWeightRetained);
			Map(m => m.VolatileGasInventory);
			Map(m => m.BoilingPointWater);
			Map(m => m.Albedo);
			Map(m => m.Illumination);
			Map(m => m.ExosphereTemperature);
			Map(m => m.SurfaceTemperature);
			Map(m => m.GreenhouseRiseTemperature);
			Map(m => m.DaytimeTemperature);
			Map(m => m.NighttimeTemperature);
			Map(m => m.MaxTemperature);
			Map(m => m.WaterCoverFraction);
			Map(m => m.CloudCoverFraction);
			Map(m => m.IceCoverFraction);
		}
	}


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
            var sun = GenerateStar();

            var sys = Generator.GenerateStellarSystem(Name, new SystemGenerationOptions(), sun : sun);
            if (String.IsNullOrEmpty(CsvOutputPath))
            {
                WriteObject(sun);
                WriteObject(sys.Planets);
            }
            else
            {
				var conf = new CsvHelper.Configuration.Configuration();
				conf.RegisterClassMap<PlanetMap>();

				var f = new FileStream(CsvOutputPath, FileMode.Create);
				var w = new StreamWriter(f);
				var cw = new CsvWriter(w, conf);

				cw.WriteHeader<Planet>();
				cw.NextRecord();
				cw.WriteRecords<Planet>(sys.Planets);
				cw.Flush();

				w.Close();
			}
		}
	}
}
